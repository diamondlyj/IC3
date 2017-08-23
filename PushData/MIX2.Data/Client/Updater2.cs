using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using IntuitiveLabs.Security;

namespace MIX2.Data.Client
{
    public class UpdaterException : ApplicationException
    {
        Uri WebServiceLocation;

        public UpdaterException(string Message, Exception InnerException, Uri WebServiceLocation)
            :
            base(Message, InnerException)
        {
            this.WebServiceLocation = WebServiceLocation;
        }

    }

    public delegate void ObjectUpdateSucceededDelegat(LocalObject obj);
    public delegate void ObjectUpdateFailedDelegat(LocalObject obj);
    public delegate void TimeoutUpperLimitDelegate(int Timeout);
    public delegate void TimeoutBackToNormDelegate(int Timeout);
    public delegate void NonTimeoutExceptionDelegate(LocalObject obj, Exception ex);

    public class Updater
    {
        int m_MinTimeout;
        int m_Timeout;
        int m_MaxTimeout;
        int m_Attempts;

        //private MIX2.Recognition.WebService.Recognition wsRecognition;
        private ServiceProxies.Recognition.RecognitionClient  svcRecognitionClient;

        private IntuitiveLabs.Security.RSAProvider provider;
        private string sourceGUID;
        private string token;

        public Updater(Uri urlWebServiceLocation, int MinTimeout /* ms */, int MaxTimeout /* ms */, int Attempts )
        {
            m_MinTimeout = MinTimeout;
            m_MaxTimeout = MaxTimeout;

            m_Timeout = m_MinTimeout;

            m_Attempts = Attempts;

            try
            {
                this.svcRecognitionClient = new ServiceProxies.Recognition.RecognitionClient();

                //wsRecognition = new MIX2.Recognition.WebService.Recognition();                
                //wsRecognition.Url = urlWebServiceLocation.AbsoluteUri;
            }
            catch(Exception ex)
            {
                throw new UpdaterException("Updater: error when trying to instantiate the object", ex, urlWebServiceLocation);
            }
        }

        public System.Net.NetworkCredential Credential
        {            
            set
            {
                svcRecognitionClient.ChannelFactory.Credentials.Windows.ClientCredential = value;                 
                //wsRecognition.Credentials = value; 
            }
        }

        public IntuitiveLabs.Security.RSAProvider SigningProvider
        {
            set { this.provider = value; }
        }

        public event ObjectUpdateSucceededDelegat ObjectUpdateSucceeded;
        public event ObjectUpdateFailedDelegat ObjectUpdateFailed;
        public event TimeoutUpperLimitDelegate TimeoutUpperLimit;
        public event TimeoutBackToNormDelegate TimeoutBackToNorm;
        public event NonTimeoutExceptionDelegate NonTimeoutException;

        protected bool SetTimeout(int NewTimeout)
        {
            bool UpperLimit = false;

            if (m_MaxTimeout <= NewTimeout)
            {
                if (m_Timeout < m_MaxTimeout && this.TimeoutUpperLimit != null)
                {
                    //  only if timeout reached upperlimit from lower value
                    UpperLimit = true;
                    this.TimeoutUpperLimit(m_MaxTimeout);
                }
                    
                m_Timeout = m_MaxTimeout;
            }
            else 
            {
                if( NewTimeout <= m_MinTimeout )
                    NewTimeout = m_MinTimeout;

                if( m_Timeout >= m_MaxTimeout && this.TimeoutBackToNorm != null)
                {
                    // only when timeout decreased from the max value
                    this.TimeoutBackToNorm(NewTimeout);
                }
                m_Timeout = NewTimeout;
                
            }
            return UpperLimit;
        }

        public string SourceGUID
        {
            set { this.sourceGUID = value; }
        }

        public string Token
        {
            set { this.token = value; }
        }

        public void TrySend(int Attempts,  LocalObject obj)
        {
            if (Attempts > 0)
            {
                try
                {
                    //wsRecognition.Timeout = m_Timeout;  // ms 
                    //svcRecognitionClient.Endpoint.Binding.ReceiveTimeout = new TimeSpan(m_Timeout);
                    DateTime dtStart = DateTime.Now;

                    Console.Write(".");
                    Console.WriteLine(obj.GetXml());
                    //string s = wsRecognition.SendObject( obj.GetXml(), this.token, this.sourceGUID );
                    string s = this.svcRecognitionClient.SendObject(obj.GetXml(), this.token, this.sourceGUID);

                    Console.WriteLine(" Response:" + s);

                    TimeSpan ts = DateTime.Now.Subtract(dtStart);
                    
                    SetTimeout(ts.Milliseconds);
                    this.ObjectUpdateSucceeded(obj);
                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("timed out") != -1 )
                    {
                        if (SetTimeout(m_Timeout*2) )
                        {
                            TrySend(Attempts, obj);
                        }
                        else
                        {
                            TrySend(Attempts - 1, obj);
                        }
                    }
                    else
                    {
                        if (this.NonTimeoutException != null)
                            this.NonTimeoutException(obj, ex);
                        TrySend(Attempts - 1, obj);
                    }
                }
            }
            else
            {
                this.ObjectUpdateFailed(obj);
            }
        }


        //       public string UpdateAttribute(string DataSource, string ObjectClass, string DataClass, string Attribute, string Obj, string Instance, string Value, System.DateTime Updated) {
        public string SendObject(LocalObject obj)
        {
            Console.Write("Trying to send");
            string Result = "OK";
            TrySend(m_Attempts, obj);
            return Result;
        }

        public XmlDocument RegisterSource( string SourceName, string Salt )
        {
            RSAProvider provider = RSAProvider.CreateFromName(SourceName + Salt);

            XmlDocument resDoc = new XmlDocument();
            //resDoc.LoadXml(wsRecognition.RegisterSource(SourceName, provider.PublicKey).OuterXml);
            string xml = svcRecognitionClient.RegisterSource(SourceName, provider.PublicKey);

            Console.Out.WriteLine(xml);

            resDoc.LoadXml( xml );

            string rijnKey = resDoc.DocumentElement.SelectSingleNode("./Key").InnerText;
            string rijnIV = resDoc.DocumentElement.SelectSingleNode("./IV").InnerText ;
            string content = resDoc.DocumentElement.SelectSingleNode("./Content").InnerText;

            RijndaelProvider rijnProvider = new RijndaelProvider(provider.DecryptFromString(rijnKey), provider.DecryptFromString(rijnIV));
            
            XmlDocument contentDoc = new XmlDocument();
            contentDoc.LoadXml( rijnProvider.Decrypt(content) );

            this.provider = RSAProvider.CreateFromKey(contentDoc.DocumentElement.SelectSingleNode("Key").InnerXml);
            this.sourceGUID = contentDoc.DocumentElement.SelectSingleNode("SourceGUID").InnerXml;
            this.token = this.provider.EncryptToString(contentDoc.DocumentElement.SelectSingleNode("Token").InnerXml);

            return contentDoc;
        }

    }
}
