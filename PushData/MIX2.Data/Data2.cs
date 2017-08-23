using System;
using System.Collections.Generic;
using System.Text;

namespace MIX2.Data
{
    public class LocalObject
    {
        private string domain;
        private object localID;
        private string objectClass;
        private string parentClass;
        private object parentID;
        private string source;

        public List<Property> Properties;

        public LocalObject( string Source, string Domain, string ObjectClass )
        {
            this.Properties = new List<Property>();
            this.objectClass = ObjectClass;
            this.domain = Domain;
            this.source = Source;
        }

        public LocalObject()
        {
            this.Properties = new List<Property>();
        }

        public string ParentClass
        {
            get { return this.parentClass; }
            set { this.parentClass = value; }
        }

        public object ParentID
        {
            get { return this.parentID; }
            set { this.parentID = value; }
        }

        public object LocalID
        {
            get { return this.localID; }
            set { this.localID = value; }
        }

        public string GetXml()
        {
            System.Text.StringBuilder builder = new StringBuilder();

            System.Xml.XmlWriterSettings writerSettings = new System.Xml.XmlWriterSettings();
            writerSettings.CheckCharacters = false;

            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create( builder,writerSettings );
            writer.WriteStartElement("Object");

            writer.WriteStartAttribute("Domain");
            writer.WriteString(this.domain );
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("ObjectClass");
            writer.WriteString(this.ObjectClass);
            writer.WriteEndAttribute();

            if (this.localID != null)
            {
                writer.WriteStartElement("LocalID");
                writer.WriteCData(this.localID.ToString());
                writer.WriteEndElement();
            }

            if (this.parentID != null)
            {
                writer.WriteStartElement("Parent");

                writer.WriteStartAttribute("ObjectClass");
                writer.WriteString(this.parentClass);
                writer.WriteEndAttribute();

                writer.WriteStartElement("LocalID");
                writer.WriteCData(this.parentID.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            writer.WriteStartElement("Signature");

            foreach (Property prop in this.Properties)
            {
                writer.WriteStartElement("Property");
                
                writer.WriteStartAttribute("Name");
                writer.WriteString(prop.Name);
                writer.WriteEndAttribute();

                foreach (string val in prop.Values)
                {
                    writer.WriteStartElement("Value");
                    writer.WriteCData(val.Trim());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Flush();

            return  builder.ToString();
        }

        public string ObjectClass
        {
            get { return this.objectClass; }
            set { this.objectClass = value; }
        }

        public class Property
        {
            private string name;
            public System.Collections.Specialized.StringCollection Values;

            public Property( string Name )
            {
                this.name = Name;
                this.Values = new System.Collections.Specialized.StringCollection();
            }

            public string Name
            {
                get { return this.name; }
            }
        }

        public string Source
        {
            get { return this.source; }
        }
    }
}
