using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIX2.Acquisition
{
    public class FileConnection: ITokenProvider
    {
        private ITokenProvider tokenProvider;
        private string filePath;

        public FileConnection( string ProviderName, string ConnectionString, int TimeOut )
        {
        }

        public FileConnection(string ConnectionString, ITokenProvider TokenProvider)
        {
            ExtractArgs(ConnectionString);
            this.tokenProvider = TokenProvider;
        }

        public object GetToken(object[] Parameters)
        {
            if (this.tokenProvider != null)
                return this.tokenProvider.GetToken(Parameters);
            else
                return null;
        }

        
        public void ExtractArgs(string ConnectionString)
        {
            string[] args = ConnectionString.Split(';');

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Trim() != string.Empty)
                {
                    int off = args[i].IndexOf('=');

                    string arg = args[i].Substring(0, off).Trim().ToLower();

                    string val = args[i].Substring(off + 1).Trim();

                    switch (arg)
                    {
                        case "path":
                            this.filePath = val;
                            break;

                        default:
                            throw new Exception("Unsopported argument was passed to FileConnection: " + arg);
                    }
                }
            }
        }

        public string FilePath
        {
            get { return this.filePath; }
        }
    }
}
