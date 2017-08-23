using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.XPath;

namespace MIX2.Acquisition
{
    public class DataSourceSchema
    {
        private XPathNavigator m_navDataSourceSchema;
        private ObjectClassCollection m_ocCollection;
        private string m_sDataSourceName;
        private ObjectClassCollection objClasses;

        public DataSourceSchema(string DataSourceName, XPathNavigator navDataSourceSchema)
        {
            this.m_sDataSourceName = DataSourceName;
            this.m_navDataSourceSchema = navDataSourceSchema.SelectSingleNode("DataSource");
            
            this.m_ocCollection = new ObjectClassCollection(m_navDataSourceSchema);

            this.objClasses = new ObjectClassCollection( navDataSourceSchema );

            //this.m_dpIndexCollection = new DataPointIndexCollection(m_navDataSourceSchema);
        }

        static public TimeSpan DefaulPeriod
        {
            get { return new TimeSpan(7, 0, 0, 0); }
        }


        /*
        public TimeSpan GetPeriod(DataPointIndex Index)
        {
            TimeSpan tsPeriod = TimeSpan.Zero; ;

            XPathNodeIterator iterator = m_navDataSourceSchema.Select("./ObjectClass[@Name=\"" + Index.ObjectClass + "\"]/Property[@Name=\"" + Index.Property + "\"]");

            string sPeriod;
            if (iterator.MoveNext())
            {
                sPeriod = iterator.Current.GetAttribute("Period", "");
                if (sPeriod != "")
                    tsPeriod = TimeSpan.Parse(sPeriod);
                if (tsPeriod == TimeSpan.Zero)
                {
                    if (iterator.Current.MoveToParent())
                    {
                        sPeriod = iterator.Current.GetAttribute("Period", "");
                        if (sPeriod != "")
                            tsPeriod = TimeSpan.Parse(sPeriod);
                        if (tsPeriod == TimeSpan.Zero)
                            tsPeriod = DefaulPeriod;
                    }
                }
            }
            return tsPeriod;
        }
         * */

        public class ObjectClass
        {
            XPathNavigator navigator;

            public ObjectClass( XPathNavigator Navigator )
            {
                this.navigator = Navigator.Clone();
            }

            public string Xml
            {
                get { return this.navigator.OuterXml; }
            }
            /*
            public string[] ParentID
            {
                get
                {
                    XPathNavigator nav = this.navigator.SelectSingleNode("./ParentID");
                    XPathNodeIterator iter = nav.SelectChildren("Property", "");

                    string[] result = new string[iter.Count];
                    int n = 0;

                    while (iter.MoveNext())
                    {
                        result[n] = iter.Current.Value;
                        n++;
                    }

                    return result;
                }
            }

            //Is similar to above can be place in SimplePropertCollection class or common method 
            public string[] LocalID
            {
                get
                {
                    XPathNavigator nav = this.navigator.SelectSingleNode("./LocalID");
                    XPathNodeIterator iter = nav.SelectChildren("Property", "");

                    string[] result = new string[iter.Count];
                    int n = 0;

                    while (iter.MoveNext())
                    {
                        result[n] = iter.Current.Value;
                        n++;
                    }

                    return result;
                }
            } 
             * */
            public TimeSpan Period
            {
                get
                {
                    TimeSpan tsPeriod = TimeSpan.Zero; ;

                    XPathNavigator nav = this.navigator.Clone();

                    string sPeriod = nav.GetAttribute("Period", "");

                    if (sPeriod != "")
                        tsPeriod = TimeSpan.Parse(sPeriod);
                    else
                    //if (tsPeriod == TimeSpan.Zero)
                    {
                        if (nav.MoveToParent())
                        {
                            sPeriod = nav.GetAttribute("Period", "");

                            if (sPeriod != "")
                                tsPeriod = TimeSpan.Parse(sPeriod);
                            else
                                tsPeriod = DefaulPeriod;

                            //if (tsPeriod == TimeSpan.Zero)
                            //    tsPeriod = DefaulPeriod;
                        }
                    }

                    /*
                    XPathNodeIterator iterator = m_navDataSourceSchema.Select("./ObjectClass[@Name=\"" + ObjectClass.Name + "\"]");

                    string sPeriod;
                    if (iterator.MoveNext())
                    {
                        sPeriod = iterator.Current.GetAttribute("Period", "");
                        if (sPeriod != "")
                            tsPeriod = TimeSpan.Parse(sPeriod);
                        else
                        //if (tsPeriod == TimeSpan.Zero)
                        {
                            if (iterator.Current.MoveToParent())
                            {
                                sPeriod = iterator.Current.GetAttribute("Period", "");

                                if (sPeriod != "")
                                    tsPeriod = TimeSpan.Parse(sPeriod);
                                else
                                    tsPeriod = DefaulPeriod;

                                //if (tsPeriod == TimeSpan.Zero)
                                //    tsPeriod = DefaulPeriod;
                            }
                        }
                    }
                    //Console.WriteLine("tsPeriod=" + tsPeriod.ToString());
                    */

                    return tsPeriod;
                }
            }

            public string Domain
            {
                get { return this.navigator.GetAttribute("Domain", ""); }
            }

            public int ChunkSize
            {
                get {
                    string num = this.navigator.GetAttribute("ChunkSize", "");

                    if (num == String.Empty)
                        return -1;



                    try
                    {
                        return int.Parse(num);
                    }
                    catch
                    {
                        throw new Exception("Failed to parse ChunkSize. Make sure the value is an integer.");
                    }
                }
            }

            public XPathNavigator Map
            {
                get 
                {
                    XPathNavigator nav = this.navigator.SelectSingleNode("./Map");

                    if (nav == null)
                        throw new Exception("No <Map/> element has been defined in the ObjectClass mapping.");

                    return nav;
                }
            }

            public string Name
            {
                get { return this.navigator.GetAttribute("Name", ""); }
            }

            public string FactoryType
            {
                get { return this.navigator.GetAttribute("FactoryType", ""); }
            }
        }

        public ObjectClassCollection ObjectClasses
        {
            get { return this.objClasses; } 
        }

        public class ObjectClassCollection : IEnumerable<ObjectClass>
        {
            XPathNavigator m_MyNavigator;

            public ObjectClassCollection(XPathNavigator MyNavigator)
            {
                m_MyNavigator = MyNavigator;
            }

            IEnumerator<ObjectClass> IEnumerable<ObjectClass>.GetEnumerator()
            {
                return new MyEnumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<ObjectClass>)this).GetEnumerator();
            }

            public class MyEnumerator : IEnumerator<ObjectClass>
            {
                //DataClassCollection m_MyCollection;

                XPathNavigator m_MyNavigator;
                XPathNodeIterator m_MyIterator;

                public MyEnumerator(ObjectClassCollection MyCollection)
                {
                    m_MyNavigator = MyCollection.m_MyNavigator;
                    m_MyIterator = m_MyNavigator.Select("./DataSource/ObjectClass");
                    //Console.WriteLine(m_MyIterator.Count );
                }

                void IEnumerator.Reset()
                {
                    m_MyIterator = m_MyNavigator.Select("");
                }

                bool IEnumerator.MoveNext()
                {
                    return m_MyIterator.MoveNext();
                }

                ObjectClass IEnumerator<ObjectClass>.Current
                {
                    get
                    {
                        return new ObjectClass(m_MyIterator.Current);
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return ((IEnumerator<ObjectClass>)this).Current;
                    }
                }

                public void Dispose() { }
            }
        }
    }
}
