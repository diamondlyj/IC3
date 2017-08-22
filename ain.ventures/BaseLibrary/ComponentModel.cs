using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseLibrary
{
    public class ComponentModel
    {
        public System.Guid ObjectGUID { get; set; }
        public Guid ParentGuid { get; set; }
        public int AmountInParent { get; set; }
        public string ComponentType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Manufacturer { get; set; }
        public string SKU { get; set; }
        public string Supplier { get; set; }
        public Nullable<System.Guid> ProjectGUID { get; set; }

        public List<ComponentModel> Elements = new List<ComponentModel>();

    }
}
