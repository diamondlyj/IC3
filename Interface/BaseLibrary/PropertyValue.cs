using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace AI.V2.BaseLibrary
{
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

