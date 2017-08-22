using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AIn.Ventures.Models
{
    [DataContract]
    public class PasswordReset
    {
        [DataMember]
        public string NewPwd { get; set; }

        [DataMember]
        public string OldPwd { get; set; }
    }
}
