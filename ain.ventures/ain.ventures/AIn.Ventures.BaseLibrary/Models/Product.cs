using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AIn.Ventures.Models
{
    [DataContract]
    public class Product
    {
        [DataMember]
        public Nullable<System.Guid> GUID { get; set; }
        [DataMember]
        public Guid ParentGUID { get; set; }
        [DataMember]
        public Guid ObjectGUID { get; set; }
        
        [DataMember]
        public string ComponentType { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public Nullable<decimal> Price { get; set; }
        [DataMember]
        public string Manufacturer { get; set; }
        [DataMember]
        public string SKU { get; set; }
        [DataMember]
        public string Supplier { get; set; }
        [DataMember]
        public Nullable<System.Guid> ProjectGUID { get; set; }
        [DataMember]
        public List<AIn.Ventures.Models.Product> Products = new List<AIn.Ventures.Models.Product>();
        [DataMember]
        public List<AIn.Ventures.Models.Component> Components = new List<AIn.Ventures.Models.Component>();

    }
}

