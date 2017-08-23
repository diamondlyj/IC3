using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

namespace MIX2.Acquisition.Xml
{
    public class MapSchema: MIX2.Acquisition.MapSchema
    {
        protected LocalIDNode localID;
        protected NamespaceCollection namespaces;
        //new protected ParentNode parent;

        public MapSchema(System.Xml.XPath.XPathNavigator navigator)
            : base(navigator)
        {
            this.namespaces = new NamespaceCollection(this.navigator);

            XPathNavigator lidNav = this.navigator.SelectSingleNode("./LocalID");

            if (lidNav != null)
                this.localID = new LocalIDNode(lidNav);

            /*
            XPathNavigator pNav = this.navigator.SelectSingleNode("./Parent");
            
            if ( pNav != null )
                this.parent = new ParentNode( pNav  );
             * */
        }

        public LocalIDNode LocalID
        {
            get { return this.localID; }
        }

        public NamespaceCollection Namespaces
        {
            get { return this.namespaces; }
        }

        /*
        new public ParentNode Parent
        {
            get { return this.parent; }
        }
        */

        /*
         * Classes
         * */

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


        public class Namespace
        {
            XPathNavigator m_MyNavigator;

            public Namespace(XPathNavigator MyNavigator)
            {
                m_MyNavigator = MyNavigator;
            }

            public string Ns
            {
                get { return m_MyNavigator.GetAttribute("Ns", ""); }
            }

            public string Prefix
            {
                get { return m_MyNavigator.GetAttribute("Prefix", ""); }
            }
        }


        public class NamespaceCollection : IEnumerable<Namespace>
        {
            XPathNavigator navigator;

            internal NamespaceCollection(XPathNavigator Navigator)
            {
                this.navigator = Navigator;
            }

            IEnumerator<Namespace> IEnumerable<Namespace>.GetEnumerator()
            {
                return new Enumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<Namespace>)this).GetEnumerator();
            }


            public class Enumerator : IEnumerator<Namespace>
            {
                //DataClassCollection m_MyCollection;

                XPathNavigator navigator;
                XPathNodeIterator iterator;

                public Enumerator(NamespaceCollection Collection)
                {
                    navigator = Collection.navigator;
                    iterator = navigator.Select("./Namespace");
                }



                void IEnumerator.Reset()
                {
                    iterator = navigator.Select("./Namespace");
                }

                bool IEnumerator.MoveNext()
                {
                    return iterator.MoveNext();
                }

                Namespace IEnumerator<Namespace>.Current
                {
                    get
                    {
                        return new Namespace(iterator.Current);
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return ((IEnumerator<Namespace>)this).Current;
                    }
                }

                public void Dispose() { }
            }
        }

        /*
        public class ParentNode
        {
            private LocalIDNode localID;

            XPathNavigator m_MyNavigator;

            public ParentNode(XPathNavigator MyNavigator)
            {
                m_MyNavigator = MyNavigator;

                XPathNavigator lidNav = m_MyNavigator.SelectSingleNode("./LocalID");

                if (lidNav != null)
                    this.localID = new LocalIDNode(lidNav);
            }

            public string ObjectClass
            {
                get { return m_MyNavigator.GetAttribute("ObjectClass", ""); }
            }

            public string Query
            {
                get { return m_MyNavigator.SelectSingleNode("./Query").Value; }
            }

            public LocalIDNode LocalID
            {
                get { return this.localID; }
            }
        }
         * */
    }
}
