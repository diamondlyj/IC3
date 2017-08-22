using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AIn.Ventures.BaseLibrary;
using Newtonsoft.Json;
using System.Web;
using System.Data.SqlClient;

namespace api.ain.ventures.Controllers
{
    public class ComponentsController : ApiController
    {
         AIn.Ventures.Models.Component RootComponent;
        List<AIn.Ventures.Models.Component> UnprocessedComponents;

        [HttpPost, Route("Component/{ParentGUID}/{GUID}/{AmountInParent}")]
        //[Authorize]
        public IHttpActionResult CreateComponent(Guid ParentGUID,Guid GUID, int AmountInParent)
        {
            //return Ok();
            
            AIn.Ventures.Models.Component componentFromQpoint = new AIn.Ventures.Models.Component();

            //Guid guid = Guid.NewGuid();
            
            AInVentureEntities ent = new AInVentureEntities();
            
            var cmd = ent.Database.Connection.CreateCommand();

            Guid nguid = Guid.NewGuid();

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "CreateComponentFromQpoints";
           
            cmd.Parameters.Add(new SqlParameter("@GUID", nguid));
            cmd.Parameters.Add(new SqlParameter("@QPointGUID", GUID));
            cmd.Parameters.Add(new SqlParameter("@ParentGUID", ParentGUID));
            cmd.Parameters.Add(new SqlParameter("@AmountInParent", AmountInParent));

            try
            {
                ent.Database.Connection.Open();
                //return Ok(componentFromQpoint);
                cmd.ExecuteNonQuery();
           
            }
            finally
            {
                ent.Database.Connection.Close();
            }

            // Guid u = new Guid(((System.Security.Claims.ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == "GUID").Value);



           return Ok(nguid);            
        }

        [HttpPost, Route("Module")]
        //[Authorize]
        public IHttpActionResult CreateModule([FromBody]AIn.Ventures.Models.Component cp)
        {
            Guid guid = Guid.NewGuid();

            AIn.Ventures.BaseLibrary.AInVentureEntities ent = new AIn.Ventures.BaseLibrary.AInVentureEntities();

            //Guid u = new Guid(((System.Security.Claims.ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == "GUID").Value);

            AIn.Ventures.Models.Component root = new AIn.Ventures.Models.Component();

            cp.GUID = Guid.NewGuid();
            var cmd = ent.Database.Connection.CreateCommand();

            cmd.Parameters.Add(new SqlParameter("@ObjectGUID", cp.GUID));
            cmd.Parameters.Add(new SqlParameter("@ComponentType", "Module"));
            cmd.Parameters.Add(new SqlParameter("@Name", cp.Name));
            cmd.Parameters.Add(new SqlParameter("@Description", cp.Description));
            cmd.Parameters.Add(new SqlParameter("@ParentGUID", cp.ParentGUID));
            cmd.Parameters.Add(new SqlParameter("@ProjectGUID", cp.ProjectGUID));
            cmd.Parameters.Add(new SqlParameter("@AmountInParent", cp.AmountInParent));
            
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "CreateComponent";

            try
            {
                ent.Database.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                ent.Database.Connection.Close();
            }

            //ent.CreateComponent(guid, cp.ComponentType, cp.Name,cp.Description, null, ParentGUID,cp.AmountInParent);

            //ent.InsertComponentToComponent(ParentGUID,guid,100);

            return Ok(cp);
        }

