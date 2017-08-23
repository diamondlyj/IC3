using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace MIX2.Acquisition.IBM.BigFix
{
    public class Factory: IMIXFactory
    {
        string sourceName;
        DataSourceSchema.ObjectClass currentClass;
        MIX2.Acquisition.MapSchema map;
        WebConnection connection;

        //Locus locus;
        bool hasParent = false;
        bool autoGenParent = false;
        Locus locus = Locus.Object;
        bool isPrimitive = false;

        bool extractsProperties = false;
        string[] extractionProps;

        string[][] locusObjects;
        string[][] nonlocusObjects;
        string nonLocusQuery;

        List<PropertyData> properties;

        int locusPosition = 0;
        int locusCount = 0;
        int chunkSize = -1;
        int chunkCount = 0;
        string firstID;
        string lastID;

        bool eod = true;

        public Factory(string source, object connection)
        {
            this.connection = (WebConnection)connection;
            this.connection.RequestMethod = "GET";
            this.sourceName = source;

        }

        public bool IsAlive(out string mes)
        {
            mes = "OK";

            try
            {
                System.IO.Stream stream = this.connection.GetRESTData("login");
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);

                mes = reader.ReadToEnd();

                //mes = this.sourceName + " contains no computer data.";
            }
            catch(Exception exc) 
            {
                mes = exc.Message;
            }

            if (mes.ToLower() == "ok")
                return true;

            return false;
        }

        public void Open(DataSourceSchema.ObjectClass objectClass)
        {
            
            this.currentClass = objectClass;

            this.map = new MapSchema(objectClass.Map);

            if (this.map.Parent != null)
            {
                this.hasParent = true;

                if( this.map.Parent.Query == null || this.map.Parent.Query == string.Empty )
                {
                    this.autoGenParent = true;
                }
                else
                {
                    if (this.map.Parent.Query.Contains("[$First]"))
                    {
                        this.nonLocusQuery = this.map.Parent.Query;

                        if(this.map.Query.Contains("[$First]"))
                        {
                            throw new Exception("The mapping indicates that both parent and object are mapping as if neither were the locus.");
                        }
                    }
                    else
                    {
                        this.locus = Locus.Parent;
                    }
                }
            }

            if (this.map.Query.Contains("[$First]"))
            {
                this.nonLocusQuery = this.map.Query;

                if (this.map.Parent.Query.Contains("[$First]"))
                {
                    throw new Exception("The mapping indicates that both parent and object are mapping as if neither were the locus.");
                }
            }

                //Console.WriteLine(this.locus);

            this.locusPosition = 0;
            this.chunkSize = objectClass.ChunkSize;
            this.chunkCount = 0;

            Console.Write("Bypassing SSL check...");

            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            Console.Write("Getting objects...");

            string query = "query?relevance=";

            if(this.locus == Locus.Object )
                query += System.Web.HttpUtility.UrlEncode(map.Query);
            else
                query += System.Web.HttpUtility.UrlEncode(map.Parent.Query);

            System.IO.Stream stream = this.connection.GetRESTData(query);
            XDocument doc = System.Xml.Linq.XDocument.Load(stream);
            stream.Close();

            XElement result = doc.Root.Element("Query").Element("Result"); ;

            //Check if data returned by BigFix is in the form of tuples. 

            IEnumerable<XElement> elements = null; 
                 
            if (result.Element("Tuple") == null)
            {
                this.isPrimitive = true;
                elements = result.Elements("Answer");
            }
            else
            {
                //throw new Exception("The relevance query for the locus must return a primitive answer, not a tuple. For example (id of it)");
                this.isPrimitive = false;
                elements = result.Elements("Tuple");
            }

            this.locusCount = elements.Count();
            int n = 0;

            if (this.locusCount > 0)
            {
                Console.WriteLine("Retrieved " + this.locusCount + " locus objects.");
                this.locusObjects = new string[this.locusCount][];

                foreach (XElement element in elements)
                {

                    if (this.isPrimitive)
                    {
                        this.locusObjects[n] = new string[] { element.Value };
                    }
                    else
                    {
                        //Console.WriteLine("Locus object is a tuple.");

                        //Watch out for illegal characters. (chekc must be added)
                        IEnumerable<XElement> answers = element.Descendants("Answer");
                        int acount = answers.Count();

                        string[] vals = new string[acount];
                        int m = 0;

                        foreach (XElement answer in answers)
                        {
                            vals[m] = answer.Value;
                            m++;
                        }

                        this.locusObjects[n] = vals;
                    }

                    n++;
                }

                this.eod = false;
            }
            else
            {
                Console.WriteLine("Relevance query returned no objects.");
                this.eod = true;
            }


            //Create a data container for each property.

            this.properties = new List<PropertyData>();

            foreach (MapSchema.Property prop in this.map.Properties)
            {
                Console.WriteLine(prop.Name);

                PropertyData propData = new PropertyData
                {
                    Name = prop.Name,
                    Query = prop.Query
                };
                      
                this.properties.Add(propData);
            }

            //this.eod = true;

            if( !String.IsNullOrEmpty(this.map.ExtractionProperties ))
            {
                //Console.WriteLine("Will map " + this.map.ExtractionProperties);

                this.extractsProperties = true;

                this.extractionProps = this.map.ExtractionProperties.Split(',');

                for (int i = 0; i < this.extractionProps.Length; i++)
                {
                    this.extractionProps[i] = this.extractionProps[i].Trim();

                    if (this.extractionProps[i] == string.Empty)
                        throw new Exception("The name of properties to extract cannot be empty.");
                }

                
            }

            if (!this.eod && this.chunkSize > 0)
            {
                GetChunk();
            }

            return;
        }

        public void Close()
        {
            return;
        }

        public bool EOD
        {
            get
            {
                return this.eod;
            }
        }

        public MIX2.Data.LocalObject GetNext()
        {
            Data.LocalObject obj = new Data.LocalObject( this.sourceName, this.currentClass.Domain, this.currentClass.Name);
            obj.ObjectClass = this.currentClass.Name;

            string id = this.locusObjects[this.locusPosition][0];
            //Console.WriteLine(id);

            obj.LocalID = id;
            
            if( this.autoGenParent )
            {
                obj.ParentID = id;
                obj.ParentClass = this.map.Parent.ObjectClass;
            }

            //Check if the properties are extracted from the locus.
            if (this.extractsProperties && !this.isPrimitive)
            {
                for( int i=0;i<this.extractionProps.Length;i++ )
                {
                    string name = this.extractionProps[i];

                    Data.LocalObject.Property objProp = obj.Properties.FirstOrDefault(p => p.Name == name);

                    bool isNew = false;

                    if (objProp == null)
                    {
                        objProp = new Data.LocalObject.Property(name);
                        isNew = true;
                    }

                    string propValue = this.locusObjects[this.locusPosition][i+1];

                    if (!String.IsNullOrEmpty(propValue))
                        objProp.Values.Add(propValue);

                    //If there is at least one value for the property, then it gets appended to the object

                    if (isNew && objProp.Values.Count > 0)
                    {
                        obj.Properties.Add(objProp);
                    }

                }
            }
            else
            {

                foreach (PropertyData propData in this.properties)
                {
                    string name = propData.Name;

                    // Check if the property was already added to the object, otherwise add it.

                    Data.LocalObject.Property objProp = obj.Properties.FirstOrDefault(p => p.Name == name);

                    bool isNew = false;

                    if (objProp == null)
                    {
                        objProp = new Data.LocalObject.Property(name);
                        isNew = true;
                    }

                    string propObjectID = string.Empty;
                    string propValue = string.Empty;

                    // We can't assume the data was returned in an orderly fashion, so we go through the entire data. We treat the results of the query as a messy bag, even if this adds overhead.

                    for (int i = 0; i < propData.Data.Length; i++)
                    {
                        if (propData.Data[i][0] == id)
                        {
                            objProp.Values.Add(propData.Data[i][1]);
                        }
                    }


                    //If there is at least one value for the property, then it gets appended to the object


                    if (isNew && objProp.Values.Count > 0)
                    {
                        obj.Properties.Add(objProp);
                    }

                }
            }

            this.locusPosition++;
            this.chunkCount++;

            if(this.locusPosition == this.locusCount)
            {
                this.eod = true;
            }
            else if( this.chunkSize > 0 && this.chunkCount == this.chunkSize)
            {
                chunkCount = 0;
                GetChunk();
            }



            return obj;
        }

        private void GetChunk()
        {
            //First we need to figure ou which will be the last id for which we will retrieve data.

            int p = this.locusPosition + this.chunkSize;

            this.firstID = this.locusObjects[this.locusPosition][0];

            if ( p < this.locusCount )
            {
                //Console.WriteLine(this.locusObjects.Length + ":" + p);

                this.lastID = this.locusObjects[p][0];
            }
            else
            {
                this.lastID = this.locusObjects[this.locusCount-1][0];
            }

            //If there is a non locus object query, get the non locus objects.

            if( !String.IsNullOrEmpty(this.nonLocusQuery))
            {
                Console.Write("Getting non locus objects...");

                string query = ReplaceVariables(this.nonLocusQuery);
                query = "query?relevance=" + System.Web.HttpUtility.UrlEncode(query);


                Console.WriteLine(query);

                System.IO.Stream stream = this.connection.GetRESTData(query);
                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                stream.CopyTo(memStream);
                stream.Close();

                byte[] arr = memStream.ToArray();

                ReplaceInvalidCharacters(ref arr);

                System.IO.MemoryStream cleanStream = new System.IO.MemoryStream(arr);

                this.nonlocusObjects = BufferData(cleanStream);

                memStream.Close();

                Console.WriteLine("retrieved " + this.nonlocusObjects.Length + " objects.");
            }

            //Now we can retrieve the property data and place it in the a container,

            foreach (PropertyData propData in this.properties)
            {
                Console.Write("Gettting " + propData.Name +" data for ids "  + this.firstID + " to " + this.lastID + "...");

                int count = 0;

                if (!String.IsNullOrEmpty(propData.Query))
                {
                    string query = ReplaceVariables(propData.Query);

                    query = "query?relevance=" + System.Web.HttpUtility.UrlEncode(query);
                    
                    System.IO.Stream stream = this.connection.GetRESTData(query);
                    System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                    stream.CopyTo(memStream);
                    stream.Close();

                    byte[] arr = memStream.ToArray();

                    ReplaceInvalidCharacters(ref arr);
                    
                    System.IO.MemoryStream cleanStream = new System.IO.MemoryStream(arr);

                    count = propData.SetData(cleanStream);
                    
                    memStream.Close();
                }

                Console.WriteLine("retrieved " + count + " values.");
            }

        }


        private class NonLocusObject
        {
            public string LocusID;
            public string ID;

            public NonLocusObject( string id, string locusId)
            {
                this.ID = id;
                this.LocusID = locusId;
            }
        }

        private enum Locus
        {
            Object = 0,
            Parent = 1
        }

        private class PropertyData
        {
            public string Name;
            public string Query;
            public string[][] Data;

            public int SetData(System.IO.MemoryStream stream)
            {
                this.Data = BufferData(stream);

                return this.Data.Length;
            }

            public int SetData( XDocument doc)
            {
                XElement result = doc.Root.Element("Query").Element("Result");

                // The results should always be a tuple becuase otherwise we couldnt map the property values to an object
                // If it's not a tuple, the mapping is wrong.

                //if (result.Element("Tuple") == null)
                //    throw new Exception("Properties selected from BigFix must be in the Relevance format (id of it, {x} of it).");

                int count = result.Elements("Tuple").Count();

                this.Data = new string[count][];

                int n = 0;

                foreach (XElement element in result.Elements("Tuple") )
                {
                    this.Data[n] = new string[]{ element.Element("Answer").Value, element.Elements("Answer").ElementAt(1).Value};

                    //Console.WriteLine(this.Data[n, 0] + ":" + this.Data[n, 1]);
                    n++;
                }


                return n; 
                //Console.WriteLine(this.Xml.ToString());
            }
        }

        private string ReplaceVariables(string str)
        {
            str = str.Replace("[$LocalID]", this.firstID);
            str = str.Replace("[$First]", this.firstID);

            str = str.Replace("[$Last]", this.lastID);

            return str;
        }

        private static void  ReplaceInvalidCharacters(ref byte[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == 0x1B)
                {
                    Console.WriteLine("\nAltering character " + Convert.ToChar(arr[i]) + " to empty space.");
                    arr[i] = 0x20;
                }
            }
        }

        private static string[][] BufferData(System.IO.MemoryStream stream)
        {
            stream.Position = 0;

            System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
            settings.CheckCharacters = false;

            System.Xml.XmlReader reader = System.Xml.XmlTextReader.Create(stream, settings);
            int count = 0;

            while (reader.Read())
            {
                if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.Name == "Tuple")
                {
                    count++;
                }
            }

            stream.Position = 0;

            string[][] buffer = new string[count][];

            int n = 0;

            reader = System.Xml.XmlTextReader.Create(stream, settings);

            while (reader.Read())
            {
                if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.Name == "Tuple")
                {
                    //Console.WriteLine(reader.ReadOuterXml());

                    if (reader.ReadToDescendant("Answer"))
                    {
                        string id = reader.ReadElementContentAsString();

                        //Console.WriteLine(id);

                        if (reader.ReadToNextSibling("Answer"))
                        {
                            string val = reader.ReadElementContentAsString();

                            buffer[n] = new string[] { id, val };

                        }
                    }

                    n++;
                }
            }

            return buffer;
        }
    }
}
