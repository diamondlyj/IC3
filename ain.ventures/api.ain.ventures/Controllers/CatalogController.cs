using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace api.ain.ventures.Controllers
{
    public class CatalogController : ApiController
    {
        [HttpGet, Route("Catalog")]
        public IHttpActionResult GetCatalog()
        {
            AIn.Ventures.BaseLibrary.AInVentureEntities ent = new AIn.Ventures.BaseLibrary.AInVentureEntities();

            var cmd = ent.Database.Connection.CreateCommand();
            cmd.CommandText = "GetCategories";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            AIn.Ventures.Models.Catalog catalog = new AIn.Ventures.Models.Catalog();

            AIn.Ventures.Models.Category c = new AIn.Ventures.Models.Category("Catalog");

            int d = 0;

            List<AIn.Ventures.Models.Category> cats = new List<AIn.Ventures.Models.Category>();
            cats.Add(new AIn.Ventures.Models.Category(string.Empty));

            try
            {
                ent.Database.Connection.Open();
                DbDataReader reader = cmd.ExecuteReader();

                while( reader.Read() )
                {
                    string[] vals = reader["Value"].ToString().Split('.');
                    
                    if(vals.Length > d)
                    {
                        d = vals.Length;
                    }

                    // Check if the root category is new.
                    if(cats[0].Name != vals[0])
                    {
                        //If it is new create a category and add it to the catalog.
                        AIn.Ventures.Models.Category cat = new AIn.Ventures.Models.Category(vals[0]);
                        cat.Value = vals[0];
                        c.Categories.Add(cat);

                        //Replace current category with new one.
                        cats[0] = cat;
                    }

                    for (int i = 1; i < vals.Length; i++)
                    {
                        int n = cats.Count();

                        // Check if the child category is new.
                        if (n <= i || cats[i].Name != vals[i])
                        {
                            //If it is new, create a new child and add it to the parent.
                            AIn.Ventures.Models.Category cat = new AIn.Ventures.Models.Category(vals[i]);

                            string val = vals[i];

                            for(int j=i-1; j>=0;j--)
                            {
                                val = vals[j] + "." + val;
                            }

                            cat.Value = val;
                            cats[i - 1].Categories.Add(cat);

                            //If the depth for storing the current categories is not enough, increase it.
                            if(n <= i)
                            {
                                cats.Add(cat);
                            }
                            else
                            {
                                cats[i] = cat;
                            }
                        }
                    }
                }
            }
            finally
            {
                ent.Database.Connection.Close();
            }


            return Ok(c);
        }

        /*
        private void AddCategory( ref List<AIn.Ventures.Models.Category> cats, ref string[] vals, int index)
        {
            AIn.Ventures.Models.Category c = new AIn.Ventures.Models.Category(vals[index]);
            cats.Add(c);
            AddCategory( ref );
        }
        */

    }
}
