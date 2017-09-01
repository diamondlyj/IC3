using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AI.V2.BaseLibrary
{
    [Serializable]
    [DataContract]
    public class DependencyLink
    {
        private string domain;
        private string dependencyType;
        private Guid guid;
        private string objectClass;
        private string friendlyName;
        private System.Guid objectGUID;
        private double weight;
        private DateTime confirmed;
        private DateTime created;
        private bool isFrozen;
        public DependencyLink()
        {
            this.Dependencies = new List<AI.V2.BaseLibrary.Dependency>();
            this.Dependants = new List<AI.V2.BaseLibrary.Dependant>();
        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int DefaultWeight { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string DependencyType { get; set; }
        [DataMember]
        public string ObjectClass { get; set; }
        [DataMember]
        public string FriendlyName { get; set; }
        [DataMember]
        public Guid ObjectGUID { get; set; }
        [DataMember]
        public double Weight { get; set; }
        [DataMember]
        public DateTime Created { get; set; }
        [DataMember]
        public bool IsFrozen { get; set; }
       
        [DataMember]
        public DateTime Confirmed { get; set; }
        [DataMember]
        public string Domain { get; set; }
        [DataMember]
        public Guid GUID { get; set; }
        [DataMember]
        public int DomainID { get; set; }
        [DataMember]
        public int ObjectClassID { get; set; }
        [DataMember]
        public int DependencyTypeID { get; set; }
        [DataMember]
        public List<AI.V2.BaseLibrary.Dependency> Dependencies { get; set; }

        [DataMember]
        public List<AI.V2.BaseLibrary.Dependant> Dependants { get; set; }
    }

    [DataContract]
    public class Dependency
    {
        public Dependency()
        {
            this.Dependencies = new List<AI.V2.BaseLibrary.Dependency>();
        }
        [DataMember]
        public List<AI.V2.BaseLibrary.Dependency> Dependencies { get; set; }
        [DataMember]
        public string DependencyType { get; set; }
        [DataMember]
        public string Domain { get; set; }
        [DataMember]
        public string SubjectClass { get; set; }
        [DataMember]
        public Guid PredictGUID { get; set; }
        [DataMember]
        public double Weight { get; set; }
        [DataMember]
        public bool IsFrozen { get; set; }
        [DataMember]
        public Guid GUID { get; set; }
        [DataMember]
        public Guid PredicateGUID { get; set; }
        [DataMember]
        public Guid SubjectGUID { get; set; }
        [DataMember]
        public int DependencyTypeID { get; set; }
        [DataMember]
        public int SubjectClassID { get; set; }
        [DataMember]
        public int PredicateClassID { get; set; }
        [DataMember]
        public System.DateTime Created { get; set; }
        [DataMember]
        public System.DateTime Confirmed { get; set; }
        [DataMember]
        public string ObjectClass { get; set; }
        [DataMember]
        public Guid ObjectGUID { get; set; }
    }

    [DataContract]
    public class Dependant
    {
        public Dependant()
        {
            this.Dependants = new List<AI.V2.BaseLibrary.Dependant>();
        }
        [DataMember]
        public List<AI.V2.BaseLibrary.Dependant> Dependants { get; set; }
        [DataMember]
        public string DependencyType { get; set; }
        [DataMember]
        public string Domain { get; set; }
        [DataMember]
        public string SubjectClass { get; set; }
        [DataMember]
        public Guid SubjectGUID { get; set; }
        [DataMember]
        public double Weight { get; set; }
        [DataMember]
        public string ObjectClass { get; set; }
        [DataMember]
        public Guid ObjectGUID { get; set; }
    }

}
