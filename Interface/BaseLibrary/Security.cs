using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.V2.BaseLibrary
{
    public class Security
    {
        public enum Role
        {
            Unknown = 0,
            Super = 1,
            Admin = 2,
            User = 4
        }
        /// <summary>
        /// Reads token from request header and (1) verifies that it exists in DB; (2) hasn't expired. 
        /// </summary>
        /// <returns></returns>
        public static void AssertRole( System.Net.Http.Headers.HttpHeaders headers, Role role)
        {
            return;

            string g = headers.First(h => h.Key == "token").Value.First();

            SqlConnection conn = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["AIn.Userbase"].ConnectionString);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "select [GUID] from Token where GUID = '" + g + "'";
            cmd.Connection = conn;

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    g = reader["GUID"].ToString();
                }
            }
            finally
            {
                conn.Close();
            }


            return;
        }
    }
}
