using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace AI.V2.BaseLibrary
{
    [Serializable]
    [DataContract]
    public class Category
    {
        private string path;
        private string name;
        private int depth;
        private List<Category> children;
        private DateTime created;
        private DateTime confirmed;
        private long objectCount;

        public Category()
        {
            this.children = new List<Category>();
            this.depth = -1;
            this.objectCount = -1;
        }

        [DataMember]
        public int Depth
        {
            get;
            set;
        }

        [DataMember]
        public long ObjectCount
        {
            get;
            set;
        }

        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public List<Category> Children
        {
            get;
            set;
        }

        [DataMember]
        public DateTime Confirmed
        {
            get;
            set;
        }

        [DataMember]
        public DateTime Created
        {
            get;
            set;
        }

        [DataMember]
        public string Path
        {
            get;
            set;
        }
    }
    [Serializable]
    [DataContract]
    public class Cube
    {
        private List<Category> catalogs;
        private string domain;
        private bool isDynamic;

        public Cube(string domain)
        {
            this.domain = domain;
            this.isDynamic = false;
            this.catalogs = new List<Category>();
        }
        [DataMember]
        public List<Category> Catalogs
        {
            get;
            set;
        }

        [DataMember]
        public bool IsDynamic
        {
            get;
            set;
        }

        [DataMember]
        public string Domain
        {
            get;
            set;
        }

        [DataContract]
      public class PropertyValue
        {
            private string prop;
            private string objClass;
            private bool inChild;
            private string[] val;

            [DataMember]
            public string Property
            {
                get;
                set;
            }

            [DataMember]
            public string ObjectClass
            {
                get;
                set;
            }

            [DataMember]
            public bool InChild
            {
                get;
                set;
            }

            [DataMember]
            public string[] Value
            {
                get;
                set;
            }
        }
    }
}
