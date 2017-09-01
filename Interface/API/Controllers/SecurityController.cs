using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using AI.V2.BaseLibrary;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;

namespace API.Controllers
{
    public class SecurityController : ApiController
    {

        [HttpGet, Route("Token/Cache")]
        public IHttpActionResult GetCache()
        {
            //return Ok();
            // return Ok((Request.Headers.First(h => h.Key == "Token").Value.First()));
            Guid t = Guid.Parse(Request.Headers.First(h => h.Key == "Token").Value.First());
            //return Ok(t.ToString());
            SqlConnection conn = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["AIn.Userbase"].ConnectionString);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "select Cache from Token where GUID = @Token";
            cmd.Connection = conn;

            SqlParameter token = new SqlParameter("@Token", System.Data.SqlDbType.UniqueIdentifier);
            token.Value = t;
            cmd.Parameters.Add(token);

            string r = "";

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    r = reader[0].ToString();
                }
            }
            finally
            {
                conn.Close();
            }

            return Ok(JsonConvert.DeserializeObject<Filter>(r));
        }

   
        [HttpPost, Route("Token/Cache")]
        public IHttpActionResult StoreCache([FromBody] Filter cache)
        {
           // return Ok(JsonConvert.SerializeObject(cache));
            Security.AssertRole(Request.Headers, Security.Role.User);

            Guid t = new Guid(Request.Headers.First(h => h.Key == "Token").Value.First() );

            Guid g = Guid.NewGuid();

            SqlConnection conn = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["AIn.Userbase"].ConnectionString);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "declare @UserGUID uniqueidentifier;"
                + "set @UserGUID = (select UserGUID from Token where GUID=@Token);"
                + "insert into Token(GUID,UserGUID,Role,Validated,LifeSpan,Cache)"
                + "values(@GUID,@UserGUID,0,@DateTime,180,@Cache);";
            cmd.Connection = conn;

            SqlParameter c = new SqlParameter("@Cache", System.Data.SqlDbType.NVarChar);
            //c.Value = cache; 
            c.Value=JsonConvert.SerializeObject(cache);
            //c.Value = "Beijing GUoan";
            cmd.Parameters.Add(c);

            SqlParameter dt = new SqlParameter("@DateTime", System.Data.SqlDbType.DateTime);
            dt.Value = DateTime.UtcNow;
            cmd.Parameters.Add(dt);

            SqlParameter token = new SqlParameter("@Token", System.Data.SqlDbType.UniqueIdentifier);
            token.Value = t;
            cmd.Parameters.Add(token);

            SqlParameter guid = new SqlParameter("@GUID", System.Data.SqlDbType.UniqueIdentifier);
            guid.Value = g;
            cmd.Parameters.Add(guid);


            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }

            return Ok(g.ToString());
        }
    }
}
