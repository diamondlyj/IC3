using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace AIn.Ventures.Models
{
    [DataContract]
    public class Filter
    {
        public Filter()
        {
            this.Filters = new List<Models.Filter>();
        }
        [DataMember]
        public List<Filter> Filters { get; set; }
    }

    [DataContract]
    public class NewFilter
    {
        public NewFilter(string name)
        {
            this.Name = name;
            this.Filters = new List<Models.NewFilter>();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public List<NewFilter> Filters { get; set; }

    }
}
