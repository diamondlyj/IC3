using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace MIX2.Acquisition.LDAP
{
    public class Connection: BaseConnection 
    {
        public Connection( NetworkCredential credential, string connectionString )
        {
            this.credential = credential;
            ExtractArgs(connectionString);
        }
             
        public System.DirectoryServices.SearchResultCollection GetData( string filter )
        {

            if (!this.parameters.Keys.Contains("path"))
                throw new Exception("The LDAP root path is not defined in the configuration file.");

            Console.WriteLine(this.parameters["path"].ToString());

            System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry(this.parameters["path"].ToString(), this.credential.UserName, this.credential.Password);
            
            System.DirectoryServices.SearchResultCollection results;

            try
            {
                System.DirectoryServices.DirectorySearcher searcher = new System.DirectoryServices.DirectorySearcher(entry);
                searcher.Filter = filter;
                searcher.Sort.PropertyName = "cn";
                searcher.Sort.Direction = System.DirectoryServices.SortDirection.Ascending;


                 results = searcher.FindAll();

                //Console.WriteLine(results[0].GetDirectoryEntry());
            }
            finally
            {
                entry.Close();
                entry.Dispose();
            }

            return results;
        }
    }
}
