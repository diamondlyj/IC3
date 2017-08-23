using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Services;

namespace MIX2.Acquisition
{
    public class ServiceConnection: BaseConnection, ITokenProvider
    {
        private object service;
        private Type serviceType;
        private ITokenProvider tokenProvider;
        private object token;
        private string method;
        private string url;
        private Assembly assembly;
        private int timeOut;

        public ServiceConnection(NetworkCredential Credential, string ConnectionString, Assembly Assembly )
        {
            this.assembly = Assembly;
            this.credential = Credential;
            Init(ConnectionString);
        }

        public ServiceConnection(NetworkCredential Credential, string ConnectionString, int Timeout, Assembly Assembly, ITokenProvider TokenProvider)
        {
            this.assembly = Assembly;
            this.credential = Credential;
            this.tokenProvider = TokenProvider;
            Init(ConnectionString);

            this.timeOut = Timeout;
            object[] args = { this.timeOut };
            this.serviceType.InvokeMember("Timeout", BindingFlags.SetProperty, null, this.service, args);
        }

        public void AppendCredentials()
        {
            object[] credArg = { this.credential };
            this.serviceType.InvokeMember("Credentials", BindingFlags.SetProperty, null, this.service, credArg);            
        }

        new public void ExtractArgs( string ConnectionString )
        {
            this.parameters = new Dictionary<string, object>();

            string[] args = ConnectionString.Split(';');

            //WebService ws = new WebService();

            for (int i = 0; i < args.Length; i++ )
            {
                if (args[i].Trim() != string.Empty)
                {
                    int off = args[i].IndexOf('=');

                    string arg = args[i].Substring(0, off).Trim();
                    string val = args[i].Substring(off + 1).Trim();

                    switch (arg.ToLower())
                    {
                        case "service type":
                            try
                            {
                                this.serviceType = this.assembly.GetType(val);
                            }
                            catch
                            {
                                throw new Exception("The referenced MIXLibrary does not contain the type " + val + ".");
                            }

                            break;


                        case "method":
                            this.method = val;
                            break;

                        case "url":
                            this.url = val;
                            break;

                        case "timeout":
                            this.timeOut = int.Parse(val);
                            break;

                        default:
                            string param = string.Empty;

                            switch (val)
                            {
                                case "[$Domain]":
                                    param = this.credential.Domain;
                                    break;

                                case "[$Password]":
                                    param = this.credential.Password;
                                    break;

                                case "[$Username]":
                                    param = this.credential.UserName;
                                    break;

                                default:
                                    param = val;
                                    break;
                            }

                            this.parameters.Add(arg, param);

                            break;
                    }
                }
            }

            if (this.serviceType == null)
                throw new Exception("The WebService connection does not define a service type in the connections string.");

            try
            {
                this.service = this.serviceType.InvokeMember(null, BindingFlags.CreateInstance, null, this.assembly, null);
            }
            catch
            {
                throw new Exception("Failed to create the WebService connection. Make sure the service type is correct in the connection string.");
            }
            
            object[] urlArg = { this.url };

            this.serviceType.InvokeMember("Url", BindingFlags.SetProperty, null, this.service, urlArg);
        }

        public object GetToken( object[] Parameters )
        {
            if (this.tokenProvider != null)
            {
                return this.tokenProvider.GetToken( Parameters );
            }
            else
            {
                if (Parameters != null)
                {
                    Parameters = ReplaceParameters( Parameters );
                }

                object token = new object();

                try
                {
                    token = this.serviceType.InvokeMember(this.method, BindingFlags.InvokeMethod, null, this.service, Parameters);
                }
                catch (TargetInvocationException exc)
                {
                    throw exc.GetBaseException();
                }

                this.token = token;

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
            ExtractArgs(ConnectionString);

            //this.timeOut = 180000;

            object[] args = { this.timeOut };
            this.serviceType.InvokeMember("Timeout", BindingFlags.SetProperty, null, this.service, args);
        }

        public object GetData( string Method, object[] Parameters)
        {
            if (Parameters != null)
                Parameters = ReplaceParameters(Parameters);

            try
            {
                return this.serviceType.InvokeMember(Method, BindingFlags.InvokeMethod, null, this.service, Parameters);
            }
            catch (TargetInvocationException exc)
            {
                throw exc.GetBaseException();
            }
        }


        public object Token
        {
            get { return this.token; }
        }
    }
}
