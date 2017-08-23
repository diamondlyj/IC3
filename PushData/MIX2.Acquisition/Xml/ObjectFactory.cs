using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

namespace MIX2.Acquisition.Xml
{
    public class ObjectFactory: IMIXFactory
    {
        private int position;
        private bool eod;
        private string filePath;
        private System.Xml.Linq.XDocument source;
        private string sourceName;
        private IEnumerable<System.Xml.Linq.XElement> nodes;
        private IEnumerator<System.Xml.Linq.XElement> nodeIterator;
        private MIX2.Acquisition.DataSourceSchema.ObjectClass objClass;
        private MapSchema mapSchema;
        //private System.Xml.IXmlNamespaceResolver nsResolver;
        private System.Xml.XmlNamespaceManager nsManager;
        private object connection;
                
        public ObjectFactory( string sourceName, string filePath )
        {
            this.position = -1;
            this.filePath = filePath;           
            this.sourceName = sourceName;
            this.eod = true;
        }                

        public ObjectFactory( string sourceName, object Connection )
        {
            this.position = -1;
            this.sourceName = sourceName;
            this.connection = Connection;
            this.eod = true;
        }

        public bool IsAlive(out string message)
        {
            message = "OK";
            return true;
        }


        private void LoadFromFile( string filePath )
        {
            try
            {
                System.Xml.XmlReader reader = System.Xml.XmlReader.Create(filePath);
                SetSource(reader);
            }
            catch(System.IO.FileNotFoundException exc)
            {
                throw new Exception("The file " + exc.FileName + " could not be found.");
            }
        }

        private void LoadFromStream(System.IO.Stream stream)
        {
            System.Xml.XmlReader reader = System.Xml.XmlReader.Create( stream );
            SetSource(reader);
        }

        private void SetSource( System.Xml.XmlReader reader )
        {
            this.source = System.Xml.Linq.XDocument.Load(reader);
            this.nsManager = new System.Xml.XmlNamespaceManager(reader.NameTable);            
        }
            
        public void Open( MIX2.Acquisition.DataSourceSchema.ObjectClass objectClass )
        {
            if( !string.IsNullOrEmpty( this.filePath ) || ( this.connection != null && this.connection.GetType().FullName == "MIX2.Acquisition.FileConnection" ) )
            {
                string path = string.Empty;

                if (!string.IsNullOrEmpty(this.filePath))
                    path = this.filePath;
                else
                {
                    FileConnection fileConn = (FileConnection)this.connection;
                    path = fileConn.FilePath;
                }

                this.LoadFromFile(path);
            }
            else if( this.connection != null && this.connection.GetType().FullName == "MIX2.Acquisition.WebConnection")
            {
                WebConnection webConn = (WebConnection)this.connection;
                this.LoadFromStream( webConn.GetData( new object[1]{new System.Collections.DictionaryEntry("soup","[$Password]")} ) );
            }
            else
            {
                throw new Exception( "Connection for " + this.GetType().FullName + " must be either a WebRequest or File.");
            }

            this.objClass = objectClass;
            this.mapSchema = new MapSchema(this.objClass.Map);

            foreach( MapSchema.Namespace ns in this.mapSchema.Namespaces )
            {
                this.nsManager.AddNamespace(ns.Prefix, ns.Ns);
            }
                       
            this.nodes = this.source.XPathSelectElements( this.mapSchema.Query, this.nsManager );
            this.nodeIterator = this.nodes.GetEnumerator();
            this.eod = !this.nodeIterator.MoveNext();

            if (!this.eod)
                this.position = 1;

            return;
        }

        public void Close()
        {
            return;
        }

        public bool EOD
        {
            get { return this.eod; }
        }

