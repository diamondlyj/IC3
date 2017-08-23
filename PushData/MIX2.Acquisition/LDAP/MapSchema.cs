using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

namespace MIX2.Acquisition.LDAP
{
    public class MapSchema: MIX2.Acquisition.MapSchema
    {
        protected LocalIDNode localID;
        //new protected ParentNode parent;

        public MapSchema(System.Xml.XPath.XPathNavigator navigator)
            : base(navigator)
        {            
            XPathNavigator pNav = this.navigator.SelectSingleNode("./Parent");

            XPathNavigator lidNav = this.navigator.SelectSingleNode("./LocalID");

            if (lidNav != null)
                this.localID = new LocalIDNode(lidNav);

            /*
            if ( pNav != null )
                this.parent = new ParentNode( pNav  );
             * */
        }


        public LocalIDNode LocalID
        {
            get { return this.localID; }
        }

        public class LocalIDNode
        {
            XPathNavigator m_MyNavigator;

            public LocalIDNode(XPathNavigator MyNavigator)
            {
                m_MyNavigator = MyNavigator;
            }

            public string Query
            {
                get { return m_MyNavigator.SelectSingleNode("./Query").Value; }
            }
        }

        /*
        new public ParentNode Parent
        {
            get
            {
                return this.parent;
            }
        }

        new public class ParentNode
        {
            XPathNavigator m_MyNavigator;

            public ParentNode(XPathNavigator MyNavigator)
            {
                m_MyNavigator = MyNavigator;
            }

            public string ObjectClass
            {
                get { return m_MyNavigator.GetAttribute("ObjectClass", ""); }
            }

            public string Path
            {
                get { return m_MyNavigator.SelectSingleNode("./Path").Value; }
            }

            public string Query
            {
                get { return m_MyNavigator.SelectSingleNode("./Query").Value; }
            }
        }
         * */
    }
}
