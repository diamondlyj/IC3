using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AI.V2.BaseLibrary
{
    [DataContract]
    public class Filter
    {
        public Filter()
        {
            this.Category = new string[] { };
            this.PropertyValue = new Cube.PropertyValue[] { };
        }

        [DataMember]
        public string[] Category
        {
            get; set;
        }

        [DataMember]
        public Cube.PropertyValue[] PropertyValue
        {
            get; set;
        }

        public static string CreateXmlFilter(string[] category, AI.V2.BaseLibrary.Cube.PropertyValue[] propertyValue)
        {
            string filter = "<Filter>";

            if (category.Length > 0)
            {
                filter += "<Category>";

                for (int i = 0; i < category.Length; i++)
                {
                    filter += "<Path>" + System.Security.SecurityElement.Escape(category[i]) + "</Path>";
                }

                filter += "</Category>";
            }

            if (propertyValue.Length > 0)
            {
                for (int i = 0; i < propertyValue.Length; i++)
                {
                    filter += propertyValue[i].ToString();
                }

            }

            filter += "</Filter>";

            return filter;

        }
    }
}
