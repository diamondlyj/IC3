using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AIn.Ventures.Models
{
    [DataContract]
    public class UserProject
    {

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

    }
}
