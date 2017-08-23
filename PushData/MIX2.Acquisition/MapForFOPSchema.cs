using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

namespace MIX2.Acquisition
{
    public class MapForFOPSchema : MapSchema
    {
        protected DelimitersNode delimiters;

        public MapForFOPSchema(XPathNavigator Navigator):base( Navigator)
        {
            this.delimiters = new DelimitersNode(Navigator.SelectSingleNode("./Delimiters"));
        }

        public DelimitersNode Delimiters
        {
            get
            {
                return this.delimiters;
            }
        }

        public class DelimitersNode
        {
            private XPathNavigator nav;

            public DelimitersNode(XPathNavigator Navigator)
            {
                this.nav = Navigator;
            }

            public char[] Instance
            {
                get
                {
                    XPathNavigator delimNav = this.nav.SelectSingleNode("./Instance");

                    if (nav != null)
                        return delimNav.Value.ToCharArray();
                    else
                        return null;
                }
            }

            public char[] Property
            {
                get
                {
                    XPathNavigator delimNav = this.nav.SelectSingleNode("./Property");

                    if (nav != null)
                        return delimNav.Value.ToCharArray();
                    else
                        return null;
                }
            }
        }

    }
}
