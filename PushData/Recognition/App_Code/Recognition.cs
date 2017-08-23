using System;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;


/// <summary>
/// Summary description for Recognition
/// </summary>
[WebService(Namespace = "http://mix.intuitivelabs.net/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Recognition : System.Web.Services.WebService {

    public Recognition () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public XmlDocument RegisterSource(string Nickname, string Key)
    {
        Guid sourceGUID = Guid.NewGuid();
        Guid token = Guid.NewGuid();
        
        System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;

        /*Capture some information about who is registering as a source for future auditing*/
        string signature = "<Signature>"
        + "<Property Name=\"IPAddress\"><Value>" + request.UserHostAddress + "</Value></Property>";

        if (request.UserHostAddress != request.UserHostName)
        signature += "<Property Name=\"DNSName\"><Value>" + request.UserHostName + "</Value></Property>";

        signature += "</Signature>";

        /*Register in the source in the MIXRegistry*/
        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandTimeout = 60;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "Source_Register";

        cmd.Parameters.Add("@GUID", SqlDbType.UniqueIdentifier).Value = sourceGUID;
        cmd.Parameters.Add("@Nickname", SqlDbType.NVarChar, 128).Value = Nickname;
        cmd.Parameters.Add("@Key", SqlDbType.Xml, -1).Value = Key;
        cmd.Parameters.Add("@Token", SqlDbType.UniqueIdentifier).Value = token;
        cmd.Parameters.Add("@Signature", SqlDbType.Xml, -1).Value = signature;

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        /*Provider for allowing client to encrypt data sent to MIX*/
        RSACryptoServiceProvider provider = CreateCryptoProvider( sourceGUID.ToString() );

        /*Provider for encryping data sent to client (MIX source) */
        RSACryptoServiceProvider encryptionProvider = CreateEncryptoProvider(Key);

        RijsdaelProvider rijsProvider = new RijsdaelProvider();

        string outXml = "<Result>"
            + "<Key>" + Convert.ToBase64String(encryptionProvider.Encrypt(rijsProvider.Algorithm.Key,true)) + "</Key>"
            + "<IV>" + Convert.ToBase64String(encryptionProvider.Encrypt(rijsProvider.Algorithm.IV, true)) + "</IV>"
            + "<Content>";

        //Using Rijndael, encrypt the public key sent to back to the registering Source
        string content = "<Source><Token>" + token.ToString() + "</Token>"
            + "<SourceGUID>" + sourceGUID.ToString() + "</SourceGUID>"
            + "<Key>" + provider.ToXmlString(false) + "</Key></Source>";


        outXml += rijsProvider.Encrypt(content)
            + "</Content>"
            + "</Result>";

        /*Create XML response containing client's authentication token and public key to encrypt data sent to MIX*/
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(outXml);

        provider.Clear();
        encryptionProvider.Clear();

        return doc;
    }

    [WebMethod]
    public string ExchangeToken(string OldToken, string SourceGUID)
    {
        RSACryptoServiceProvider provider = CreateCryptoProvider( SourceGUID );

        Guid oldToken = DecryptToken(provider, OldToken); 
        Guid newToken = Guid.NewGuid();

        /*Exchage the old and new token in the MIXRegistry*/
        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandTimeout = 60;
        cmd.CommandType = CommandType.Text;

        cmd.Parameters.Add("@NewToken", SqlDbType.UniqueIdentifier).Value = newToken;
        cmd.Parameters.Add("@OldToken", SqlDbType.UniqueIdentifier).Value = oldToken;

        SqlParameter param = new SqlParameter("@Key", SqlDbType.Xml, -1);
        param.Direction = ParameterDirection.Output; ;
        cmd.Parameters.Add(param);

        cmd.CommandText = "update Source set Token = @NewToken where Token = @OldToken"
            + " set @Key = (select [Key] from Source where Token = @NewToken)";

        conn.Open();
        int affected = cmd.ExecuteNonQuery();
        conn.Close();

        /* If no record was updated, assume the reason is that the Source was never registered. */
        /* However, this would be suspicious since the token was properly deciphered. */
        /* Consider adding an audit trail and send a warning to the MIX administrators. */
        if (affected == 0)
            throw new NotRegisterdSourceException();

        RSACryptoServiceProvider encryptoProvider = CreateEncryptoProvider(param.Value.ToString());

        string str = Convert.ToBase64String(encryptoProvider.Encrypt(newToken.ToByteArray(), true)); 

        provider.Clear();
        encryptoProvider.Clear();

        return str;         
    }

    [WebMethod]
    public string VerifyToken( string Token, string SourceGUID )
    {
        RSACryptoServiceProvider provider = CreateCryptoProvider(SourceGUID);

        string str = DecryptToken(provider, Token).ToString();

        provider.Clear();

        return str;
    }
   
    [WebMethod]
    public XmlDocument ExchangeKey(string Key, string IV, string NewKey, string Token, string SourceGUID)
    {
        string outXml = String.Empty;

        RSACryptoServiceProvider oldProvider = CreateCryptoProvider(SourceGUID);
        
        Guid token = DecryptToken(oldProvider, Token);

        RijsdaelProvider rijsProvider = new RijsdaelProvider( Key, IV, ref oldProvider );
        
        string newKey = rijsProvider.Decrypt( NewKey );

        //Store new public key provided by source in MIXRegistry
        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandTimeout = 60;
        cmd.CommandType = CommandType.Text;

        cmd.Parameters.Add("@NewKey", SqlDbType.Xml, -1).Value = newKey;
        cmd.Parameters.Add("@Token", SqlDbType.UniqueIdentifier).Value = token;

        cmd.CommandText = "update Source set [Key] = @NewKey where Token = @Token";

        conn.Open();
        int affected = cmd.ExecuteNonQuery();
        conn.Close();

        //If nothing was affcted, it is an indication of something strange. Consider warning MIX administrators.
        if (affected == 0)
            throw new NotRegisterdSourceException();

        //Clear old private, public ky pair and generate a new one.
        oldProvider.PersistKeyInCsp = false;
        oldProvider.Clear();

        //The new key from the source is used to encrypt the information being sent back.
        RSACryptoServiceProvider clientProvider = CreateEncryptoProvider(newKey);
        
        RSACryptoServiceProvider provider = CreateCryptoProvider(SourceGUID);

        RijsdaelProvider newRijsProv = new RijsdaelProvider();

        //Return the new stuff. This packet of XML information is staring to look like a standard. Create class to simplify passing it.
        outXml = "<Source>" 
            + "<Key>" +  Convert.ToBase64String( clientProvider.Encrypt( newRijsProv.Algorithm.Key, true ))  + "</Key>"
            + "<IV>" + Convert.ToBase64String( clientProvider.Encrypt(newRijsProv.Algorithm.IV, true)) + "</IV>"
            + "<Content>" + newRijsProv.Encrypt( provider.ToXmlString(false) ) + "</Content>"
            + "</Source>";

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(outXml);

        newRijsProv.Clear();
        provider.Clear();
        clientProvider.Clear();

        return doc;
    }    

    [WebMethod]
    public XmlDocument GetSignatureTemplate( string ObjectClass ) 
    {
      XmlDocument doc = new XmlDocument();

      //Test. Eventually load from MIXRegistry
      doc.LoadXml("<SignatureTemplate ObjectClass=\"Node\"><Property Name=\"HostName\"/><Property Name=\"IPAddress\"/><Property Name=\"IPAssignment\"/><Property Name=\"DomainName\"/></SignatureTemplate>");
      return doc;
    }

    [WebMethod]
    public string EmulateClientDecrypt(string Key, string IV, string Text)
    {
        CspParameters cspParams = new CspParameters();
        cspParams.KeyContainerName = "ClientEmulator";
        
        RSACryptoServiceProvider provider = new RSACryptoServiceProvider(cspParams);

        RijsdaelProvider rijsProvider = new RijsdaelProvider
            (
                provider.Decrypt(Convert.FromBase64String(Key), true),
                provider.Decrypt(Convert.FromBase64String(IV), true)
            );

        string str = rijsProvider.Decrypt(Text);

        provider.Clear();
        rijsProvider.Clear();

        return str;

        //return "uh";
    }

    [WebMethod]
    public XmlDocument EmulateClientEncrypt(string Text, string Key)
    {
        string outXml = String.Empty;

        CspParameters cspParams = new CspParameters();
        cspParams.Flags = CspProviderFlags.UseMachineKeyStore;

        RSACryptoServiceProvider provider = new RSACryptoServiceProvider(cspParams);
        provider.FromXmlString(Key);
        
        //byte[] cipher = provider.Encrypt(System.Text.Encoding.Default.GetBytes( Token ), true);

        RijsdaelProvider rijsProvider = new RijsdaelProvider();
        outXml += "<Result><Key>" + Convert.ToBase64String( provider.Encrypt(rijsProvider.Algorithm.Key, true ) ) + "</Key>"
            + "<IV>" + Convert.ToBase64String( provider.Encrypt(rijsProvider.Algorithm.IV,true) ) + "</IV>"
            + "<Content>" + rijsProvider.Encrypt(Text) + "</Content></Result>";

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(outXml);


        provider.Clear();
        rijsProvider.Clear();

        return doc;
    }

    private string Decrypt(RSACryptoServiceProvider provider, string text)
    {
        byte[] decipher = provider.Decrypt(Convert.FromBase64String(text), true);

        return System.Text.Encoding.Default.GetString(decipher);
    }

    private Guid DecryptToken(RSACryptoServiceProvider provider, string token )
    {
        byte[] decipher = provider.Decrypt( Convert.FromBase64String( token ), true );

        try
        {            
            return new Guid(System.Text.Encoding.Default.GetString(decipher));
        }
        catch
        {
            throw new NotRegisterdSourceException();
        }
    }

    private RSACryptoServiceProvider CreateEncryptoProvider(string key)
    {
        CspParameters cspParams = new CspParameters();
        cspParams.Flags = CspProviderFlags.UseMachineKeyStore;        
        
        RSACryptoServiceProvider provider = new RSACryptoServiceProvider(2048,cspParams);        
        provider.FromXmlString(key);       

        return provider;
    }

    private RSACryptoServiceProvider CreateCryptoProvider(string sourceGUID )
    {
        CspParameters cspParams = new CspParameters();
        cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
        cspParams.KeyContainerName = sourceGUID;

        RSACryptoServiceProvider provider = new RSACryptoServiceProvider(cspParams);
        provider.PersistKeyInCsp = true;
        
        return provider;
    }

        [WebMethod]
    public XmlDocument GetEmulationKey()
    {
        CspParameters cspParams = new CspParameters();
        cspParams.KeyContainerName = "ClientEmulator";
        RSACryptoServiceProvider provider = new RSACryptoServiceProvider(cspParams);
        provider.PersistKeyInCsp = true;

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(provider.ToXmlString( false ));

        provider.Clear();

        return doc; 
    }

    private string GetMIX1Identifier(Guid Token, string Domain, string ObjectClass, string LocalID, string ParentID)
    {
        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "Object_GetMIX1Keys";

        cmd.Parameters.Add("@Token", SqlDbType.UniqueIdentifier).Value = Token;
        cmd.Parameters.Add("@Domain", SqlDbType.NVarChar, 64).Value = Domain;
        cmd.Parameters.Add("@ObjectClass", SqlDbType.NVarChar, 64).Value = ObjectClass;
        cmd.Parameters.Add("@LocalID", SqlDbType.NVarChar, 128).Value = LocalID;

        if( ParentID != null )
            cmd.Parameters.Add("@ParentID", SqlDbType.NVarChar, 128).Value = ParentID;
        else
            cmd.Parameters.Add("@ParentID", SqlDbType.NVarChar, 128).Value = DBNull.Value;

        SqlDataAdapter da = new SqlDataAdapter(cmd);

        DataSet ds = new DataSet();
        
        conn.Open();
        da.Fill(ds);
        conn.Close();

        string identifier = string.Empty;

        int n = 0;
        int last  = ds.Tables[0].Rows.Count - 1;

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            identifier += row[0].ToString();

            if( n < last )
                 identifier += ";" ;

            n++;
        }

        return identifier;
    }

    [WebMethod]
    public string SendObject(string Object, string Token, string SourceGUID)
    {
        RSACryptoServiceProvider provider = CreateCryptoProvider(SourceGUID);
        Guid token = DecryptToken(provider, Token);

        string str = string.Empty;

        bool storeInMIX1 = false;

        if (System.Configuration.ConfigurationManager.AppSettings["StoreInMix1"] != null)
            storeInMIX1 = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["StoreInMix1"]);
        
        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandTimeout = 1800;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "Object_Identify";

        cmd.Parameters.Add("@Object", SqlDbType.Xml, -1).Value = Object;
        cmd.Parameters.Add("@Token", SqlDbType.UniqueIdentifier).Value = token;

        SqlParameter param = new SqlParameter("@MixGUID", SqlDbType.UniqueIdentifier);
        param.Direction = ParameterDirection.Output;
        cmd.Parameters.Add(param);

        SqlParameter testParam = new SqlParameter("@Test", SqlDbType.NVarChar, -1);
        testParam.Direction = ParameterDirection.Output;
        cmd.Parameters.Add(testParam);

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        if (storeInMIX1)
            str = SendToMIX1(Object, token) + "\n";

        return str + testParam.Value.ToString() + ":" + param.Value.ToString();
    }

    private string SendToMIX1(string Object, Guid Token)
    {
        MIX1.Services.Updater ws = new MIX1.Services.Updater();
        ws.Credentials = System.Net.CredentialCache.DefaultCredentials;
        
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(Object);

        string domain = doc.DocumentElement.Attributes["Domain"].Value;

        string str = string.Empty;

        string dataSource = string.Empty;

        //Get DataSource Name
        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        //cmd.CommandTimeout = 1;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "Source_GetNickname";

        cmd.Parameters.Add("@Token", SqlDbType.UniqueIdentifier).Value = Token;

        SqlParameter nameParam = new SqlParameter("@Nickname", SqlDbType.NVarChar, 128);
        nameParam.Direction = ParameterDirection.Output;
        cmd.Parameters.Add(nameParam);

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        if (nameParam.Value != DBNull.Value)
            dataSource = nameParam.Value.ToString();
        else
            throw new Exception("Source is unknown");

        str = dataSource + "\n";

        //Convert data to MIX1 schema
        string objClass = string.Empty;

        XmlNode parent = doc.DocumentElement.SelectSingleNode("./Parent");
        string parentID = null;
        string objName = string.Empty;

        if (parent != null)
        {
            if (parent.Attributes["ObjectClass"] == null)
                throw new Exception("Parent has no ObjectClass.");

            objClass = parent.Attributes["ObjectClass"].Value.Trim();

            if (objClass == string.Empty)
                throw new Exception("Parent's ObjectClass can't be an empty string.");


            //Figure out MIX1 identifier of Parent

            XmlNode lidNode = parent.SelectSingleNode("./LocalID");

            if (lidNode == null)
                throw new Exception("Parent has no LocalID.");

            parentID = lidNode.InnerText.Trim();

            if( parentID == string.Empty )
                throw new Exception("Parent cannot have a an empty string for LocalID.");

            objName = GetMIX1Identifier(Token, domain, objClass, parentID, null);

        }
        else
        {
            //Get Attributes from KeyProperty?
            return string.Empty;
        }
        
        string dataClass = string.Empty;

        if (objClass.Trim() != string.Empty )
        {
            if ( doc.DocumentElement.Attributes["ObjectClass"] == null )
                throw new Exception("No ObjectClass is defined for the Object");

            dataClass = doc.DocumentElement.Attributes["ObjectClass"].Value.Trim();

            if (dataClass == string.Empty)
                throw new Exception("ObjectClass can't be an empty string.");

            //Figure out MIX1 identifier
            string localID = string.Empty;

            XmlNode lidNode = doc.DocumentElement.SelectSingleNode("./LocalID");

            if (lidNode == null)
                throw new Exception("Object has no LocalID.");

            localID = lidNode.InnerText.Trim();

            if( localID == string.Empty )
                throw new Exception("Object cannot have a an empty string for LocalID.");

            string instName = string.Empty;

            instName = GetMIX1Identifier(Token, domain, dataClass, localID, parentID);

            XmlNodeList props = doc.DocumentElement.SelectNodes("./Signature/Property");

            MIX1Instance inst = new MIX1Instance( objClass, dataClass );
            inst.ObjectName = objName;
            inst.InstanceName = instName;
            
            System.Collections.Generic.List<MIX1DataPoint> dataPoints = new System.Collections.Generic.List<MIX1DataPoint>();

            //MIX1DataPoint[] dataPoints = new MIX1DataPoint[ props.Count ];
            bool first = true;
                        
            foreach( XmlNode node in props )
            {
                if (node.Attributes["Name"] == null)
                    throw new Exception("Name of Property is missing");

                string attr = node.Attributes["Name"].Value.Trim();

                if( attr == String.Empty )
                    throw new Exception("Name of Property can't be an empty string");

                XmlNodeList vals = node.SelectNodes("./Value");

                /*
                if (inst.IsKeyProperty(attr))
                {
                    if (!first)
                        inst.InstanceName = " ";
                    else
                        first = false;

                    inst.InstanceName += vals[0].InnerText;
                }
                */

                foreach (XmlNode val in vals)
                {
                    MIX1DataPoint dp = new MIX1DataPoint();
                    dp.Attribute = attr;
                    dp.Value = val.InnerText;

                    dataPoints.Add(dp);

                    //str += objName + ":" + objClass + "." + dataClass + "." + attr + ":" + val.InnerText + "\n";                    
                }
            }
            
            foreach( MIX1DataPoint dp in dataPoints )
            {
                ws.UpdateAttribute(dataSource, inst.ObjectClass, inst.DataClass, dp.Attribute, inst.ObjectName, inst.InstanceName, dp.Value, DateTime.Now);
                str += inst.ObjectName + ": " + inst.InstanceName + ": " + inst.ObjectClass + "." + inst.DataClass + "." + dp.Attribute + ":" + dp.Value + "\n";
            }

            return str;
        }

        return dataSource;
    }

    public class NotRegisterdSourceException : Exception
    {
        public NotRegisterdSourceException(): base("You are not a registered source.")
        {
        }
    }
}

