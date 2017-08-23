using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using System.Xml.XPath;


namespace MIX.Data
{



    public class DataSchema
    {


        public DataSchema(XPathDocument xmlDataSchema)
        {
            m_xmlDataSchema = xmlDataSchema;
            m_MyNavigator = m_xmlDataSchema.CreateNavigator();

            m_DataClasses = new DataClassCollection(m_MyNavigator);
        }




        XPathDocument m_xmlDataSchema;
        XPathNavigator m_MyNavigator;

        DataClassCollection m_DataClasses;

        public DataClassCollection DataClasses
        {
            get { return m_DataClasses; }
        }

        public class DataClass
        {
            XPathNavigator m_MyNavigator;

            AttributeCollection m_Attributes;

            public AttributeCollection Attributes
            {
                get { return m_Attributes; }
            }

            public DataClass(XPathNavigator MyNavigator)
            {

                m_MyNavigator = MyNavigator;

                
                m_Attributes = new AttributeCollection(m_MyNavigator);
            }

            public string Name
            {
                get { return m_MyNavigator.GetAttribute("Name", ""); }
            }



            public class Attribute
            {
                XPathNavigator m_MyNavigator;

                public Attribute(XPathNavigator MyNavigator)
                {

                    m_MyNavigator = MyNavigator;
                }

                public string Name
                {
                    get { return m_MyNavigator.GetAttribute("Name", ""); }
                }

            }

            public class AttributeCollection : IEnumerable<Attribute>
            {
                XPathNavigator m_MyNavigator;

                internal AttributeCollection(XPathNavigator MyNavigator)
                {
                    m_MyNavigator = MyNavigator;
                }

                IEnumerator<Attribute> IEnumerable<Attribute>.GetEnumerator()
                {
                    return new MyEnumerator(this);
                }
                IEnumerator IEnumerable.GetEnumerator()
                {
                    return ((IEnumerable<DataClass>)this).GetEnumerator();
                }


                public class MyEnumerator : IEnumerator<Attribute>
                {
                    //DataClassCollection m_MyCollection;

                    XPathNavigator m_MyNavigator;
                    XPathNodeIterator m_MyIterator;

                    public MyEnumerator(AttributeCollection MyCollection)
                    {
                        m_MyNavigator = MyCollection.m_MyNavigator;
                        m_MyIterator = m_MyNavigator.Select("./Attribute");
                    }



                    void IEnumerator.Reset()
                    {
                        m_MyIterator = m_MyNavigator.Select("./Attribute");
                    }

                    bool IEnumerator.MoveNext()
                    {
                        return m_MyIterator.MoveNext();
                    }

                    Attribute IEnumerator<Attribute>.Current
                    {
                        get
                        {
                            return new Attribute(m_MyIterator.Current);
                        }
                    }

                    object IEnumerator.Current
                    {
                        get
                        {
                            return ((IEnumerator<Attribute>)this).Current;
                        }
                    }

                    public void Dispose() { }




                }


            }

        }

        public class DataClassCollection : IEnumerable<DataClass>
        {
            XPathNavigator m_MyNavigator;

            internal DataClassCollection(XPathNavigator MyNavigator)
            {
                m_MyNavigator = MyNavigator;
            }

            IEnumerator<DataClass> IEnumerable<DataClass>.GetEnumerator()
            {
                return new MyEnumerator(this);
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<DataClass>)this).GetEnumerator();
            }


            public class MyEnumerator : IEnumerator<DataClass>
            {
                //DataClassCollection m_MyCollection;

                XPathNavigator m_MyNavigator;
                XPathNodeIterator m_MyIterator;

                public MyEnumerator(DataClassCollection MyCollection)
                {
                    m_MyNavigator = MyCollection.m_MyNavigator;
                    m_MyIterator  = m_MyNavigator.Select("/DataSchema/DataClass");
                }



                void IEnumerator.Reset()
                {
                    m_MyIterator = m_MyNavigator.Select("");
                }
  
                bool IEnumerator.MoveNext()
                {
                    return m_MyIterator.MoveNext();
                }
                
                DataClass IEnumerator<DataClass>.Current
                {
                    get
                    {
                        return new DataClass(m_MyIterator.Current);
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return ((IEnumerator<DataClass>)this).Current;
                    }
                }

                public void Dispose() { }
            
            
            
            
            }



        }
        
    
    }
}
