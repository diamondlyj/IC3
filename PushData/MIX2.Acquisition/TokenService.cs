using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MIX2.Acquisition
{
    public class TokenProvider: BaseSource, ITokenProvider
    {
        public TokenProvider( string Name, NetworkCredential NetworkCredential, MIX2.Acquisition.Configuration.Connection Connection, string AssemblyName ):
            base( Name, NetworkCredential, Connection, AssemblyName )
        {
        }

        public TokenProvider(string Name, NetworkCredential NetworkCredential, MIX2.Acquisition.Configuration.Connection Connection, string AssemblyName, ITokenProvider TokenProvider):
            base(Name, NetworkCredential, Connection, AssemblyName, TokenProvider)
        {
        }

        public object GetToken( object[] Parameters )
        {

            ITokenProvider prov = (ITokenProvider)this.Connection;
            return prov.GetToken( Parameters );
        }
    }
}
