using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AIn.Ventures.Models
{
    [DataContract]
    public class Project
    {
        public Project()
        {
            this.Collaborators = new List<Models.Collaborator>();
            this.Stakeholders = new List<Stakeholder>();
        }

        [DataMember]
        public Nullable<System.Guid> GUID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public double? Shares { get; set; }

        [DataMember]
        public List<AIn.Ventures.Models.Collaborator> Collaborators = new List<AIn.Ventures.Models.Collaborator>();

        [DataMember]
        public List<AIn.Ventures.Models.Stakeholder> Stakeholders = new List<AIn.Ventures.Models.Stakeholder>();

        [DataMember]
        public List<AIn.Ventures.Models.Component> Components = new List<AIn.Ventures.Models.Component>();

    }
}
