using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data.SqlClient;
using AI.V2.BaseLibrary;
using System.Runtime.Serialization;
using System.Data;

namespace API.Controllers
{
    public class CubeController : ApiController
    {
        // GET: Cube
        [HttpGet, Route()]
        public IHttpActionResult GetCategoryInfo(Guid tokenValue, string domain, string categoryPath, string[] category, PropertyValue[] propertyValue, bool matchSubstrings)
        {
            bool all = true;

            if (category.Length > 0 || propertyValue.Length > 0)
                all = false;

            string filter = CreateFilter(category, propertyValue);

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Cube"].ConnectionString);

            Category cat = new Category();
            cat.Path = categoryPath;

            SqlParameter domainParam = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
            domainParam.Value = domain;

            SqlParameter pathParam = new SqlParameter("@CategoryPath", System.Data.SqlDbType.NVarChar, 256);
            pathParam.Value = categoryPath;

            SqlParameter filterParam = new SqlParameter("@Filter", System.Data.SqlDbType.Xml);
            filterParam.Value = filter;

            SqlParameter substringsParam = new SqlParameter("@MatchSubstrings", System.Data.SqlDbType.Bit);
            substringsParam.Value = matchSubstrings;
            conn.Open();

            using (SqlCommand com = new SqlCommand("Category_GetChildInfo", conn))
            {
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.Parameters.Add(domainParam);
                com.Parameters.Add(pathParam);
                com.Parameters.Add(filterParam);
                com.Parameters.Add(substringsParam);

                SqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    Category child = new Category();
                    child.Path = (string)reader[0];
                    child.ObjectCount = (long)reader[1];

                    cat.Children.Add(child);
                }

                reader.Close();
            }

            conn.Close();

            return Ok(cat);
        }

        [HttpGet, Route()]
        public IHttpActionResult GetCube(Guid tokenValue, string domain, string orderBy, AI.V2.BaseLibrary.Direction direction)
        {
            Cube cube = new Cube(domain);

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Cube"].ConnectionString);

            SqlParameter domainParam = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
            domainParam.Value = domain;

            conn.Open();

            using (SqlCommand com = new SqlCommand("Domain_GetCategories", conn))
            {
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.Parameters.Add(domainParam);

                Console.WriteLine(domainParam.Value);

                SqlDataReader reader = com.ExecuteReader();

                // cube.AddCatalogs(reader);

                reader.Close();
            }

            conn.Close();
            return Ok(cube);
        }
        [HttpGet, Route()]
        public IHttpActionResult GetDynamicCube(Guid tokenValue, string[] category, PropertyValue[] propertyValue, bool matchSubstrings, string orderBy, AI.V2.BaseLibrary.Direction direction)
        {
            string domain = "IPNetworking";

            bool all = true;

            if (category.Length > 0 || propertyValue.Length > 0)
                all = false;

            string filter = CreateFilter(category, propertyValue);

            Cube cube = new Cube(domain);
            cube.IsDynamic = true;

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Cube"].ConnectionString);

            SqlParameter domainParam = new SqlParameter("@Domain", System.Data.SqlDbType.NVarChar, 64);
            domainParam.Value = domain;

            conn.Open();

            using (SqlCommand com = new SqlCommand("Domain_Search_GetPropertyValues", conn))
            {
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.Parameters.Add(domainParam);

                SqlParameter filterParam = new SqlParameter("@Filter", System.Data.SqlDbType.Xml);
                filterParam.Value = filter;  //"<Filter><Category><Path>a</Path><Path>b</Path></Category></Filter>";
                com.Parameters.Add(filterParam);

                SqlParameter substringsParam = new SqlParameter("@MatchSubstrings", System.Data.SqlDbType.Bit);
                substringsParam.Value = matchSubstrings;
                com.Parameters.Add(substringsParam);

                //Console.WriteLine(domainParam.Value);

                SqlDataReader reader = com.ExecuteReader();

                string currentClass = string.Empty;

                if (reader.Read())
                {
                    string className = reader.GetString(0);

                    while (className != string.Empty)
                    {
                        currentClass = className;

                        Category cat = new Category();
                        cat.Name = className;
                        cat.Path = className;
                        cat.Depth = 0;

                        cube.Catalogs.Add(cat);

                        while (className == currentClass)
                        {
                            Category prop = new Category();
                            string propName = reader.GetString(1);
                            string currentProp = propName;

                            prop.Name = propName;
                            prop.Path = className + "." + propName;
                            prop.Depth = 1;

                            cat.Children.Add(prop);

                            /* Temporary until caching is fixed */

                            if (reader.Read())
                            {
                                className = reader.GetString(0);
                            }
                            else
                            {
                                className = string.Empty;
                            }


                            while (className == currentClass && propName == currentProp)
                            {
                                Category val = new Category();

                                val.Name = reader.GetString(2);

                                if (val.Name.Length > 64)
                                    val.Name = val.Name.Substring(0, 64);

                                val.Path = className + "." + propName + val.Name;
                                prop.Depth = 2;

                                prop.Children.Add(val);

                                if (reader.Read())
                                {
                                    className = reader.GetString(0);
                                    propName = reader.GetString(1);
                                }
                                else
                                {
                                    className = string.Empty;
                                }
                            }

                        }
                    }
                }

                reader.Close();
            }

            conn.Close();

            return Ok(cube);
        }
        [HttpGet, Route()]
        public IHttpActionResult Search(Guid tokenValue, string domain, string[] category, PropertyValue[] propertyValue, bool matchSubstrings, long position, long count, string orderBy, AI.V2.BaseLibrary.Direction direction)
        {

            return Ok();
        }

        private string CreateFilter(string[] category, PropertyValue[] propertyValue)
        {
            return null;
        }
        private void AddCatalogs(SqlDataReader reader)
        {
            if (reader.Read())
            {
                Next:
                string name = reader.GetString(1);

                Console.WriteLine("catalog.name=" + name);

                Category cat = new Category();
                cat.Name = name;
                cat.Path = name;
                cat.Depth = 0;

                //this.catalogs.Add(cat);

                //if (!cat.AddChildren(reader))
                //{
                //    Console.WriteLine("time to add the next catalog:" + reader.GetString(1));
                //    goto Next;
                //}
            }
        }
    }
}