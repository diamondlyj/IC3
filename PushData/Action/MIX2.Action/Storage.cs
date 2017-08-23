using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MIX2.Action
{
    public class Input : Xcellence.IInput
    {
        private bool isTest;
        private Dictionary< string, Xcellence.MemoryStorage> stores;

        public Input( Dictionary< string, Xcellence.MemoryStorage> objects, bool isTest )
        {
            this.isTest = isTest;
            this.stores = objects;
        }

        public event Xcellence.ReadInputHandler OnRead;

        public IEnumerable<Xcellence.Object> Get(IEnumerable<Xcellence.PathPoint> predicate)
        {
            if (predicate != null)
            {
                foreach (Xcellence.PathPoint s in predicate)
                {
                    if (s.Object is string)
                    {
                        string key = s.Object.ToString();

                        if (this.stores.ContainsKey(key))
                        {
                            foreach (Xcellence.Object obj in this.stores[key].GetWorld(null))
                            {
                                if (this.OnRead != null)
                                    OnRead(this, obj);

                                yield return obj;
                            }
                        }
                    }
                }
            }        
        }
    }

    public class Storage: Xcellence.IStorage
    {
        private bool isTest;
        private string processIdentifier;

        public Storage( bool isTest )
        {
            this.processIdentifier = string.Empty;
            this.isTest = isTest;
        }

        public event Xcellence.WriteObjectHandler OnWrite;
        public event Xcellence.ReadObjectHandler OnRead;
        public event Xcellence.CategorizeObjectHandler OnCategorize;

        public string ProcessIdentifier
        {
            get { return this.processIdentifier; }
            set { this.processIdentifier = value; }
        }

        public void AddObject( Xcellence.Object obj )
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { 
                throw new NotImplementedException(); 
            }
        }

        public IEnumerable<Xcellence.Object> GetWorld(IEnumerable<Xcellence.PathPoint> predicate)
        {
            SqlConnection conn = new SqlConnection( System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);
            SqlConnection conn2 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);
            
            conn.Open();
            conn2.Open();

            if (predicate == null)
            {
                List<Xcellence.PathPoint> preds = new List<Xcellence.PathPoint>();

                using (SqlCommand command = new SqlCommand(" select [Name] from [ObjectClass]", conn))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                        preds.Add(new Xcellence.PathPoint(null, reader[0].ToString()));

                    reader.Close();
                }

                predicate = preds.ToArray();
            }


            foreach (Xcellence.PathPoint s in predicate)
            {
                if (s.Object is string)
                {
                    SqlParameter param = new SqlParameter("@ObjectClass", System.Data.SqlDbType.NVarChar, 64);

                    string str = (string)s.Object.ToString();
                    param.Value = str;

                    using (SqlCommand command = new SqlCommand("declare @ocid int;"
                        + " set @ocid = (select [ID] from [ObjectClass] where [Name] = @ObjectClass);"
                        + " select [ObjectClassID],[GUID],[FriendlyName], [ParentClassID], [ParentGUID] from [Object] where ObjectClassID = @ocid;", conn))
                    {
                        command.Parameters.Add(param);

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Guid guid = reader.GetGuid(1);

                            Xcellence.Object obj = new Xcellence.Object(guid, this);
                            obj.Class.Add(str);

                            string fname = null;

                            if (!reader.IsDBNull(2))
                                fname = reader.GetString(2);

                            if (fname != null)
                            {
                                Xcellence.Property fprop = new Xcellence.Property(obj, "FriendlyName");
                                fprop.Values.Add(new Xcellence.Value(fprop, fname));
                                obj.Meta.Property.Add(fprop);
                            }

                            SqlParameter guidParam = new SqlParameter("@guid", System.Data.SqlDbType.UniqueIdentifier);
                            guidParam.Value = guid;

                            SqlParameter ocidParam = new SqlParameter("@ocid", System.Data.SqlDbType.Int);
                            ocidParam.Value = reader.GetInt32(0);

                            using (SqlCommand propCommand = new SqlCommand("select [PropertyClass].[Name], [Property].[Value] from [Property], [PropertyClass] where [Property].[ObjectClassID] = @ocid and [Property].[ObjectGUID] = @guid and [PropertyClass].[ID] = [Property].[PropertyClassID] order by [PropertyClass].[Name];", conn2))
                            {
                                propCommand.Parameters.Add(guidParam);
                                propCommand.Parameters.Add(ocidParam);

                                SqlDataReader propReader = propCommand.ExecuteReader();

                                string propName = string.Empty;
                                Xcellence.Property prop = new Xcellence.Property(obj, "");

                                while (propReader.Read())
                                {
                                    string nextName = propReader.GetString(0);

                                    if (nextName != propName)
                                    {
                                        if (propName != string.Empty)
                                            obj.Signature.Property.Add(prop);

                                        propName = nextName;
                                        prop = new Xcellence.Property(obj, propName);
                                    }

                                    prop.AddValue(propReader.GetString(1));
                                }

                                if (propName != string.Empty)
                                    obj.Signature.Property.Add(prop);

                                propReader.Close();
                            }

                            //select children
                            using (SqlCommand relCommand = new SqlCommand(
                                "select [ObjectClass].[Name], [Object].[GUID] from [Object], [ObjectClass]"
                                    + " where ("
                                    + " ([Object].[ParentClassID] = @ocid and [Object].[ParentGUID] = @guid)"
                                    + " ) and [ObjectClass].[ID] = [Object].[ObjectClassID]"
                                    + " order by [ObjectClass].[Name];", conn2)
                                )
                            {
                                SqlParameter guidParam2 = new SqlParameter("@guid", System.Data.SqlDbType.UniqueIdentifier);
                                guidParam2.Value = guidParam.Value;

                                SqlParameter ocidParam2 = new SqlParameter("@ocid", System.Data.SqlDbType.Int);
                                ocidParam2.Value = ocidParam.Value;

                                relCommand.Parameters.Add(guidParam2);
                                relCommand.Parameters.Add(ocidParam2);

                                SqlDataReader relReader = relCommand.ExecuteReader();

                                string relName = string.Empty;
                                Xcellence.Relation rel = new Xcellence.Relation(obj);
                                Xcellence.Link link = new Xcellence.Link(rel);

                                while (relReader.Read())
                                {
                                    string nextName = relReader.GetString(0);

                                    if (nextName != relName)
                                    {
                                        if (relName != string.Empty)
                                        {
                                            rel.Link.Add(link);
                                            obj.Network.Add(rel);
                                        }

                                        relName = nextName;
                                        rel = new Xcellence.Relation(obj);
                                        rel.Name.Add(relName);
                                        link = new Xcellence.Link(rel);
                                    }



                                    Xcellence.Reference r = new Xcellence.Reference(link);
                                    r.GUID = relReader.GetGuid(1);

                                    if (relName != string.Empty)
                                        r.RelationType.Add(relName);

                                    link.Reference.Add(r);
                                }

                                if (relName != string.Empty)
                                {
                                    rel.Link.Add(link);
                                    obj.Network.Add(rel);
                                }

                                relReader.Close();
                            }


                            //select parent

                            if (!reader.IsDBNull(3) && !reader.IsDBNull(4))
                            {
                                using (SqlCommand relCommand = new SqlCommand(
                                    "select [ObjectClass].[Name], [Object].[GUID] from [Object], [ObjectClass]"
                                        + " where ("
                                        + " ([Object].[ObjectClassID] = @ocid and [Object].[GUID] = @guid)"
                                        + " ) and [ObjectClass].[ID] = [Object].[ObjectClassID]"
                                        + " order by [ObjectClass].[Name];", conn2)
                                    )
                                {
                                    SqlParameter guidParam2 = new SqlParameter("@guid", System.Data.SqlDbType.UniqueIdentifier);
                                    guidParam2.Value = reader.GetGuid(4);

                                    SqlParameter ocidParam2 = new SqlParameter("@ocid", System.Data.SqlDbType.Int);
                                    ocidParam2.Value = reader.GetInt32(3);

                                    relCommand.Parameters.Add(guidParam2);
                                    relCommand.Parameters.Add(ocidParam2);

                                    SqlDataReader relReader = relCommand.ExecuteReader();

                                    string relName = string.Empty;
                                    Xcellence.Relation rel = new Xcellence.Relation(obj);
                                    Xcellence.Link link = new Xcellence.Link(rel);

                                    while (relReader.Read())
                                    {
                                        string nextName = relReader.GetString(0);

                                        if (nextName != relName)
                                        {
                                            if (relName != string.Empty)
                                            {
                                                rel.Link.Add(link);
                                                obj.Network.Add(rel);
                                            }

                                            relName = nextName;
                                            rel = new Xcellence.Relation(obj);
                                            rel.Name.Add(relName);
                                            link = new Xcellence.Link(rel);
                                        }



                                        Xcellence.Reference r = new Xcellence.Reference(link);
                                        r.GUID = relReader.GetGuid(1);

                                        if (relName != string.Empty)
                                            r.RelationType.Add(relName);

                                        link.Reference.Add(r);
                                    }

                                    if (relName != string.Empty)
                                    {
                                        rel.Link.Add(link);
                                        obj.Network.Add(rel);
                                    }

                                    relReader.Close();
                                }
                            }

                            if (this.OnRead != null)
                                this.OnRead(this, obj);

                            yield return obj;
                        }

                        reader.Close();
                    }
                }
            }
        
            conn.Close();
            conn2.Close();
        }

        private Xcellence.Object GetObject(ref SqlConnection conn, string className, Guid guid)
        {
            SqlParameter domainParam = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
            domainParam.Value = System.Configuration.ConfigurationManager.AppSettings["Domain"];

            SqlParameter ocParam = new SqlParameter("@ObjectClass", System.Data.SqlDbType.NVarChar, 64);
            ocParam.Value = className;

            SqlParameter guidParam = new SqlParameter("@GUID", System.Data.SqlDbType.UniqueIdentifier);
            guidParam.Value = guid;

            SqlParameter ocidParam = new SqlParameter("@ocid", System.Data.SqlDbType.Int);
            ocidParam.Direction = System.Data.ParameterDirection.Output;

            int pocid = -1;
            Guid pguid = Guid.Empty;

            bool exists = false;

            Xcellence.Object obj = new Xcellence.Object(guid,this);
            obj.Class.Add(className);

            using
                (
                SqlCommand command = new SqlCommand
                    (
                    "set @ocid = (select [ID] from [ObjectClass] where [Name] = @ObjectClass and [DomainID] = (select [ID] from [Domain] where [Name] = @Domain));"
                    + "select [GUID],[FriendlyName],[Created],[Confirmed],[State],[ParentClassID], [ParentGUID] from [Object] where ObjectClassID = @ocid and [GUID] = @GUID;",
                    conn
                    )
                )
            {
                command.Parameters.Add(domainParam);
                command.Parameters.Add(ocParam);
                command.Parameters.Add(ocidParam);
                command.Parameters.Add(guidParam);

                SqlDataReader reader = command.ExecuteReader();


                if (reader.Read())
                {
                    exists = true;

                    Xcellence.Property confirmedProp = new Xcellence.Property(obj, "Confirmed");
                    confirmedProp.Values.Add(new Xcellence.Value(confirmedProp, reader.GetDateTime(3)));
                    obj.Meta.Property.Add(confirmedProp);

                    Xcellence.Property createdProp = new Xcellence.Property(obj, "Created");
                    createdProp.Values.Add(new Xcellence.Value(createdProp, reader.GetDateTime(2)));
                    obj.Meta.Property.Add(createdProp);

                    Xcellence.Property stateProp = new Xcellence.Property(obj, "State");
                    stateProp.Values.Add(new Xcellence.Value(stateProp, reader.GetDouble(4)));
                    obj.Meta.Property.Add(stateProp);

                    if (!reader.IsDBNull(1))
                    {
                        Xcellence.Property fprop = new Xcellence.Property(obj, "FriendlyName");
                        fprop.Values.Add(new Xcellence.Value(fprop, reader.GetString(1)));
                        obj.Meta.Property.Add(fprop);
                    }

                    if (!reader.IsDBNull(5))
                        pocid = reader.GetInt32(5);

                    if (!reader.IsDBNull(6))
                        pguid = reader.GetGuid(6);
                }

                reader.Close();
            }

            if (!exists)
            {
                return null;
            }

            using (SqlCommand propCommand = new SqlCommand("select [PropertyClass].[Name], [Property].[Value] from [Property], [PropertyClass] where [Property].[ObjectClassID] = @ocid and [Property].[ObjectGUID] = @guid and [PropertyClass].[ID] = [Property].[PropertyClassID] order by [PropertyClass].[Name];", conn))
            {
                SqlParameter guidParam2 = new SqlParameter("@guid", System.Data.SqlDbType.UniqueIdentifier);
                guidParam2.Value = guid;

                SqlParameter ocidParam2 = new SqlParameter("@ocid", System.Data.SqlDbType.Int);
                ocidParam2.Value = ocidParam.Value;

                propCommand.Parameters.Add(guidParam2);
                propCommand.Parameters.Add(ocidParam2);

                SqlDataReader propReader = propCommand.ExecuteReader();

                string propName = string.Empty;
                Xcellence.Property prop = new Xcellence.Property(obj, "");

                while (propReader.Read())
                {
                    string nextName = propReader.GetString(0);

                    if (nextName != propName)
                    {
                        if (propName != string.Empty)
                            obj.Signature.Property.Add(prop);

                        propName = nextName;
                        prop = new Xcellence.Property(obj, propName);
                    }

                    prop.AddValue(propReader.GetString(1));
                }

                if (propName != string.Empty)
                    obj.Signature.Property.Add(prop);

                propReader.Close();
            }

            //select children
            using (SqlCommand relCommand = new SqlCommand("select [ObjectClass].[Name], [Object].[GUID] from [Object], [ObjectClass]"
                + " where [Object].[ParentClassID] = @ocid and [Object].[ParentGUID] = @guid"
                + " and [ObjectClass].[ID] = [Object].[ObjectClassID] order by [ObjectClass].[Name];", conn))
            {
                SqlParameter guidParam2 = new SqlParameter("@guid", System.Data.SqlDbType.UniqueIdentifier);
                guidParam2.Value = guid;

                SqlParameter ocidParam2 = new SqlParameter("@ocid", System.Data.SqlDbType.Int);
                ocidParam2.Value = ocidParam.Value;

                relCommand.Parameters.Add(guidParam2);
                relCommand.Parameters.Add(ocidParam2);

                SqlDataReader relReader = relCommand.ExecuteReader();

                string relName = string.Empty;
                Xcellence.Relation rel = new Xcellence.Relation( obj );
                Xcellence.Link link = new Xcellence.Link(rel);

                while (relReader.Read())
                {
                    string nextName = relReader.GetString(0);

                    if (nextName != relName)
                    {
                        if (relName != string.Empty)
                        {
                            rel.Link.Add(link);
                            obj.Network.Add(rel);
                        }

                        relName = nextName;
                        rel = new Xcellence.Relation( obj );
                        rel.Name.Add(relName);
                        link = new Xcellence.Link(rel);
                    }

                    Xcellence.Reference r = new Xcellence.Reference(link);
                    r.GUID = relReader.GetGuid(1);

                    if (relName != string.Empty)
                        r.RelationType.Add(relName);

                    link.Reference.Add(r);
                }

                if (relName != string.Empty)
                {
                    rel.Link.Add(link);
                    obj.Network.Add(rel);
                }

                relReader.Close();
            }

            //select parent
            if (pocid != -1 && pguid != Guid.Empty)
            {
                using (SqlCommand relCommand = new SqlCommand("select [ObjectClass].[Name], [Object].[GUID] from [Object], [ObjectClass]"
                    + " where [Object].[ObjectClassID] = @ocid and [Object].[GUID] = @guid"
                    + " and [ObjectClass].[ID] = [Object].[ObjectClassID] order by [ObjectClass].[Name];", conn))
                {
                    SqlParameter guidParam2 = new SqlParameter("@guid", System.Data.SqlDbType.UniqueIdentifier);
                    guidParam2.Value = pguid;

                    SqlParameter ocidParam2 = new SqlParameter("@ocid", System.Data.SqlDbType.Int);
                    ocidParam2.Value = pocid;

                    relCommand.Parameters.Add(guidParam2);
                    relCommand.Parameters.Add(ocidParam2);

                    SqlDataReader relReader = relCommand.ExecuteReader();

                    string relName = string.Empty;
                    Xcellence.Relation rel = new Xcellence.Relation( obj );
                    Xcellence.Link link = new Xcellence.Link(rel);

                    while (relReader.Read())
                    {
                        string nextName = relReader.GetString(0);

                        if (nextName != relName)
                        {
                            if (relName != string.Empty)
                            {
                                rel.Link.Add(link);
                                obj.Network.Add(rel);
                            }

                            relName = nextName;
                            rel = new Xcellence.Relation( obj );
                            rel.Name.Add(relName);
                            link = new Xcellence.Link(rel);
                        }

                        Xcellence.Reference r = new Xcellence.Reference(link);
                        r.GUID = relReader.GetGuid(1);
                        
                        if (relName != string.Empty)
                            r.RelationType.Add(relName);

                        link.Reference.Add(r);
                    }

                    if (relName != string.Empty)
                    {
                        rel.Link.Add(link);
                        obj.Network.Add(rel);
                    }

                    relReader.Close();
                }
            }

            if (this.OnRead != null)
                this.OnRead(this, obj);

            return obj;
        }

        public Xcellence.Object GetObject(List<string> className, Guid guid)
        {
            if( className.Count == 0 )
                return null;

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);

            conn.Open();

            Xcellence.Object obj = this.GetObject(ref conn, className[0], guid);

            conn.Close();

            return obj;

        }

        public IEnumerable<Xcellence.Property> GetProperties(IEnumerable<Xcellence.PathPoint> classPredicate, bool negateClasses, IEnumerable<Xcellence.PathPoint> propertyPredicate, bool negateProperties)
        {
            if (!(classPredicate == null && negateClasses) && !(propertyPredicate == null && negateProperties))
            {
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);
                SqlConnection conn2 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);

                XElement filterElement = new XElement("Filter");

                SqlParameter anyClassParam = new SqlParameter("@AnyClass", System.Data.SqlDbType.Bit);

                if (classPredicate != null)
                {
                    anyClassParam.Value = false;

                    foreach (Xcellence.PathPoint c in classPredicate)
                    {
                        if (c.Object is string)
                        {
                            XElement cel = new XElement("ObjectClass");
                            cel.SetValue(c.Object.ToString());
                            filterElement.Add(cel);
                        }
                    }
                }
                else
                    anyClassParam.Value = true;

                SqlParameter anyPropParam = new SqlParameter("@AnyProperty", System.Data.SqlDbType.Bit);

                if (propertyPredicate != null)
                {
                    anyPropParam.Value = false;

                    foreach (Xcellence.PathPoint p in propertyPredicate)
                    {
                        if (p.Object is string)
                        {
                            XElement pel = new XElement("Property");
                            pel.SetValue(p.Object.ToString());
                            filterElement.Add(pel);
                        }
                    }
                }
                else
                    anyPropParam.Value = true;

                SqlParameter filterParam = new SqlParameter("@Filter", System.Data.SqlDbType.Xml);
                filterParam.Value = filterElement.ToString();

                SqlParameter domainParam = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
                domainParam.Value = System.Configuration.ConfigurationManager.AppSettings["Domain"];

                string sql = "select distinct [ObjectClass].[Name], "
                    + "   [Property].[ObjectGUID],"
                    + "   [PropertyClass].[Name]"
                    + " from [Property], [PropertyClass], [ObjectClass]"
                    + " where [Property].[PropertyClassID] = [PropertyClass].[ID]"
                    + "   and [Property].[ObjectClassID] = [ObjectClass].[ID]"
                    + "   and (@AnyClass = 1 or [ObjectClass].[Name] " + (negateClasses ? "not" : "") + " in (select oc.name.value('.', 'nvarchar(64)') from  @Filter.nodes('/Filter/ObjectClass') oc(name)))"
                    + "   and (@AnyProperty = 1 or [PropertyClass].[Name] " + (negateProperties ? "not" : "") + " in (select prop.name.value('.', 'nvarchar(64)') from  @Filter.nodes('/Filter/Property') prop(name)))";

                conn.Open();
                conn2.Open();

                using (SqlCommand com = new SqlCommand(sql, conn))
                {
                    com.CommandType = System.Data.CommandType.Text;
                    com.Parameters.Add(domainParam);
                    com.Parameters.Add(filterParam);
                    com.Parameters.Add(anyClassParam);
                    com.Parameters.Add(anyPropParam);

                    SqlDataReader reader = com.ExecuteReader();


                    while (reader.Read())
                    {
                        string ocName = reader.GetString(0);
                        Guid guid = reader.GetGuid(1);

                        Xcellence.Object obj = this.GetObject(ref conn2, ocName, guid);

                        string propName = reader.GetString(2);

                        if (obj != null)
                        {
                            //Console.WriteLine(obj);

                            foreach (Xcellence.Property prop in obj.Signature.Property)
                            {
                                if (prop.Name == propName)
                                {
                                    yield return prop;
                                    break;
                                }
                            }
                        }
                    }
                }

                conn2.Close();
                conn.Close();
            }
        }

        public IEnumerable<Xcellence.Property> GetPropertiesByValues(IEnumerable<Xcellence.PathPoint> classPredicate, bool negateClasses, IEnumerable<Xcellence.PathPoint> propertyPredicate, bool negateProperties, IEnumerable<Xcellence.PathPoint> valuePredicate, bool negateValues, Xcellence.Scripting.Nodes.FiltrationMethod filtration, double threshold)
        {
            if (!(classPredicate == null && negateClasses) && !(propertyPredicate == null && negateProperties) && !(valuePredicate == null && negateValues))
            {
                if (filtration == Xcellence.Scripting.Nodes.FiltrationMethod.Fuzzy)
                {
                    if (threshold == 0)
                        filtration = Xcellence.Scripting.Nodes.FiltrationMethod.None;
                    else if (threshold == 1)
                        filtration = Xcellence.Scripting.Nodes.FiltrationMethod.All;
                }

                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);
                SqlConnection conn2 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);

                XElement filterElement = new XElement("Filter");

                SqlParameter anyClassParam = new SqlParameter("@AnyClass", System.Data.SqlDbType.Bit);

                if (classPredicate != null)
                {
                    anyClassParam.Value = false;

                    foreach (Xcellence.PathPoint c in classPredicate)
                    {
                        if (c.Object is string)
                        {
                            XElement cel = new XElement("ObjectClass");
                            cel.SetValue(c.Object.ToString());
                            filterElement.Add(cel);
                        }
                    }
                }
                else
                    anyClassParam.Value = true;

                SqlParameter anyPropParam = new SqlParameter("@AnyProperty", System.Data.SqlDbType.Bit);

                if (propertyPredicate != null)
                {
                    anyPropParam.Value = false;

                    foreach (Xcellence.PathPoint p in propertyPredicate)
                    {
                        if (p.Object is string)
                        {
                            XElement pel = new XElement("Property");
                            pel.SetValue(p.Object.ToString());
                            filterElement.Add(pel);
                        }
                    }
                }
                else
                    anyPropParam.Value = true;

                SqlParameter anyValParam = new SqlParameter("@AnyValue", System.Data.SqlDbType.Bit);


                if (valuePredicate != null)
                {
                    anyValParam.Value = false;

                    foreach (Xcellence.PathPoint v in valuePredicate)
                    {
                        if (v.Object is string)
                        {
                            bool isRegEx = false;

                            if (v is System.Text.RegularExpressions.Regex)
                                isRegEx = true;

                            XElement vel = new XElement("Value");
                            vel.SetAttributeValue("IsRegEx", isRegEx);
                            vel.SetValue(v.ToString());
                            filterElement.Add(vel);
                        }
                    }
                }
                else
                {
                    anyValParam.Value = true;
                }

                SqlParameter filterParam = new SqlParameter("@Filter", System.Data.SqlDbType.Xml);
                filterParam.Value = filterElement.ToString();

                SqlParameter domainParam = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
                domainParam.Value = System.Configuration.ConfigurationManager.AppSettings["Domain"];

                string sql = "select distinct [ObjectClass].[Name], "
                    + "   [Property].[ObjectGUID],"
                    + "   [PropertyClass].[Name]"
                    + " from [Property], [PropertyClass], [ObjectClass]"
                    + " where [Property].[PropertyClassID] = [PropertyClass].[ID]"
                    + "   and [Property].[ObjectClassID] = [ObjectClass].[ID]"
                    + "   and (@AnyClass = 1 or [ObjectClass].[Name] " + (negateClasses ? "not" : "") + " in (select oc.name.value('.', 'nvarchar(64)') from  @Filter.nodes('/Filter/ObjectClass') oc(name)))"
                    + "   and (@AnyProperty = 1 or [PropertyClass].[Name] " + (negateProperties ? "not" : "") + " in (select prop.name.value('.', 'nvarchar(64)') from  @Filter.nodes('/Filter/Property') prop(name)))";

                if( filtration != Xcellence.Scripting.Nodes.FiltrationMethod.Fuzzy && !(filtration == Xcellence.Scripting.Nodes.FiltrationMethod.All && negateValues) )
                    sql += "   and (@AnyValue = 1 or " + (filtration == Xcellence.Scripting.Nodes.FiltrationMethod.None ^ negateValues? "not": "") + " exists (select * from  @Filter.nodes('/Filter/Value') val(v) where (val.v.value('@IsRegEx','bit') = 0 and " + (negateValues ? "not" : "") + " [Property].[Value] = val.v.value('.', 'nvarchar(1024)') ) or (val.v.value('@IsRegEx','bit') = 1 and dbo.RegexMatch([Property].[Value], val.v.value('.', 'nvarchar(1024)')) = 1)))";

                conn.Open();
                conn2.Open();

                using (SqlCommand com = new SqlCommand(sql, conn))
                {
                    com.CommandType = System.Data.CommandType.Text;
                    com.Parameters.Add(domainParam);
                    com.Parameters.Add(filterParam);
                    com.Parameters.Add(anyClassParam);
                    com.Parameters.Add(anyPropParam);
                    com.Parameters.Add(anyValParam);

                    SqlDataReader reader = com.ExecuteReader();


                    while (reader.Read())
                    {
                        string ocName = reader.GetString(0);
                        Guid guid = reader.GetGuid(1);


                        Xcellence.Object obj = this.GetObject(ref conn2, ocName, guid);

                        string propName = reader.GetString(2);

                        if (obj != null)
                        {
                            //Console.WriteLine(obj);

                            foreach (Xcellence.Property prop in obj.Signature.Property)
                            {
                                if (prop.Name == propName)
                                {
                                    bool meetsArgs = false;

                                    switch( filtration )
                                    {
                                        case Xcellence.Scripting.Nodes.FiltrationMethod.All:

                                            meetsArgs = true;

                                            foreach (object predicate in valuePredicate)
                                            {
                                                bool hasValue = false;

                                                foreach (Xcellence.Value val in prop.Values)
                                                {
                                                    object xval = val.Get();

                                                    if (predicate is System.Text.RegularExpressions.Regex)
                                                    {
                                                        System.Text.RegularExpressions.Regex reg = (System.Text.RegularExpressions.Regex)predicate;

                                                        if (reg.IsMatch(xval.ToString()))
                                                        {
                                                            hasValue = true;
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (xval.Equals(predicate))
                                                        {
                                                            hasValue = true;
                                                            break;
                                                        }
                                                    }
                                                }

                                                if (!hasValue)
                                                {
                                                    meetsArgs = false;
                                                    break;
                                                }
                                            }

                                            meetsArgs = meetsArgs ^ negateValues;

                                            break;

                                        case Xcellence.Scripting.Nodes.FiltrationMethod.Fuzzy:

                                            if (valuePredicate != null)
                                            {
                                                meetsArgs = false;

                                                double total = 0;
                                                double pass = 0;

                                                foreach( object predicate in valuePredicate )
                                                {
                                                    foreach (Xcellence.Value val in prop.Values)
                                                    {
                                                        object xval = val.Get();

                                                        if (predicate is System.Text.RegularExpressions.Regex)
                                                        {
                                                            System.Text.RegularExpressions.Regex reg = (System.Text.RegularExpressions.Regex)predicate;

                                                            if (reg.IsMatch(xval.ToString()))
                                                            {
                                                                pass++;
                                                                break;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (xval.Equals(predicate))
                                                            {
                                                                pass++;
                                                                break;
                                                            }
                                                        }
                                                    }

                                                    total++;
                                                }

                                                if (total > 0)
                                                {
                                                    double ratio = pass / total;

                                                    if (ratio >= threshold)
                                                        meetsArgs = true;
                                                }
                                                else
                                                    meetsArgs = true;

                                            }
                                            else
                                                meetsArgs = true;

                                            meetsArgs = meetsArgs ^ negateValues;
                                            break;

                                        default:
                                            meetsArgs = true;
                                            break;

                                    }

                                    if( meetsArgs )
                                        yield return prop;

                                    break;
                                }
                            }
                        }
                    }
                }

                conn2.Close();
                conn.Close();
            }
        }


        public IEnumerable<Xcellence.Value> GetValues(IEnumerable<Xcellence.PathPoint> classPredicate, bool negateClasses, IEnumerable<Xcellence.PathPoint> propertyPredicate, bool negateProperties, IEnumerable<Xcellence.PathPoint> valuePredicate, bool negateValues)
        {
            //The first portion is identical to GetPropertiesByValues
            if (!(classPredicate == null && negateClasses) && !(propertyPredicate == null && negateProperties) && !(valuePredicate == null && negateValues))
            {
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);
                SqlConnection conn2 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);

                XElement filterElement = new XElement("Filter");

                SqlParameter anyClassParam = new SqlParameter("@AnyClass", System.Data.SqlDbType.Bit);

                if (classPredicate != null)
                {
                    anyClassParam.Value = false;

                    foreach (Xcellence.PathPoint c in classPredicate)
                    {
                        if (c.Object is string)
                        {
                            XElement cel = new XElement("ObjectClass");
                            cel.SetValue(c.Object.ToString());
                            filterElement.Add(cel);
                        }
                    }
                }
                else
                    anyClassParam.Value = true;

                SqlParameter anyPropParam = new SqlParameter("@AnyProperty", System.Data.SqlDbType.Bit);

                if (propertyPredicate != null)
                {
                    anyPropParam.Value = false;

                    foreach (Xcellence.PathPoint p in propertyPredicate)
                    {
                        if (p.Object is string)
                        {
                            XElement pel = new XElement("Property");
                            pel.SetValue(p.Object.ToString());
                            filterElement.Add(pel);
                        }
                    }
                }
                else
                    anyPropParam.Value = true;

                SqlParameter anyValParam = new SqlParameter("@AnyValue", System.Data.SqlDbType.Bit);


                if (valuePredicate != null)
                {
                    anyValParam.Value = false;

                    foreach (Xcellence.PathPoint v in valuePredicate)
                    {
                        if (v.Object is string)
                        {
                            bool isRegEx = false;

                            if (v is System.Text.RegularExpressions.Regex)
                                isRegEx = true;

                            XElement vel = new XElement("Value");
                            vel.SetAttributeValue("IsRegEx", isRegEx);
                            vel.SetValue(v.Object.ToString());
                            filterElement.Add(vel);
                        }
                    }
                }
                else
                {
                    anyValParam.Value = true;
                }

                SqlParameter filterParam = new SqlParameter("@Filter", System.Data.SqlDbType.Xml);
                filterParam.Value = filterElement.ToString();

                SqlParameter domainParam = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
                domainParam.Value = System.Configuration.ConfigurationManager.AppSettings["Domain"];

                string sql = "select [ObjectClass].[Name], "
                    + "   [Property].[ObjectGUID],"
                    + "   [PropertyClass].[Name],"
                    + "   [Property].[Value]"
                    + " from [Property], [PropertyClass], [ObjectClass]"
                    + " where [Property].[PropertyClassID] = [PropertyClass].[ID]"
                    + "   and [Property].[ObjectClassID] = [ObjectClass].[ID]"
                    + "   and (@AnyClass = 1 or [ObjectClass].[Name] " + (negateClasses ? "not" : "") + " in (select oc.name.value('.', 'nvarchar(64)') from  @Filter.nodes('/Filter/ObjectClass') oc(name)))"
                    + "   and (@AnyProperty = 1 or [PropertyClass].[Name] " + (negateProperties ? "not" : "") + " in (select prop.name.value('.', 'nvarchar(64)') from  @Filter.nodes('/Filter/Property') prop(name)))"
                    + "   and (@AnyValue = 1 or exists (select * from  @Filter.nodes('/Filter/Value') val(v) where (val.v.value('@IsRegEx','bit') = 0 and " + (negateValues ? "not" : "") + " [Property].[Value] = val.v.value('.', 'nvarchar(1024)') ) or (val.v.value('@IsRegEx','bit') = 1 and dbo.RegexMatch([Property].[Value], val.v.value('.', 'nvarchar(1024)')) = " + Convert.ToByte((true ^ negateValues)).ToString() + ")))";

                conn.Open();
                conn2.Open();

                using (SqlCommand com = new SqlCommand(sql, conn))
                {
                    com.CommandType = System.Data.CommandType.Text;
                    com.Parameters.Add(domainParam);
                    com.Parameters.Add(filterParam);
                    com.Parameters.Add(anyClassParam);
                    com.Parameters.Add(anyPropParam);
                    com.Parameters.Add(anyValParam);

                    SqlDataReader reader = com.ExecuteReader();


                    while (reader.Read())
                    {
                        string ocName = reader.GetString(0);
                        Guid guid = reader.GetGuid(1);

                        Xcellence.Object obj = this.GetObject(ref conn2, ocName, guid);

                        string propName = reader.GetString(2);
                        string val = reader.GetString(3);


                        if (obj != null)
                        {
                            //Console.WriteLine(obj);

                            foreach (Xcellence.Property prop in obj.Signature.Property)
                            {
                                if (prop.Name == propName)
                                {
                                    foreach (Xcellence.Value xval in prop.Values)
                                    {
                                        if (xval.Get().Equals(val))
                                        {
                                            yield return xval;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }

                conn2.Close();
                conn.Close();
            }
        }
    
        public void Categorize(Xcellence.Object obj, string category)
        {
            if (this.OnCategorize != null)
                this.OnCategorize(this, obj, category);

            category = category.Trim().Replace(". ", ".").Replace(" .", ".");
            category = category.Trim('.');

            if (category == string.Empty)
            {
                Console.WriteLine("Failed to categorize into " + category);
                return;
            }
            
            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex("\\.");

            System.Text.RegularExpressions.Match match = re.Match(category);

            string catalog = string.Empty;

            if (match.Success)
            {
                catalog = category.Substring(0, match.Index).Trim();
            }
            else
            {
                catalog = category;
            }
            
            if (isTest)
            {
                while (match.Success)
                {
                    string cat = category.Substring(0, match.Index);
                    cat = cat.Trim().Replace(". ", ".").Replace(" .", ".");

                    if (cat != string.Empty)
                    {
                        //Console.WriteLine("Categorizing into " + cat);
                        match = match.NextMatch();
                    }
                }
                
                //Console.WriteLine("Categorizing into " + category);
                //Console.WriteLine(obj);

                return;
            }            

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Cube"].ConnectionString);

            SqlParameter domainParam = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
            domainParam.Value = System.Configuration.ConfigurationManager.AppSettings["Domain"];

            //SqlParameter catalogParam = new SqlParameter("@ObjectGUID", System.Data.SqlDbType.NVarChar, 64);
            //catalogParam.Value = catalog;

            SqlParameter ocParam = new SqlParameter("@ObjectClass", System.Data.SqlDbType.NVarChar, 64);

            if (obj.Class.Count > 0)
                ocParam.Value = obj.Class[0];
            else
                ocParam.Value = "Unknown";

            SqlParameter guidParam = new SqlParameter("@ObjectGUID", System.Data.SqlDbType.UniqueIdentifier);
            guidParam.Value = obj.GUID;

            SqlParameter fnameParam = new SqlParameter("@FriendlyName", System.Data.SqlDbType.NVarChar, 256);

            Xcellence.Property fnameProp  = obj.Meta.GetProperty("FriendlyName");

            if( fnameProp != null && fnameProp.Values.Count > 0)
                fnameParam.Value = fnameProp.Values[0].ToString();
            else
                fnameParam.Value = DBNull.Value;

            SqlParameter pathParam = new SqlParameter("@CategoryPath", System.Data.SqlDbType.NVarChar, 256);

            string sql = "CategoryEntry_Add";

            conn.Open();

            using (SqlCommand com = new SqlCommand( sql, conn ) )
            {
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.Parameters.Add(domainParam);
                com.Parameters.Add(ocParam);
                com.Parameters.Add(guidParam);
                com.Parameters.Add(fnameParam);
                com.Parameters.Add(pathParam);
                //com.Parameters.Add(catalogParam);

                while (match.Success)
                {
                    string cat = category.Substring(0, match.Index);
                    cat = cat.Trim().Replace(". ", ".").Replace(" .", ".");

                    if (cat != string.Empty)
                    {
                        Console.WriteLine( this.processIdentifier + ": Categorizing into " + cat);
                        pathParam.Value = cat;
                        com.ExecuteNonQuery();

                        match = match.NextMatch();
                    }
                }

                Console.WriteLine(this.processIdentifier + ": Categorizing into " + category);
                pathParam.Value = category;

                com.ExecuteNonQuery();

                //This should be replace with OnCategorize
                if (this.OnWrite != null)
                    this.OnWrite(this, obj);

                //Console.WriteLine(obj);
                /*
                for (int i = 0; i < cats.Length; i++)
                {
                    pathParam.Value = cats[i];
                    com.ExecuteNonQuery();
                }
                 * */
            }

            conn.Close();

            return;
        }

        public void Relate(Xcellence.Object obj, Xcellence.Object pred, IEnumerable<string> relationType)
        {
            Console.WriteLine(this.processIdentifier + ": Relating " + obj.GUID + " to " + pred.GUID);
            this.SetDependency(obj, pred, 1, relationType);
        }

        public void SetWeight(Xcellence.Weight weight)
        {
            Console.WriteLine(this.processIdentifier + ": Setting weight to " + weight.NewValue + " (" + weight.Reference.Subject.GUID + " > " + weight.Reference.Predicate.GUID + ")" );
            this.SetDependency(weight.Reference.Subject, weight.Reference.Predicate, weight.NewValue, weight.Reference.RelationType.Value);
        }

        private void SetDependency(Xcellence.Object obj, Xcellence.Object pred, double weight, IEnumerable<string> relationType){
            if (!isTest)
            {
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Network"].ConnectionString);

                SqlParameter domainParam = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
                domainParam.Value = System.Configuration.ConfigurationManager.AppSettings["Domain"];

                SqlParameter scParam = new SqlParameter("@SubjectClass", System.Data.SqlDbType.NVarChar, 64);
                SqlParameter pcParam = new SqlParameter("@PredicateClass", System.Data.SqlDbType.NVarChar, 64);

                if (obj.Class.Count > 0)
                    scParam.Value = obj.Class[0];
                else
                    scParam.Value = "Unknown";

                if (pred.Class.Count > 0)
                    pcParam.Value = pred.Class[0];
                else
                    pcParam.Value = "Unknown";

                SqlParameter sguidParam = new SqlParameter("@SubjectGUID", System.Data.SqlDbType.UniqueIdentifier);
                sguidParam.Value = obj.GUID;

                SqlParameter pguidParam = new SqlParameter("@PredicateGUID", System.Data.SqlDbType.UniqueIdentifier);
                pguidParam.Value = pred.GUID;

                SqlParameter typeParam = new SqlParameter("@DependencyType", System.Data.SqlDbType.NVarChar, 64);

                if (relationType.Count() > 0)
                    typeParam.Value = relationType.First();
                else
                    typeParam.Value = "Generic";

                SqlParameter weightParam = new SqlParameter("@Weight", System.Data.SqlDbType.Float);
                weightParam.Value = weight;

                string sql = "Dependency_Set";

                conn.Open();

                using (SqlCommand com = new SqlCommand(sql, conn))
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.Parameters.Add(domainParam);
                    com.Parameters.Add(scParam);
                    com.Parameters.Add(pcParam);
                    com.Parameters.Add(sguidParam);
                    com.Parameters.Add(pguidParam);
                    com.Parameters.Add(typeParam);
                    com.Parameters.Add(weightParam);

                    com.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        public void Strengthen(Xcellence.Object obj, double amount, string callpoint)
        {
            //Almost identical to Weaken, except strengthening occurs only if there has been a previous weakening by the same amount.

            if (obj.Class.Count == 0)
                return;

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Network"].ConnectionString);
            SqlConnection conn2 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);

            //Not implemented yet.

            foreach (Xcellence.Property prop in obj.Meta.Property)
            {
                if (prop.Name == "State")
                {
                    double cval = (double)prop.Values[0].Get();

                    //---------------------

                    SqlParameter domainParam = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
                    domainParam.Value = System.Configuration.ConfigurationManager.AppSettings["Domain"];

                    SqlParameter ocParam = new SqlParameter("@ObjectClass", System.Data.SqlDbType.NVarChar, 64);
                    ocParam.Value = obj.Class[0];

                    SqlParameter guidParam = new SqlParameter("@ObjectGUID", System.Data.SqlDbType.UniqueIdentifier);
                    guidParam.Value = obj.GUID;

                    SqlParameter deltaParam = new SqlParameter("@Delta", System.Data.SqlDbType.Float);
                    //deltaParam.Direction = System.Data.ParameterDirection.Output;
                    deltaParam.Value = amount;

                    //-----------------

                    SqlParameter domainParam2 = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
                    domainParam2.Value = System.Configuration.ConfigurationManager.AppSettings["Domain"];

                    SqlParameter ocParam2 = new SqlParameter("@ObjectClass", System.Data.SqlDbType.NVarChar, 64);

                    SqlParameter guidParam2 = new SqlParameter("@ObjectGUID", System.Data.SqlDbType.UniqueIdentifier);

                    SqlParameter deltaParam2 = new SqlParameter("@Delta", System.Data.SqlDbType.Float);

                    SqlParameter ocidParam2 = new SqlParameter("@ocid", System.Data.SqlDbType.Int);
                    ocidParam2.Direction = System.Data.ParameterDirection.Output;

                    conn.Open();
                    conn2.Open();

                    string sql = "PropagateChange";

                    string sql2 = "set @ocid = (select [ID] from [ObjectClass] where [Name] = @ObjectClass and [DomainID] = (select [ID] from [Domain] where [Name] = @Domain));"
                    + "update Object set State = case when State + ( State * @Delta ) < 1 then State + ( State * @Delta ) else 1 end "
                    + "where ObjectClassID = @ocid and [GUID] = @ObjectGUID"
                    + " and exists (select * from [StateChange] where Delta = @Delta and ObjectClassID = @ocid and ObjectGUID = @ObjectGUID);";
                   
                    // Removed deletion of StatChange because new algorithm calls for natural healing.

                    //+ " delete top(1) from [StateChange] where [ObjectClassID] = @ocid and [ObjectGUID] = @ObjectGUID and [Delta] = @Delta;";
                    //+ "select State from Object where ObjectClassID = @ocid and GUID = @ObjectGUID";

                    using (SqlCommand com = new SqlCommand(sql, conn))
                    {
                        com.CommandType = System.Data.CommandType.StoredProcedure;
                        com.Parameters.Add(domainParam);
                        com.Parameters.Add(ocParam);
                        com.Parameters.Add(guidParam);
                        com.Parameters.Add(deltaParam);

                        SqlDataReader reader = com.ExecuteReader();

                        using (SqlCommand com2 = new SqlCommand(sql2, conn2))
                        {
                            com2.Parameters.Add(domainParam2);
                            com2.Parameters.Add(ocParam2);
                            com2.Parameters.Add(guidParam2);
                            com2.Parameters.Add(ocidParam2);
                            com2.Parameters.Add(deltaParam2);

                            while (reader.Read())
                            {
                                ocParam2.Value = reader[1].ToString();
                                guidParam2.Value = (Guid)reader[2];
                                deltaParam2.Value = (double)reader[3];
                                
                                com2.ExecuteNonQuery();

                                Console.WriteLine("Updated state of " + reader[1].ToString() + " " + reader[2].ToString());                                   


                            }
                        }
                    }

                    conn.Close();
                    conn2.Close();

                    break;
                }            
            }

            return;
        }

        public void Weaken(Xcellence.Object obj, double amount, string callpoint)
        {
            //Weaken and strengthen can be joined into one by using negative delta and checking for both proximity to 1 and 0.

            //Almost identical to Strengthen, except weakening occurs only once for any given callpoint.

            if (obj.Class.Count == 0)
                return;

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Network"].ConnectionString);
            SqlConnection conn2 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MIX2.Registry"].ConnectionString);

            //Not implemented yet.

            foreach (Xcellence.Property prop in obj.Meta.Property)
            {
                if (prop.Name == "State")
                {
                    double cval = (double)prop.Values[0].Get();

                    //---------------------

                    SqlParameter domainParam = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
                    domainParam.Value = System.Configuration.ConfigurationManager.AppSettings["Domain"];

                    SqlParameter ocParam = new SqlParameter("@ObjectClass", System.Data.SqlDbType.NVarChar, 64);
                    ocParam.Value = obj.Class[0];

                    SqlParameter guidParam = new SqlParameter("@ObjectGUID", System.Data.SqlDbType.UniqueIdentifier);
                    guidParam.Value = obj.GUID;

                    SqlParameter deltaParam = new SqlParameter("@Delta", System.Data.SqlDbType.Float);
                    //deltaParam.Direction = System.Data.ParameterDirection.Output;
                    deltaParam.Value = amount;

                    //-----------------

                    SqlParameter callpointParam2 = new SqlParameter("@Callpoint", System.Data.SqlDbType.NVarChar, 256);
                    callpointParam2.Value = callpoint;

                    SqlParameter domainParam2 = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
                    domainParam2.Value = System.Configuration.ConfigurationManager.AppSettings["Domain"];

                    SqlParameter ocParam2 = new SqlParameter("@ObjectClass", System.Data.SqlDbType.NVarChar, 64);

                    SqlParameter guidParam2 = new SqlParameter("@ObjectGUID", System.Data.SqlDbType.UniqueIdentifier);

                    SqlParameter deltaParam2 = new SqlParameter("@Delta", System.Data.SqlDbType.Float);

                    SqlParameter ocidParam2 = new SqlParameter("@ocid", System.Data.SqlDbType.Int);
                    ocidParam2.Direction = System.Data.ParameterDirection.Output;

                    conn.Open();
                    conn2.Open();

                    string sql = "PropagateChange";

                    string sql2 = "set @ocid = (select [ID] from [ObjectClass] where [Name] = @ObjectClass and [DomainID] = (select [ID] from [Domain] where [Name] = @Domain));"
                    + "update Object set State = case when State - ( State * @Delta ) > 0 then State - ( State * @Delta ) else 0 end "
                    + "where ObjectClassID = @ocid and [GUID] = @ObjectGUID"
                    + " and not exists (select * from [StateChange] where Callpoint = @Callpoint and ObjectClassID = @ocid and ObjectGUID = @ObjectGUID);"

                    + " merge [StateChange]"
                    + " using ( select @ocid ocid, @ObjectGUID guid, @Callpoint cpoint) T"
                    + " on [ObjectClassID] = T.ocid and [ObjectGUID] = T.guid and [Callpoint] = T.cpoint"
                    + " when matched then"
                    + "   update set LastCall = getutcdate(), Delta = -@Delta"
                    + " when not matched then"
                    + "   insert ([ObjectClassID], [ObjectGUID], [Callpoint], [Delta], [LastCall]) values (@ocid, @ObjectGUID, @Callpoint, -@Delta, getutcdate());";

                    //+ "select State from Object where ObjectClassID = @ocid and GUID = @ObjectGUID";

                    using (SqlCommand com = new SqlCommand(sql, conn))
                    {
                        com.CommandType = System.Data.CommandType.StoredProcedure;
                        com.Parameters.Add(domainParam);
                        com.Parameters.Add(ocParam);
                        com.Parameters.Add(guidParam);
                        com.Parameters.Add(deltaParam);

                        SqlDataReader reader = com.ExecuteReader();

                        using (SqlCommand com2 = new SqlCommand(sql2, conn2))
                        {
                            com2.Parameters.Add(domainParam2);
                            com2.Parameters.Add(ocParam2);
                            com2.Parameters.Add(guidParam2);
                            com2.Parameters.Add(ocidParam2);
                            com2.Parameters.Add(callpointParam2);
                            com2.Parameters.Add(deltaParam2);

                            while (reader.Read())
                            {
                                ocParam2.Value = reader[1].ToString();
                                guidParam2.Value = (Guid)reader[2];
                                deltaParam2.Value = (double)reader[3];

                                com2.ExecuteNonQuery();

                                Console.WriteLine("Updated state of " + reader[1].ToString() + " " + reader[2].ToString());


                            }
                        }
                    }

                    conn.Close();
                    conn2.Close();

                    break;
                }
            }

            return;
        }
    }
}
