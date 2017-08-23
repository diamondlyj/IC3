using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIX2.Acquisition
{
    public class CustomConnection: BaseConnection
    {        
        public CustomConnection( System.Net.NetworkCredential credential, string ConnectionString )
        {
            if (credential == null)
                this.credential = System.Net.CredentialCache.DefaultNetworkCredentials;
            else
                this.credential = credential;

            this.ExtractArgs(ConnectionString);
        }

        public System.Net.NetworkCredential Credential
        {
            get { return this.credential; }
        }

        public Dictionary<string, object> Parameters
        {
            get { return this.parameters; }
        }
    }
}
