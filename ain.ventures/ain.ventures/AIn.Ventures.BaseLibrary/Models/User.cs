using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AIn.Ventures.Models
{
    [DataContract]
    public class User
    {
        public User()
        {
            this.Projects = new List<Project>();
            this.Colleagues = new List<Colleague>();
        }
        [DataMember]
        public System.Guid GUID { get; set; }

        [DataMember]
        public string GivenNames { get; set; }

        [DataMember]
        public string Surname { get; set; }


        [DataMember]
        public string EmailAddress { get; set; }


        [DataMember]
        public long PhoneNumber { get; set; }

        [DataMember]
        public string Pwd { get; set; }

        [DataMember]
        public List<Project> Projects { get; set; }

        [DataMember]
        public List<Colleague> Colleagues { get; set; }
    }
}
