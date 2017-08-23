using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Reflection;
//using System.Data.SqlClient;
using System.Xml.XPath;

using IntuitiveLabs.Data;
using MIX2.Data;

using System.Diagnostics;

namespace MIX2.Acquisition
{
    public class BaseSource
    {
        private Assembly assembly;
        private object connection;
        private string m_Name;
        private NetworkCredential m_NetworkCredential;
        private ITokenProvider tokenProvider;

        //private DataSourceSchema m_Schema;

        public BaseSource(string Name, NetworkCredential NetworkCredential, MIX2.Acquisition.Configuration.Connection Connection, string AssemblyName)
        {
            this.Init(Name, NetworkCredential, Connection, AssemblyName);
        }

        public BaseSource(string Name, NetworkCredential NetworkCredential, MIX2.Acquisition.Configuration.Connection Connection, string AssemblyName, ITokenProvider TokenProvider)
        {
            this.tokenProvider = TokenProvider;
            this.Init(Name, NetworkCredential, Connection, AssemblyName);
        }

        protected Assembly Assembly
        {
            get
            {
                return this.assembly;
            }
        }
     
        private object CreateConnection(MIX2.Acquisition.Configuration.Connection Connection)
        {
            int timeOut = Connection.Timeout;

            switch (Connection.Type)
            {
                case "Database":
                    return new DbConnection(Connection.Invariant, ModifyCredentials(Connection.ConnectionString), timeOut, this.tokenProvider );

                case "Directory":
                    timeOut = timeOut * 1000;
                    return new LDAP.Connection(this.m_NetworkCredential, Connection.ConnectionString);

                case "File":
                    return new FileConnection(Connection.ConnectionString, this.tokenProvider);

                case "WebRequest":
                    timeOut = timeOut * 1000;
                    return new WebConnection(this.m_NetworkCredential, Connection.ConnectionString, timeOut, this.tokenProvider);

                case "WebService":
                    timeOut = timeOut * 1000;
                    return new ServiceConnection(this.m_NetworkCredential, Connection.ConnectionString, timeOut, this.assembly, this.tokenProvider);

                case "Custom":
                    return new CustomConnection(this.m_NetworkCredential, Connection.ConnectionString);

                default:
                    throw new Exception("Unsupported connection type: " + Connection.Type );
            }
        }

        private void Init   (string Name, NetworkCredential NetworkCredential, MIX2.Acquisition.Configuration.Connection Connection, string AssemblyName)
        {
            this.m_Name = Name;

            this.m_NetworkCredential = NetworkCredential;

            string asmName = string.Empty;
            string type = string.Empty;

            if (AssemblyName == null || AssemblyName == string.Empty)
                AssemblyName = "MIX2.Acquisition.dll";

            try
            {
                this.assembly = Assembly.LoadFrom(AssemblyName);
            }
            catch
            {
                throw new Exception("Error loading assembly. Make sure the file " + AssemblyName + " exists.");
            }

            this.connection = CreateConnection(Connection);
        }

        private string ModifyCredentials(string ConnectionString )
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();

            builder.ConnectionString = ConnectionString;

            string UserID = this.m_NetworkCredential.UserName;
            
            if (this.m_NetworkCredential.Domain != "")
                UserID = this.m_NetworkCredential.Domain + "\\" + UserID;

            string Password = this.m_NetworkCredential.Password;

            builder.Remove("User ID");
            builder.Add("User ID", UserID);

            builder.Remove("Password");
            builder.Add("Password", Password);
            ConnectionString = builder.ConnectionString;

            return ConnectionString;
        }

        protected object Connection
        {
            get
            {
                return this.connection;
            }
        }

        public string Name
        {
            get { return m_Name; }
        }    
    }
}
