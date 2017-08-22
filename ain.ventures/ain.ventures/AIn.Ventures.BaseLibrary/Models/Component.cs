using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace AIn.Ventures.Models
{
    [DataContract]
    public class Component
    {
        [DataMember]
        public Nullable<System.Guid> GUID { get; set; }
        [DataMember]
        public Nullable<Guid> ParentGUID { get; set; }
        [DataMember]
        public int AmountInParent { get; set; }
        [DataMember]
        public string ComponentType { get; set; }
        [DataMember]
        [Required]
        [Display(Name="Name")]
        public string Name { get; set; }
        [DataMember]
        [Required]
        [Display(Name ="Description")]
        public string Description { get; set; }
        [DataMember]
        public Nullable<decimal> MinPrice { get; set; }
        [DataMember]
        public Nullable<decimal> MaxPrice { get; set; }
        [DataMember]
        public Nullable<decimal> Price { get; set; }
        [DataMember]
        public string Manufacturer { get; set; }
        [DataMember]
        public string SKU { get; set; }
        [DataMember]
        public string Supplier { get; set; }
        [DataMember]
        public Nullable<System.Guid> SourceGUID { get; set; }
        [DataMember]
        public List<AIn.Ventures.Models.Component> Children = new List<AIn.Ventures.Models.Component>();
        [DataMember]
        public Nullable<Guid> ProjectGUID { get; set; }
        //[DataMember]
        //public Guid ItemGuid { get; set; }
        //[DataMember]
        //public string ItemKey { get; set; }
        //[DataMember]
        //public DateTime? ItemDate { get; set; }
        [DataMember]
        public SourceProduct SourceProduct { get; set; }

    }
}
