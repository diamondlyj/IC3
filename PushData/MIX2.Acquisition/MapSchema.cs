using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

namespace MIX2.Acquisition
{
    public class MapSchema
    {
        protected XPathNavigator navigator;
        protected ParentNode parent;
        protected PropertyCollection properties;

        public MapSchema( XPathNavigator Navigator )
        {
            this.navigator = Navigator;
            this.properties = new PropertyCollection(this.navigator);

            XPathNavigator pNav = this.navigator.SelectSingleNode("./Parent");
            
            if ( pNav != null )
                this.parent = new ParentNode( pNav  );
        }

        public string Locus
        {
            get { return this.navigator.GetAttribute("Locus", ""); }
        }

        public string ExtractionMethod
        {
            get {
                XPathNavigator nav = this.navigator.SelectSingleNode("./Query");

                if (nav == null)
                    return string.Empty;

                return nav.GetAttribute("ExtractionMethod", ""); 
            }
        }

        public string ExtractionProperties
        {
            get {             
                XPathNavigator nav = this.navigator.SelectSingleNode("./Query");

                if (nav == null)
                    return string.Empty;

                return nav.GetAttribute("Properties", ""); 
            }
        }

        public string ExtractionDelimiter
        {
            get
            {
                XPathNavigator nav = this.navigator.SelectSingleNode("./Query");

                if (nav == null)
                    return string.Empty;
                
                return nav.GetAttribute("ValueDelimiter", "");
            }
        }

        public string LocalID
        {
            get
            {
                XPathNavigator nav = this.navigator.SelectSingleNode("./Query");

                if (nav == null)
                    return string.Empty;
                
                return nav.GetAttribute("LocalID", ""); 
            }
        }

        public class ParentNode
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

            public string Query
            {
                get 
                {
                    XPathNavigator nav = m_MyNavigator.SelectSingleNode("./Query");

                    if( nav != null )
                        return nav.Value; 
                    else
                        return null;
                }
            }
        }

        public class Property
        {
            XPathNavigator m_MyNavigator;
            TrimNode m_Trim;
            protected ValueCollection values;

            public Property(XPathNavigator MyNavigator)
            {
                m_MyNavigator = MyNavigator;
                m_Trim = new TrimNode(m_MyNavigator);
                this.values = new ValueCollection(m_MyNavigator);
            }

            public string Name
            {
                get { return m_MyNavigator.GetAttribute("Name", ""); }
            }

            public string Query
            {
                get {
                    XPathNavigator qnav = m_MyNavigator.SelectSingleNode("./Query");
                    
                    if (qnav == null)
                        return null;

                    return qnav.Value;
                }
            }

            public ValueCollection Values
            {
                get { return this.values; }
            }

            
            public TrimNode Trim
            {
                get { return this.m_Trim; }
            }
                
            public class TrimNode
            {
                XPathNavigator m_MyNavigator;

                public TrimNode(XPathNavigator MyNavigator) { m_MyNavigator = MyNavigator; }

                public char[] Start
                {
                    get
                    {
                        XPathNavigator nav = m_MyNavigator.SelectSingleNode("./Trim/Start");

                        if (nav != null)
                            return nav.Value.ToCharArray();
                        else
                            return null;
                    }
                }

                public char[] End
                {
                    get
                    {
                        XPathNavigator nav = m_MyNavigator.SelectSingleNode("./Trim/End");

                        if (nav != null)
                            return nav.Value.ToCharArray();
                        else
                            return null;
                    }
                }
            }

            public char[] ValueDelimiter
            {
                get
                {
                    XPathNavigator nav = m_MyNavigator.SelectSingleNode("./ValueDelimiter");

                    if (nav != null)
                        return nav.Value.ToCharArray();
                    else
                        return null;
                }
            }
        }

        public class Value
        {
            XPathNavigator m_MyNavigator;

            public Value(XPathNavigator MyNavigator)
            {
                m_MyNavigator = MyNavigator;
            }


            public string Action
            {
                get { return m_MyNavigator.GetAttribute("Action", "").ToLower(); }
            }

            public string InnerValue
            {
                get { return m_MyNavigator.Value; }
            }
        }

        public ParentNode Parent
        {
            get { return this.parent; }
        }

        public PropertyCollection Properties
        {
            get { return this.properties; }
        }

        public class PropertyCollection : IEnumerable<Property>
        {
            XPathNavigator navigator;

            internal PropertyCollection(XPathNavigator Navigator)
            {
              this.navigator = Navigator;
            }

            IEnumerator<Property> IEnumerable<Property>.GetEnumerator()
            {
              return new Enumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
              return ((IEnumerable<Property>)this).GetEnumerator();
            }


            public class Enumerator : IEnumerator<Property>
            {
                //DataClassCollection m_MyCollection;

                XPathNavigator navigator;
                XPathNodeIterator iterator;

                public Enumerator(PropertyCollection Collection)
                {
                    navigator = Collection.navigator;
                    iterator = navigator.Select("./Property");
                }



                void IEnumerator.Reset()
                {
                    iterator = navigator.Select("./Property");
                }

                bool IEnumerator.MoveNext()
                {
                    return iterator.MoveNext();
                }

                Property IEnumerator<Property>.Current
                {
                    get
                    {
                      return new Property(iterator.Current);
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                      return ((IEnumerator<Property>)this).Current;
                    }
                }

                public void Dispose() { }
            }
        }

        public class ValueCollection : IEnumerable<Value>
        {
            XPathNavigator navigator;

            internal ValueCollection(XPathNavigator Navigator)
            {
                this.navigator = Navigator;
            }

            IEnumerator<Value> IEnumerable<Value>.GetEnumerator()
            {
                return new Enumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<Value>)this).GetEnumerator();
            }


            public class Enumerator : IEnumerator<Value>
            {
                //DataClassCollection m_MyCollection;

                XPathNavigator navigator;
                XPathNodeIterator iterator;

                public Enumerator(ValueCollection Collection)
                {
                    navigator = Collection.navigator;
                    iterator = navigator.Select("./Value");
                }



                void IEnumerator.Reset()
                {
                    iterator = navigator.Select("./Value");
                }

                bool IEnumerator.MoveNext()
                {
                    return iterator.MoveNext();
                }

                Value IEnumerator<Value>.Current
                {
                    get
                    {
                        return new Value(iterator.Current);
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return ((IEnumerator<Value>)this).Current;
                    }
                }

                public void Dispose() { }
            }
        }

        public string Query
        {
            get { return this.navigator.SelectSingleNode("./Query").Value; }
        }
    }
}
