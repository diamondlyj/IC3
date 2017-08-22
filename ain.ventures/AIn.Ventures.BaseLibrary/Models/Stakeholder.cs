using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AIn.Ventures.Models
{
    [DataContract]
    public class Stakeholder
    {
        [DataMember]
        public System.Guid GUID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public float Shares { get; set; }

        [DataMember]
        public Boolean IsUser { get; set; }

        [DataMember]
        public System.Guid ProjectGUID { get; set; }

    }
}