        [HttpGet, Route("Module/{GUID}")]
        [AllowAnonymous]
        // [Authorize]
        // [AIn.Ventures.BaseLibrary.ActionRightsFilter(RequiredRight = Right.View, SecuredEntity = Entity.Component)]
        public IHttpActionResult GetComponents(Guid GUID)
        {
            AIn.Ventures.Models.Component root = new AIn.Ventures.Models.Component();

            AIn.Ventures.BaseLibrary.AInVentureEntities ent = new AIn.Ventures.BaseLibrary.AInVentureEntities();

            var cmd = ent.Database.Connection.CreateCommand();

            cmd.Parameters.Add(new SqlParameter("@ObjectGUID", GUID));

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetComponents_ByObjectGUID";


            int n = 0;

            try
            {
                ent.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // First we find out the number of result so we can initalize an array of fized length
                    // for storing referencing components sequenially (to figure out their hierarchical relationship).

                    // This is done by the stored procedure return a single row single column table
                    // instead of an output parameter because the value of the output would only be aalaible after 
                    // all records are read. We want to know the length of the "stream" prior to reading it.

                    int count = (int)reader[0];

                    AIn.Ventures.Models.Component[] comps = new AIn.Ventures.Models.Component[count];


                    if (reader.NextResult())
                    {
                        // Go through the components returned and build their hierarchical relationship.

                        while (reader.Read())
                        {

                            AIn.Ventures.Models.Component c = new AIn.Ventures.Models.Component()
                            {
                                GUID = (Guid)reader["ObjectGUID"],
                                ParentGUID = (Guid)reader["ParentGUID"],
                                AmountInParent = (int)reader["amount"],
                                ComponentType = reader["ComponentType"].ToString(),
                                Name = reader["Name"].ToString(),
                                Supplier = reader["Supplier"].ToString(),
                            };

                            
                            string key = reader["ItemKey"].ToString();

                            if (!string.IsNullOrEmpty(key) && reader["SourceGUID"] != System.DBNull.Value)
                            {
                                c.SourceProduct = new AIn.Ventures.Models.SourceProduct()
                                {
                                    ItemKey= key,
                                    ItemDate = (DateTime)reader["ItemDate"]
                                };

                                c.SourceProduct.Source.GUID = (Guid)reader["SourceGUID"];
                                c.SourceProduct.Source.Name = reader["SourceName"].ToString();
                                c.SourceProduct.Source.Token = reader["Token"];

                            }
                            comps[n] = c;

                            /*
                            // If component is a child of the root, add it to the root.
                            if (GUID == c.ParentGUID)
                            {
                                root.Children.Add(c);
                            }

                            // Look for parent and add component to the parent.
                            for (int i = 0; i < n; i++)
                            {
                                if (comps[i].GUID == c.ParentGUID)
                                {
                                    comps[i].Children.Add(c);
                                    break;
                                }
                            }
                            */
                            n++;

                        }
                    }

                }
            }
            finally
            {
                ent.Database.Connection.Close();
            }
            
            return Ok(root);

        }

        [HttpDelete, Route("Component/{GUID}")]
        [Authorize]
        [ActionRightsFilter(SecuredEntity = Entity.Project, RequiredRight = Right.Delete)]
        public IHttpActionResult DeleteComponent(Guid GUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            ent.DeleteComponent_ByObjectGUID(GUID);
            ent.SaveChanges();

            return Ok();
        }

        [HttpDelete, Route("Module/{GUID}")]
        [Authorize]
        [ActionRightsFilter(SecuredEntity = Entity.Project, RequiredRight = Right.Delete)]
        public IHttpActionResult DeleteModule(Guid GUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            ent.DeleteComponent_ByObjectGUID(GUID);
            ent.SaveChanges();

            return Ok();
        }

        [Authorize]
        [ActionRightsFilter(SecuredEntity = Entity.Project, RequiredRight = Right.View)]
        [HttpPut, Route("Component/{GUID}")]
        public IHttpActionResult UpdateComponent(Guid GUID, AIn.Ventures.Models.Component Component)
        {
            AInVentureEntities ent = new AInVentureEntities();

            ent.UpdateComponent_ByObjectGUID(GUID, Component.Name, Component.Description, Component.ComponentType, Component.Price, Component.Manufacturer, Component.SKU, Component.Supplier, Component.ParentGUID);
            ent.SaveChanges();

            return Ok("OK");
        }


        [Authorize]
        [ActionRightsFilter(SecuredEntity = Entity.Project, RequiredRight = Right.View)]
        [HttpPut, Route("Module/{GUID}")]
        public IHttpActionResult UpdateModule(Guid GUID, AIn.Ventures.Models.Component Component)
        {
            AInVentureEntities ent = new AInVentureEntities();

            ent.UpdateComponent_ByObjectGUID(GUID, Component.Name, Component.Description, Component.ComponentType, Component.Price, Component.Manufacturer, Component.SKU, Component.Supplier, Component.ParentGUID);
            ent.SaveChanges();

            return Ok("OK");
        }




