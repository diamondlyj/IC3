using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AIn.Ventures.Models
{
    [DataContract]
    public class Catalog 
    {
        public Catalog()
        {
            this.Categories = new List<Models.Category>();
        }
        [DataMember]
        public List<Category> Categories { get; set; }
    }

    [DataContract]
    public class Category
    {
        public Category( string name )
        {
            this.Name = name;
            this.Categories = new List<Models.Category>();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public List<Category> Categories { get; set; }

    }
}
