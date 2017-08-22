using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using AIn.Ventures.BaseLibrary;

namespace AIn.Ventures.Models
{
    public class Collaborator: User
    {
        [DataMember]
        public Right Rights { get; set;}
    }
}
