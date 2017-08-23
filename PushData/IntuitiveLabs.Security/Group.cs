using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntuitiveLabs.Security
{
    [Serializable]
    public class Group
    {
        private string identifier;
        private Guid guid;
        private Role role;

        public Group() 
        {
        }

        public Group(string identifier, Guid guid)
        {
            this.identifier = identifier;
            this.guid = guid;
        }

        public Guid GUID
        {
            get { return this.guid; }
            set { this.guid = value; }
        }

        public Role Role
        {
            get { return this.role; }
            set { this.role = value; }
        }

        public string Identifier
        {
            get { return this.identifier; }
            set { this.identifier = value; }
        }        
    }
}