        private string ExtractID(object obj)
        {
            string id = string.Empty;

            switch (obj.GetType().Name)
            {
                case "Boolean":
                    break;

                case "String":
                    id = (string)obj;
                    break;

                case "Double":
                    System.Double numericID = (System.Double)obj;
                    id = numericID.ToString();
                    break;

                default:

                    System.Collections.IEnumerable idNodes = (System.Collections.IEnumerable)obj;

                    foreach (Object idNode in idNodes)
                    {
                        string idVal = string.Empty;

                        switch (idNode.GetType().Name)
                        {
                            case "XAttribute":
                                System.Xml.Linq.XAttribute attr = (System.Xml.Linq.XAttribute)idNode;
                                idVal = attr.Value;
                                break;

                            case "XElement":
                                System.Xml.Linq.XElement elem = (System.Xml.Linq.XElement)idNode;
                                idVal = elem.Value;
                                break;
                        }

                        if (!string.IsNullOrEmpty(idVal))
                        {
                            if (id != string.Empty)
                                id += ";";

                            id += idVal;
                        }
                    }
                    break;
            }

            return id;

        }

        public MIX2.Data.LocalObject GetNext()
        {
            if (this.nodeIterator.Current == null)
                throw new Exception("No data. Use EOD to check if the method GetNext() should be called.");
            
            MIX2.Data.LocalObject obj = new MIX2.Data.LocalObject( this.sourceName, this.objClass.Domain, this.objClass.Name);

            string id = string.Empty;

            if (this.mapSchema.LocalID != null && !string.IsNullOrEmpty(this.mapSchema.LocalID.Query))
                id = ExtractID( this.nodeIterator.Current.XPathEvaluate(this.mapSchema.LocalID.Query, this.nsManager) );           

            if (id == string.Empty)
                id = "position(" + this.position.ToString() + ")";

            obj.LocalID = id;

            if (this.mapSchema.Parent != null)
            {
                string parentID = ExtractID( this.nodeIterator.Current.XPathEvaluate(this.mapSchema.Parent.Query, this.nsManager) );

                if (parentID != string.Empty)
                {
                    obj.ParentID = parentID;
                    obj.ParentClass = this.mapSchema.Parent.ObjectClass;
                }

                /*
                System.Xml.Linq.XElement parentNode = this.nodeIterator.Current.XPathSelectElement(this.mapSchema.Parent.Query, this.nsManager );

                if( parentNode != null )
                {
                    string parentID = ExtractID(parentNode.XPathEvaluate(this.mapSchema.Parent.LocalID.Query, this.nsManager));

                    if( parentID != string.Empty )
                    {
                        obj.ParentID = parentID;
                        obj.ParentClass = this.mapSchema.Parent.ObjectClass;
                    }
                }*/
            }

            foreach (MapSchema.Property prop in this.mapSchema.Properties)
            {
                MIX2.Data.LocalObject.Property objProp = new MIX2.Data.LocalObject.Property(prop.Name);

                object valsObj = this.nodeIterator.Current.XPathEvaluate(prop.Query, this.nsManager);

                switch (valsObj.GetType().Name)
                {
                    case "Boolean":
                        objProp.Values.Add(((System.Boolean)valsObj).ToString());
                        break;

                    case "String":
                        objProp.Values.Add( (string)valsObj );
                        break;

                    case "Double":
                        objProp.Values.Add( ((System.Double)valsObj).ToString() );
                        break;

                    default:
                        System.Collections.IEnumerable vals = (System.Collections.IEnumerable)valsObj;

                        foreach (Object valObj in vals)
                        {
                            string val = string.Empty;

                            switch (valObj.GetType().Name)
                            {
                                case "XAttribute":
                                    System.Xml.Linq.XAttribute attr = (System.Xml.Linq.XAttribute)valObj;
                                    val = attr.Value;
                                    break;

                                case "XElement":
                                    System.Xml.Linq.XElement elem = (System.Xml.Linq.XElement)valObj;
                                    val = elem.Value;
                                    break;
                            }

                            if (!string.IsNullOrEmpty(val))
                                objProp.Values.Add(val);

                        }
                        break;
                }

                if (objProp.Values.Count > 0)
                    obj.Properties.Add(objProp);
            }

            this.eod = !this.nodeIterator.MoveNext();

            if (!this.eod)
                this.position++;
            else
                this.position = -1;

            return obj;
        }
    }
}
