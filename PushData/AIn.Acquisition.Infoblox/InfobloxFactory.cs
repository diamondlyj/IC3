

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MIX2.Acquisition;
using System.Data;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Specialized;
using System.Xml;


namespace AIn.Acquisition.Infoblox
{


    public class InfobloxFactory : IMIXFactory
    {
        private WebConnection connection;
        private bool eod = false;
        string nextPath = string.Empty;
        private string source;
        int timeOut = 18880;
        DataSourceSchema.ObjectClass currentClass;
        int chunkSize;
        int chunkCount;
        MIX2.Acquisition.MapSchema map;
        bool hasParent;
        string pageID;
        int count;
       string currentPageID;
        int maxPages;
        string username = string.Empty;
        string password = string.Empty;
        public XmlFileClass xml;
        private string sa;
        public List<XmlFileClass> list = new List<XmlFileClass>();
        IEnumerator<XElement> idEnumerator;
        private string sourceName;

        public InfobloxFactory(string source, object connection)
        {
            this.connection = (WebConnection)connection;
            this.source = source;
            this.connection.RequestMethod = "GET";
            this.chunkSize = -1;
            this.hasParent = false;
            //this.eod = true;
            this.count = 0;

        }

        public bool IsAlive(out string mes)
        {
            Console.WriteLine("Connecting to " + this.source);

            try
            {
                System.IO.Stream stream = this.connection.GetRESTData("grid");
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                mes = reader.ReadToEnd();
                return true;
            }
            catch (Exception exc)
            {
                mes = exc.Message;
                return false;
            }
        }

        public void getData(InfobloxFactory factory)
        {
            getData();
            //throw new NotImplementedException();
        }

