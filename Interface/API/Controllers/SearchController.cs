using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class SearchController : ApiController
    {
        [HttpPost, Route("{domain}/Search/{position?}/{count?}/{matchSubstrings?}")]
        public IHttpActionResult Search(string domain, [FromBody] AI.V2.BaseLibrary.Filter filter, int position = -1, int count = -1, bool matchSubstrings = false)
        {

            List<AI.V2.BaseLibrary.AIObject> res = new List<AI.V2.BaseLibrary.AIObject>();

            bool all = true;


            /*
            if (!string.IsNullOrEmpty(filter.Category) && filter.ToLower() != "all")
            {
                category = filter.Split(';');
            }
            */
            //return Ok(filter);

            // Add proper propert filter

            AI.V2.BaseLibrary.Cube.PropertyValue[] propertyValue = { };

            if (filter.Category.Length > 0 || filter.PropertyValue.Length > 0)
                all = false;


            string xmlFilter = AI.V2.BaseLibrary.Filter.CreateXmlFilter(filter.Category, filter.PropertyValue);


            //return Ok(filter);

            string connKey = "AIn.Cube";

            if (all)
                connKey = "AIn.Registry";

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[connKey].ConnectionString);

            SqlParameter domainParam = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
            domainParam.Value = domain;

            SqlParameter objectCountParam = new SqlParameter("@ObjectCount", System.Data.SqlDbType.BigInt);
            objectCountParam.Direction = System.Data.ParameterDirection.Output;

            SqlParameter filterParam = new SqlParameter("@Filter", System.Data.SqlDbType.Xml);
            filterParam.Value = xmlFilter;  //"<Filter><Category><Path>a</Path><Path>b</Path></Category></Filter>";

            SqlParameter substringsParam = new SqlParameter("@MatchSubstrings", System.Data.SqlDbType.Bit);
            substringsParam.Value = matchSubstrings;

            SqlParameter positionParam = new SqlParameter("@Position", System.Data.SqlDbType.BigInt);
            positionParam.Value = position;

            SqlParameter countParam = new SqlParameter("@Count", System.Data.SqlDbType.BigInt);
            countParam.Value = count;

            conn.Open();

            List<Object> objects = new List<Object>();

            string spName = "Domain_Search";

            if (all)
                spName = "Domain_GetObjects";

            //SearchResult result = new SearchResult();

            using (SqlCommand com = new SqlCommand(spName, conn))
            {
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.Parameters.Add(domainParam);

                if (!all)
                {
                    com.Parameters.Add(filterParam);
                    com.Parameters.Add(substringsParam);
                }

                com.Parameters.Add(positionParam);
                com.Parameters.Add(countParam);
                com.Parameters.Add(objectCountParam);

                SqlDataReader reader = com.ExecuteReader();


                while (reader.Read())
                {
                    /*
                    Object item = new Object() { Domain = domain, ObjectClass = reader.GetString(0), ObjectGUID = reader.GetGuid(1) };

                    item = item.AppendObjectData(this.context, domain, item.ObjectClass, item.ObjectGUID);
                    objects.Add(item);
                    

                    Console.WriteLine(item.ObjectGUID);
                    */
                    AI.V2.BaseLibrary.AIObject obj = new AI.V2.BaseLibrary.AIObject(reader[1].ToString());
                    obj.ObjectClass = reader[0].ToString();

                    AI.V2.BaseLibrary.Property p = new AI.V2.BaseLibrary.Property("FriendlyName");
                    AI.V2.BaseLibrary.Value v = new AI.V2.BaseLibrary.Value(p, reader[2].ToString());
                    p.Value.Add(v.Val.ToString());
                    obj.Meta.Properties.Add(p);

                    res.Add(obj);
                }

                reader.Close();

                /*
                if (objectCountParam.Value != DBNull.Value)
                    result.Count = (long)objectCountParam.Value;
               `*/
            }

            conn.Close();

            //result.Objects = objects.ToArray();

            return Ok(res);
        }

    }
}