        private void addElements(AIn.Ventures.Models.Component cmp)
        {
            var childComponents = UnprocessedComponents.Where(cp => cp.ParentGUID == cmp.GUID).ToList<AIn.Ventures.Models.Component>();

            foreach (var child in childComponents)
            {
                addElements(child);
                cmp.Children.Add(child);
            }

        }

        [Authorize]
        [ActionRightsFilter(SecuredEntity = Entity.Project, RequiredRight = Right.View)]
        [HttpPut, Route("Module/addchild/{GUID}")]
        public IHttpActionResult AddChild(Guid GUID, AIn.Ventures.Models.Component Component)
        {
            var childComponents = UnprocessedComponents.Where(cp => cp.ParentGUID == Component.GUID).ToList<AIn.Ventures.Models.Component>();

            foreach (var child in childComponents)
            {
                addElements(child);
                Component.Children.Add(child);
            }

            return Ok();
        }

        [HttpPost, Route("Component/{GUID}/{ObjectGUID}/{SourceName}/{ItemKey}")]
        //[Authorize]
        public IHttpActionResult AssignToSource(Guid GUID, Guid ObjectGUID, string SourceName, string ItemKey)
        {
            //return Ok();

            //Guid guid = Guid.NewGuid();

            AIn.Ventures.Models.SourceProduct p = new AIn.Ventures.Models.SourceProduct();
            p.ItemKey = ItemKey;
            p.ItemDate = DateTime.Now;
            p.Source.Name = SourceName;
            p.ComponentGUID = ObjectGUID;
            p.ParentGUID = GUID;
            

            AInVentureEntities ent = new AInVentureEntities();

            var cmd = ent.Database.Connection.CreateCommand();

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "Component_AssignSource";

            cmd.Parameters.Add(new SqlParameter("@ParentGUID", GUID));
            cmd.Parameters.Add(new SqlParameter("@ObjectGUID", ObjectGUID));
            cmd.Parameters.Add(new SqlParameter("@SourceName", SourceName));
            cmd.Parameters.Add(new SqlParameter("@ItemKey", ItemKey));

            try
            {
                ent.Database.Connection.Open();
                //return Ok(componentFromQpoint);
                System.Data.Common.DbDataReader reader = cmd.ExecuteReader();

                if( reader.Read())
                {
                    p.Source.GUID = (Guid)reader["GUID"];
                    p.Source.Token = reader["CommissionToken"];

                    if (SourceName.ToLower() == "ebay")
                    {
                        p.Url = "http://rover.ebay.com/rover/1/711-53200-19255-0/1?icep_ff3=2&pub=5575211573&toolid=10001&campid=5338035878&customid=FFTest&icep_item=" + ItemKey + "&ipn=psmain&icep_vectorid=229466&kwid=902099&mtid=824&kw=lg";
                    }

                }

            }
            finally
            {
                ent.Database.Connection.Close();
            }

            // Guid u = new Guid(((System.Security.Claims.ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == "GUID").Value);



            return Ok(p);
        }

        [HttpGet, Route("Component/{GUID}/{ObjectGUID}/Source")]
        public IHttpActionResult GetAvailableProduct(Guid GUID, Guid ObjectGUID)
        {
            AIn.Ventures.Models.Component root = new AIn.Ventures.Models.Component();

            AIn.Ventures.BaseLibrary.AInVentureEntities ent = new AIn.Ventures.BaseLibrary.AInVentureEntities();

            var cmd = ent.Database.Connection.CreateCommand();

            cmd.Parameters.Add(new SqlParameter("@ParentGUID", GUID));
            cmd.Parameters.Add(new SqlParameter("@ObjectGUID", ObjectGUID));

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetSourceProduct_ByObjectGUID";

            AIn.Ventures.Models.SourceProduct sprod = new AIn.Ventures.Models.SourceProduct();
            
            try
            {
                ent.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    sprod.ItemKey = reader["ItemKey"].ToString();
                    sprod.ItemDate = (DateTime)reader["ItemDate"];
                    sprod.Source.Name = reader["SourceName"].ToString();
                    sprod.Source.Token = reader["CommissionToken"].ToString();
                }

                reader.Close();
            }
            finally
            {
                ent.Database.Connection.Close();

            }

            return Ok(sprod);
       }


    }
}

