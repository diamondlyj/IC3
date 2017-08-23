using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntuitiveLabs.Security
{
    [Serializable]
    public class Directory
    {
        private string identifier;
        private Guid guid;
        List<Group> groups;
        private int groupCount;

        public Directory() 
        {
            this.init();
        }
        
        public Directory( string identifier, Guid guid )
        {
            this.init();
            this.identifier = identifier;
            this.guid = guid;
        }

        private void init()
        {
            this.guid = Guid.Empty;
            this.identifier = string.Empty;
            this.groups = new List<Group>();
            this.groupCount = -1;
        }

        public Guid GUID
        {
            get { return this.guid; }
            set { this.guid = value; }
        }

        public int GroupCount
        {
            get { return this.groupCount; }
            set { this.groupCount = value; }
        }

        public List<Group> Groups
        {
            get { return this.groups; }
            set { this.groups = value; }
        }

        public string Identifier
        {
            get { return this.identifier; }
            set { this.identifier = value; }
        }
    }
}
