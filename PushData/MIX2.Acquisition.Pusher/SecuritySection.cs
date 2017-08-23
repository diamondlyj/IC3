using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace IntuitiveLabs.Configuration
{
    public class SecuritySection : ConfigurationSection
    {        
        public SecuritySection()
        {
        }

        [ConfigurationProperty("NetworkCredentials", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(NetworkCredentialsCollection), AddItemName = "NetworkCredential")]        
        public NetworkCredentialsCollection NetworkCredentials
        {
            get { return base["NetworkCredentials"] as NetworkCredentialsCollection; }
        }


        [ConfigurationProperty("Salt", IsDefaultCollection = false)]
        public Salt Salt
        {
            get { return base["Salt"] as Salt; }
        }

        [ConfigurationProperty("TokenProviders", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(NetworkCredentialsCollection), AddItemName = "TokenProvider")]
        public TokenProviderCollection TokenProviders
        {
            get { return base["TokenProviders"] as TokenProviderCollection; }
        }
    }

    public class Salt: ConfigurationElement
    {
        public Salt()
        {
        }

        [ConfigurationProperty("Value", IsKey = true, IsRequired = true)]
        public string Value
        {
            get { return  (string)this["Value"]; }
        }
    }

    public class NetworkCredential : ConfigurationElement
    {
        public NetworkCredential()
        {
        }

        [ConfigurationProperty("ID", IsKey = true, IsRequired = true)]
        public string ID
        {
            get { return (string)this["ID"]; }
        }

        [ConfigurationProperty("Domain", IsRequired = false)]
        public string Domain
        {
            get { return (string)this["Domain"]; }
        }

        [ConfigurationProperty("UserName", IsRequired = true)]
        public string UserName
        {
            get { return (string)this["UserName"]; }
        }

        [ConfigurationProperty("Password", IsRequired = true)]
        public string Password
        {
            get { return (string)this["Password"]; }
        }

        public System.Net.NetworkCredential Create()
        {
            return new System.Net.NetworkCredential(this.UserName, this.Password);
        }
    }

    public class NetworkCredentialsCollection : ConfigurationElementCollection
    {
        public NetworkCredential this[int index]
        {
            get { return (NetworkCredential)BaseGet(index); }
        }

        public NetworkCredential this[object id]
        {
            get { return (NetworkCredential)this.BaseGet(id); }
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
            return new NetworkCredential();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NetworkCredential)element).ID;
        }
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
    }

    public class TokenProviderCollection : ConfigurationElementCollection
    {
        public TokenProvider this[int index]
        {
            get { return (TokenProvider)BaseGet(index); }
        }

        public TokenProvider this[object id]
        {
            get { return (TokenProvider)this.BaseGet(id); }
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
            return new TokenProvider();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TokenProvider)element).Name;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
    }

    public class TokenProvider : ConfigurationElement
    {
        public TokenProvider() { }

        [ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["Name"]; }
        }

        [ConfigurationProperty("Connection", IsRequired = true)]
        public MIX2.Acquisition.Configuration.Connection Connection
        {
            get { return (MIX2.Acquisition.Configuration.Connection)this["Connection"]; }
        }


        [ConfigurationProperty("MIXLibrary", IsRequired = false)]
        public MIX2.Acquisition.Configuration.MIXLibrary MIXLibrary
        {
            get { return (MIX2.Acquisition.Configuration.MIXLibrary)this["MIXLibrary"]; }
        }

    }

    public class Authentication : ConfigurationElement
    {
        [ConfigurationProperty("AuthenticationString", IsRequired = true)]
        public string AuthenticationString
        {
            get { return (string)this["AuthenticationString"]; }
        }

        [ConfigurationProperty("NetworkCredentialID", IsRequired = false)]
        public string NetworkCredentialID
        {
            get { return (string)this["NetworkCredentialID"]; }
        }
    }
}
