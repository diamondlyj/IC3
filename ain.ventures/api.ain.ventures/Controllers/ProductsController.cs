using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AIn.Ventures.BaseLibrary;
using Newtonsoft.Json;



namespace api.ain.ventures.Controllers
{
    public class ProductsController : ApiController
    {

        AIn.Ventures.Models.Component RootComponent;
        List<AIn.Ventures.Models.Component> UnprocessedComponents;


        // GET: Products

        [HttpPost, Route("Product")]
        [Authorize]
        public IHttpActionResult CreateProduct([FromBody]AIn.Ventures.Models.Product product)
        {
            Guid guid = Guid.NewGuid();

            AInVentureEntities ent = new AInVentureEntities();

            Guid u = new Guid(((System.Security.Claims.ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == "GUID").Value);

            //ent.CreateComponent(guid, "Produt", product.Name, product.Description, 0, "self", "", "", product.ProjectGUID);//(guid, project.Name, project.Description, 100, u);

            return Ok(guid);
        }
        [HttpDelete, Route("Product/{GUID}")]
        [Authorize]
        [ActionRightsFilter(SecuredEntity = Entity.Project, RequiredRight = Right.Delete)]
        public IHttpActionResult DeleteProduct(Guid GUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            ent.DeleteComponent_ByObjectGUID(GUID);
            ent.SaveChanges();

            return Ok();
        }
        [HttpGet, Route("Product/{RootGuid}")]
        [AllowAnonymous]
        public IHttpActionResult GetProducts(Guid? RootGuid)
        {

            // Guid RootGuid = Guid.Parse("820BF26F-4D0B-4630-9CC0-16205ED93759");
            UnprocessedComponents = new List<AIn.Ventures.Models.Component>();

            AIn.Ventures.Models.Project project = new AIn.Ventures.Models.Project();
            // List<AIn.Ventures.Models.Component> list1 = new List<AIn.Ventures.Models.Component>;
            AIn.Ventures.BaseLibrary.AInVentureEntities entities = new AInVentureEntities();
            IEnumerator<GetProducts_ByProjectGUID1_Result> ienumerator = entities.GetProducts_ByProjectGUID1(RootGuid).GetEnumerator();
            IEnumerator<GetProjects_ByProjectGUID_Result> result = entities.GetProjects_ByProjectGUID(RootGuid).GetEnumerator();
            var ProjectList = new List<GetProjects_ByProjectGUID_Result>();
            while (result.MoveNext())
            {
                ProjectList.Add(result.Current);
            }
            foreach (var thing in ProjectList)
            {

                project.GUID = thing.ProjectGUID;
                project.Name = thing.ProjectName;
                project.Description = thing.Description;
            }


            var list = new List<GetProducts_ByProjectGUID1_Result>();

            if (ienumerator.MoveNext())
            {
                list.Add(ienumerator.Current);
                RootComponent = new AIn.Ventures.Models.Component { ParentGUID = ienumerator.Current.ParentGUID, GUID = ienumerator.Current.ObjectGUID, ComponentType = ienumerator.Current.ComponentType, AmountInParent = 1, Description = ienumerator.Current.Description, Name = ienumerator.Current.Name, Supplier = ienumerator.Current.Supplier, SKU = ienumerator.Current.SKU, Manufacturer = ienumerator.Current.Manufacturer };
            }

            //    return Ok(list);




            foreach (var item in list)
            {
                if (item.ObjectGUID == RootGuid)
                {
                    RootComponent = new AIn.Ventures.Models.Component { ParentGUID = item.ParentGUID, GUID = item.ObjectGUID, ComponentType = item.ComponentType, Description = item.Description, Name = item.Name, AmountInParent = 1, Manufacturer = item.Manufacturer, SKU = item.SKU, Supplier = item.Supplier, Price = item.Price };
                    continue;
                }
                var newComp = new AIn.Ventures.Models.Component { ParentGUID = item.ParentGUID, GUID = item.ObjectGUID, ComponentType = item.ComponentType, Description = item.Description, Name = item.Name, AmountInParent = 1, Manufacturer = item.Manufacturer, SKU = item.SKU, Supplier = item.Supplier, Price = item.Price };
                if (newComp.ParentGUID == RootGuid)
                    RootComponent.Children.Add(newComp);
                UnprocessedComponents.Add(newComp);
            }
            foreach (var el in RootComponent.Children)
            {
                addElements(el);
            }

            addElements(RootComponent);
            project.Components.Add(RootComponent);
            return Ok(project);
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

        public IHttpActionResult UpdateProduct(Guid GUID, AIn.Ventures.Models.Product product)
        {
            AInVentureEntities ent = new AInVentureEntities();
            ent.UpdateComponent_ByObjectGUID(GUID, product.ComponentType, product.Name, product.Description, product.Price, product.Manufacturer, product.SKU, product.Supplier, product.ProjectGUID);
            ent.SaveChanges();
            return Ok(GUID + " was updated succesfully.");
        }



    }
    //[HttpGet, Route("Products/dummy")]
    //    [AllowAnonymous]
    //    public IHttpActionResult GetProducts(Guid GUID)
    //    {
    //        AIn.Ventures.Models.Product pro = new AIn.Ventures.Models.Product();
    //        pro.GUID = GUID;
    //        pro.Name = "First";
    //        pro.Description = "Our first dummy product";
    //        pro.Price = 100;
    //        pro.ParentGUID = Guid.NewGuid();
    //        pro.ObjectGUID = Guid.NewGuid();
    //        pro.Manufacturer = "Brooklyn Grid";
    //        pro.SKU = "LOL";
    //        pro.Supplier = "China";
            
    //        return Ok(pro);
    //    }
    }