        public void Open(DataSourceSchema.ObjectClass objectClass)
        {
            this.currentClass = objectClass;
            this.chunkSize = objectClass.ChunkSize;
            this.chunkCount = 0;
            this.map = new MapSchema(objectClass.Map);


            string path = "network?_return_type=xml&_return_as_object=1&_max_results=1000&_paging=1";

            string str = string.Empty;

            try
            {
                System.IO.Stream stream = this.connection.GetRESTData(path);
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                str = reader.ReadToEnd();
                //Console.WriteLine(str);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
                string next_page_id = "";
                Console.WriteLine("Load from Server");
                XmlDocument result = new System.Xml.XmlDocument();
                result.LoadXml(str);
                XmlElement root = null;
                root = result.DocumentElement;
                xml = new XmlFileClass();
                XmlNodeList listNodes = null;
                listNodes = root.SelectNodes("/value/next_page_id");
                foreach (XmlNode node in listNodes)
                {
                    next_page_id = node.InnerText;
                }

                listNodes = null;
                listNodes = root.SelectNodes("/value/result/list/value");
                foreach (XmlNode node in listNodes)
                {
                    try
                    {
                        //   XmlNodeList itemNodes = node.ChildNodes[].InnerText;
                        string comment = node["comment"] != null ? node["comment"].InnerText : "";
                        string network = node["network"] != null ? node["network"].InnerText : "";
                        string network_view = node["network_view"] != null ? node["network_view"].InnerText : "";
                        string _ref = node["_ref"] != null ? node["_ref"].InnerText : "";
                        valueObject value = new valueObject(comment, network, network_view, _ref);
                        xml.list.Add(value);
                    }
                    catch
                    {
                        Console.WriteLine(node.InnerXml);
                    }
                }
                //list.Add(xml);
                Console.WriteLine("First" + listNodes.Count + "object");
                if (String.IsNullOrEmpty(next_page_id))
                {
                    this.eod = true;
                }
                else
                {
                    string next = string.Format("&_page_id={0}", next_page_id);
                    nextPath = path + next;
                    //System.IO.Stream stream = this.connection.GetRESTData(nextPath);
                    //System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                    //str = reader.ReadToEnd();
                    //Console.WriteLine(str);
                    //getNextChunk();
                    //getChunk();
                }
        }

        public void Close()
        {
            return;
        }

        public bool EOD
        {
            get
            {
                return this.eod;
            }
        }
        
        public MIX2.Data.LocalObject GetNext()
        {
            return new MIX2.Data.LocalObject();
        }
        

        public void GetNextData()
        {

            string path = "network?_return_type=xml&_return_as_object=1&_max_results=1000&_paging=1";
            string str = string.Empty;

            try
            {
                System.IO.Stream stream = this.connection.GetRESTData(nextPath);
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                str = reader.ReadToEnd();
                //Console.WriteLine(str);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            string next_page_id = "";
            Console.WriteLine("Load from Server");
            XmlDocument result = new System.Xml.XmlDocument();
            result.LoadXml(str);
            XmlElement root = null;
            root = result.DocumentElement;
            xml = new XmlFileClass();
            XmlNodeList listNodes = null;
            listNodes = root.SelectNodes("/value/next_page_id");
            foreach (XmlNode node in listNodes)
            {
                next_page_id = node.InnerText;
            }

            listNodes = null;
            listNodes = root.SelectNodes("/value/result/list/value");
            foreach (XmlNode node in listNodes)
            {
                try
                {
                    //   XmlNodeList itemNodes = node.ChildNodes[].InnerText;
                    string comment = node["comment"] != null ? node["comment"].InnerText : "";
                    string network = node["network"] != null ? node["network"].InnerText : "";
                    string network_view = node["network_view"] != null ? node["network_view"].InnerText : "";
                    string _ref = node["_ref"] != null ? node["_ref"].InnerText : "";
                    valueObject value = new valueObject(comment, network, network_view, _ref);
                    xml.list.Add(value);
                }
                catch
                {
                    Console.WriteLine(node.InnerXml);
                }
            }
            //list.Add(xml);
            Console.WriteLine("First" + listNodes.Count + "object");
            if (String.IsNullOrEmpty(next_page_id))
            {
                this.eod = true;
            }
            else
            {
                string next = string.Format("&_page_id={0}", next_page_id);
                nextPath = path + next;
                //System.IO.Stream stream = this.connection.GetRESTData(nextPath);
                //System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                //str = reader.ReadToEnd();
                //Console.WriteLine(str);
                //getNextChunk();
                //getChunk();
            }
        }

        /*
        public string GetHttpWebRequest(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(uri);
                myReq.Credentials = new System.Net.NetworkCredential();
                myReq.ContentType = "application/json";
                myReq.Method = "GET";
                HttpWebResponse result = (HttpWebResponse)myReq.GetResponse();
                Stream receviceStream = result.GetResponseStream();
                StreamReader readerOfStream = new StreamReader(receviceStream);
                string strHTML = readerOfStream.ReadToEnd();
                Console.WriteLine("Load from server");
                
                readerOfStream.Close();
                receviceStream.Close();
                return strHTML;
            }
            catch (Exception ex)
            {
                throw new Exception("error" + ex.Message);
            }
        }
        */
        private void getChunk()
        {

        }

        private void getNextChunk()
        {
            this.pageID = this.idEnumerator.Current.Value;

            int n = 0;

            while (n < this.chunkSize)
            {
                this.currentPageID = idEnumerator.Current.Value;

                if (!this.idEnumerator.MoveNext())
                    n = this.chunkSize;
                else
                    n++;
            }

            return;
        }
        public void getData()

        {
            // List<XmlFileClass> list = new List<XmlFileClass>();

            string path = "network?_return_type=xml&_return_as_object=1&_max_results=1000&_paging=1";

            string nextPath = path;
            string str = string.Empty;
            try
            {
                System.IO.Stream stream = this.connection.GetRESTData(path);
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                str = reader.ReadToEnd();
                //Console.WriteLine(str);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            
            while (true)
            {
                string next_page_id = "";
                //string str = GetHttpWebRequest(nextUrl);
                Console.WriteLine("Load from Server");
                XmlDocument result = new System.Xml.XmlDocument();
                result.LoadXml(str);
                XmlElement root = null;
                root = result.DocumentElement;
                xml = new XmlFileClass();
                XmlNodeList listNodes = null;
                listNodes = root.SelectNodes("/value/next_page_id");
                foreach (XmlNode node in listNodes)
                {
                    next_page_id = node.InnerText;
                }

                listNodes = null;
                listNodes = root.SelectNodes("/value/result/list/value");
                foreach (XmlNode node in listNodes)
                {
                    try
                    {
                        //   XmlNodeList itemNodes = node.ChildNodes[].InnerText;
                        string comment = node["comment"] != null ? node["comment"].InnerText : "";
                        string network = node["network"] != null ? node["network"].InnerText : "";
                        string network_view = node["network_view"] != null ? node["network_view"].InnerText : "";
                        string _ref = node["_ref"] != null ? node["_ref"].InnerText : "";
                        valueObject value = new valueObject(comment, network, network_view, _ref);
                        xml.list.Add(value);
                    }
                    catch
                    {
                        Console.WriteLine(node.InnerXml);
                    }
                }
                list.Add(xml);
                Console.WriteLine("First" + listNodes.Count + "object");
                if (String.IsNullOrEmpty(next_page_id))
                    break;
                else
                {
                    string next = string.Format("&_page_id={0}", next_page_id);
                    nextPath = path + next;
                    System.IO.Stream stream = this.connection.GetRESTData(nextPath);
                    System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                    str = reader.ReadToEnd();
                    Console.WriteLine(str);
                   
                }
            }
            
        }
    }
}
