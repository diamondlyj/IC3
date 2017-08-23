using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIn.Acquisition.Infoblox
{
        public class XmlFileClass
        {
            public XmlFileClass()
            {
                list = new List<valueObject>();
            }
            public string next_page_id { get; set; }

            public List<valueObject> list { get; set; }
        }

        public class valueObject
        {
            public valueObject(string comment, string network, string network_view, string _ref)
            {
                this._Ref = _ref;
                this.Comment = comment;
                this.Network = network;
                this.Network_view = network_view;
            }
            public string Comment { get; set; }
            public string Network { get; set; }
            public string Network_view { get; set; }
            public string _Ref { get; set; }
        }
    }


