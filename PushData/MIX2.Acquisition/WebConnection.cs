using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MIX2.Acquisition
{
    public class WebConnection: ITokenProvider
    {
        private string contentType;
        private NetworkCredential credential;
        private Dictionary<string, object> parameters;
        private string url;
        private string requestMethod;
        private string query;
        private string token;
        private ITokenProvider tokenProvider;
        private int timeOut;


        public WebConnection( string ConnectionString )
        {
            Init(ConnectionString);
        }

        public WebConnection(NetworkCredential Credential, string ConnectionString)
        {
            this.credential = Credential;
            Init(ConnectionString);
        }

        public WebConnection(NetworkCredential Credential, string ConnectionString, int Timeout, ITokenProvider TokenProvider)
        {
            this.credential = Credential;
            this.tokenProvider = TokenProvider;
            Init(ConnectionString);
            this.timeOut = Timeout;

            if( this.tokenProvider != null )
                this.token = this.tokenProvider.GetToken( new object[0] ).ToString();
        }

        private string BuildUrl( object[] Parameters, out string content )
        {
            string url = ReplaceAllParams( this.url );

            string s = string.Empty;

            if (this.query != null)
                s = this.query;

            if( !string.IsNullOrEmpty(s) )
                s = ReplaceAllParams(s);

            if (Parameters.Length > 0)
            {
                bool skipFirst = false;

                if ( s == string.Empty || s.LastIndexOf("&") == s.Length - 1)
                    skipFirst = true;

                for (int i = 0; i < Parameters.Length; i++)
                {
                    if (i != 0 || !skipFirst)
                        s += "&";

                    Type t = Parameters[i].GetType();

                    if (t.FullName == "System.String")
                        s += ReplaceAllParams( (string)Parameters[i] );

                    else if (t.FullName == "System.Collections.DictionaryEntry")
                    {
                        System.Collections.DictionaryEntry entry = (System.Collections.DictionaryEntry)Parameters[i];

                        s += entry.Key.ToString() + "=" + ReplaceAllParams( entry.Value.ToString() ); 
                    }
                }
            }

            if (this.requestMethod == "GET")
            {
                if (url.IndexOf("?") == -1)
                    url += "?";
                else
                {
                    if (url.LastIndexOf("&") != url.Length - 1)
                        url += "&";
                }

                url += s;

                content = string.Empty;
            }
            else
                content = s;

            //Console.WriteLine( url );

            return url;
        }

        public void ExtractArgs( string ConnectionString )
        {
            this.parameters = new Dictionary<string, object>();

            string[] args = ConnectionString.Split(';');
            
            for (int i = 0; i < args.Length; i++ )
            {
                if (args[i].Trim() != string.Empty)
                {
                    int off = args[i].IndexOf('=');

                    string arg = args[i].Substring(0, off).Trim().ToLower();

                    string val = args[i].Substring(off + 1).Trim();

                    switch (arg)
                    {
                        case "url":
                            this.url = val;
                            break;

                        case "content type":
                            this.contentType = val;
                            break;

                        case "method":
                            this.requestMethod = val.ToUpper();
                            break;

                        case "query":
                            this.query = val;

                            break;

                        default:
                            this.parameters.Add(arg, val);

                            break;
                    }
                }
            }
        }

        public object GetToken(object[] Parameters)       
        {
            if (this.tokenProvider != null)
            {
                return this.tokenProvider.GetToken( Parameters );
            }
            else
            {
                string content = string.Empty;
                
                this.token = GetDataAsString( BuildUrl( Parameters, out content ), content );

                return token;
            }
        }

        public void Init( string ConnectionString )
        {
            /*
            ServicePointManager.ServerCertificateValidationCallback += delegate(object Sender, X509Certificate cert, X509Chain chain, System.Net.Security.SslPolicyErrors errors)
            {
                return true;
            };
             * */

            this.contentType = "application/x-www-form-urlencoded";
            this.requestMethod = "POST";
            this.timeOut = 180000;

            ExtractArgs(ConnectionString);
        }

        public System.IO.Stream GetData(object[] Parameters)
        {
            string content = string.Empty;
            return SendRequest(BuildUrl(Parameters, out content), content);
        }

        public System.IO.Stream GetRESTData(string path)
        {
            string content = string.Empty;

            string u = url;

            if (!url.EndsWith("/") && !path.EndsWith("/"))
                u += "/";

            u += path;
            
            return SendRequest(u, content);
        }

        private string ReplaceBaseParams(string s)
        {
            if (this.credential != null)
            {
                s = s.Replace("[$Username]", this.credential.UserName);
                s = s.Replace("[$Password]", this.credential.Password);
                s = s.Replace("[$Domain]", this.credential.Domain);
            }

            if( this.token != null )
                s = s.Replace("[$Token]", this.token);

            return s;
        }
        
        private string ReplaceAllParams(string s)
        {
            s = ReplaceBaseParams(s);
            s = ReplaceCustomParams(s);

            return s;
        }

        private string GetDataAsString(string url, string content)
        {
            System.IO.Stream stream = SendRequest(url, content);
            System.IO.StreamReader reader = new System.IO.StreamReader( stream );
            string response = reader.ReadToEnd();

            stream.Close();

            return response;
        }

        private System.IO.Stream SendRequest(string url, string content)
        {
            WebRequest req = HttpWebRequest.Create(url);
            req.Timeout = timeOut;

            if (this.credential != null)
            {
                req.Credentials = this.credential;
            }

            req.ContentType = this.contentType;
            req.Method = this.requestMethod;

            //Console.WriteLine(url);
            
            
            if (this.requestMethod == "POST")
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
                req.ContentLength = buffer.Length;

                System.IO.Stream reqStream = req.GetRequestStream();
                reqStream.Write(buffer, 0, buffer.Length);
                reqStream.Close();
            }
            

            System.Net.WebResponse resp = req.GetResponse();
            return resp.GetResponseStream();
        }

        protected string ReplaceCustomParams(string s)
        {
            foreach (string key in this.parameters.Keys)
                s = s.Replace("[$" + key + "]", this.parameters[key].ToString());

            return s;
        }
        public string RequestMethod
        {
            get { return this.requestMethod; }
            set
            {
                string val = value.ToUpper();

                if (!(val == "POST" || val == "GET"))
                    throw new Exception("A WebFactory must use POST or GET method: " + val);

                this.requestMethod = val;
            }
        }
        public string Url
        {
            get { return this.url; }
            set { this.url = value; }
        }

        public string Token
        {
            get { return this.token; }
        }
    }
}
