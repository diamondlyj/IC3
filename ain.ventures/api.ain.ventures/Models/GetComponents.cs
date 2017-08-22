using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.ain.ventures.Models
{
    public class GetComponents
    {
        public Guid ParentGUID { get; set; }
        public Guid ObjectGUID { get; set; }
        public int amount { get; set; }
        public string ComponentType { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public decimal price { get; set; }
        public string SKU { get; set; }
        public string supplier { get; set; }

    }
}