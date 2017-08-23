using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;
using System.Xml;


namespace MIX2.Acquisition.Configuration
{
    public class AcquisitionSection : ConfigurationSection
    {
        public AcquisitionSection()
        {
        }

        [ConfigurationProperty("DataSources", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(DataSources), AddItemName = "DataSource")]
        public DataSources DataSources
        {
            get { return base["DataSources"] as DataSources; }
        }



        [ConfigurationProperty("Updater", IsDefaultCollection = false)]
        public Updater Updater
        {
            get { return base["Updater"] as Updater; }
        }
    }


    public class Connection : ConfigurationElement
    {
        [ConfigurationProperty("Invariant", IsRequired = false)]
        public string Invariant
        {
            get { return (string)this["Invariant"]; }
            set { this["Invariant"] = value; }
        }

        [ConfigurationProperty("Type", IsRequired = false)]
        public string Type
        {
            get 
            { 
                string  tp = (string)this["Type"];
                
                if (tp == null || tp == string.Empty)
                    return "Database";
                else
                    return tp;
            }

            set
            {
                this["Type"] = value;
            }
        }

        [ConfigurationProperty("ConnectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)this["ConnectionString"]; }
            set { this["ConnectionString"] = value; }
        }

        [ConfigurationProperty("Timeout", IsRequired = true)]
        public int Timeout
        {
            get { int T = (int)this["Timeout"]; if (T < 0) throw new Exception("Timeout cannot be negative"); return T; }
            set { this["Timeout"] = value; }
        }

        [ConfigurationProperty("NetworkCredentialID", IsRequired = false)]
        public string NetworkCredentialID
        {
            get { return (string)this["NetworkCredentialID"]; }
        }

        [ConfigurationProperty("TokenProvider", IsRequired = false)]
        public string TokenProvider
        {
            get 
            {
                string val = (string)this["TokenProvider"];

                val= val.Trim();

                if (val == null || val == string.Empty)
                    return null;
                else
                    return val;
            }
        }
    }

    public class Map : ConfigurationElement
    {
        [ConfigurationProperty("Uri", IsRequired = true)]
        public Uri Uri
        {
            get { return (Uri)this["Uri"]; }
        }
    }

    public class DataSource : ConfigurationElement
    {
        public DataSource() { }

        [ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["Name"]; }
        }

        [ConfigurationProperty("SourceGUID", IsKey = true, IsRequired = false)]
        public string SourceGUID
        {
            get 
            {
                if (this["SourceGUID"] == null || (string)this["SourceGUID"] == String.Empty )
                    return null;

                return (string)this["SourceGUID"]; 
            }
        }

        [ConfigurationProperty("Connection", IsRequired = false)]
        public Connection Connection
        {
            get { return (Connection)this["Connection"]; }
        }

        [ConfigurationProperty("Map", IsRequired = true)]
        public Map Map
        {
            get { return (Map)this["Map"]; }
        }

        [ConfigurationProperty("MIXLibrary", IsRequired = false)]
        public MIXLibrary MIXLibrary
        {
            get { return (MIXLibrary)this["MIXLibrary"]; }
        }

    }

    public class DataSources : ConfigurationElementCollection
    {
        public DataSources() 
        { 
        }

        public DataSource this[int index]
        {
            get { return (DataSource)BaseGet(index); }
        }

        public DataSource  this[object Name]
        {
            get { return (DataSource)this.BaseGet(Name); }
        }

        public bool Exists(object key)
        {
            object[] keys = base.BaseGetAllKeys();

            for (int i = 0; i < keys.Length; i++)
            {
                if (key.Equals(keys[i]))
                    return true;
            }

            return false;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DataSource();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DataSource)element).Name;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

    }

    public class MIXLibrary: ConfigurationElement
    {
        [ConfigurationProperty("File", IsRequired = true)]
        public string File
        {
            get { return (string)this["File"]; }
        }
    }

    public class Updater : ConfigurationElement
    {
        public Updater() 
        { }

        [ConfigurationProperty("Url", IsRequired = true)]
        public Uri Url
        {
            get { return (Uri)this["Url"]; }
        }

        [ConfigurationProperty("MinTimeout", IsRequired = true)]
        public int MinTimeout
        {
            get { return (int)this["MinTimeout"]; }
        }

        [ConfigurationProperty("MaxTimeout", IsRequired = true)]
        public int MaxTimeout
        {
            get { return (int)this["MaxTimeout"]; }
        }

        [ConfigurationProperty("Attempts", IsRequired = true)]
        public int Attempts
        {
            get { return (int)this["Attempts"]; }
        }

        [ConfigurationProperty("NetworkCredentialID", IsRequired = false)]
        public string NetworkCredentialID
        {
            get
            {
                if (this["NetworkCredentialID"] == null || (string)this["NetworkCredentialID"] == String.Empty)
                    return null;

                return (string)this["NetworkCredentialID"];
            }
        }

    }        
}
