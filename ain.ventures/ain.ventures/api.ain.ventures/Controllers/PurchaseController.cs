using AIn.Ventures.BaseLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AIn.Ventures.Models;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace api.ain.ventures.Controllers
{
    public class PurchaseController : ApiController
    {
        //[Authorize]
        //[ActionRightsFilter(SecuredEntity = Entity.Component, RequiredRight = Right.Transact)]
        [HttpPost, Route("Component/{GUID}/Source")]
        public IHttpActionResult OrderComponent(Guid GUID, [FromBody]OrderParameters OrderParameters)
        {
            //QPoint
            //Amount
            //Price range
            //Source
            //Sign-in

            AIn.Ventures.Models.QPoint qpoint = new AIn.Ventures.Models.QPoint();
            qpoint.Categories.Add(OrderParameters.Amount.ToString());

            // Get query for source and comission token.
                        
            AInVentureEntities ent = new AInVentureEntities();

            var cmd = ent.Database.Connection.CreateCommand();

            cmd.Parameters.Add(new SqlParameter("@ObjectGUID", GUID));
            cmd.Parameters.Add(new SqlParameter("@SourceName", OrderParameters.SourceName));

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[GetQPoint_BySourceName]";

            string query = string.Empty;
            string commissionToken = string.Empty;

            try
            {
                ent.Database.Connection.Open();

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    query = reader["Query"].ToString();
                    commissionToken = reader["CommissionToken"].ToString();
                }
            }
            finally
            {
                ent.Database.Connection.Close();
            }

            List <AvailableProduct> availableProducts = new List<AvailableProduct>();

            if (OrderParameters.SourceName.ToLower() == "ebay")
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-EBAY-SOA-SECURITY-APPNAME", "AINTHInc-AInVentu-PRD-02461718a-5741feaf");
                    client.DefaultRequestHeaders.Add("X-EBAY-SOA-OPERATION-NAME", "findItemsByKeywords");

                    // Call supplier and get products

                    String body = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><findItemsByKeywords xmlns=\"http://www.ebay.com/marketplace/search/v1/services\">" + query + commissionToken + "<itemFilter><name>topRatedSellerOnly</name><value>true</value></itemFilter><itemFilter><name>hideDuplicateItems</name><value>true</value></itemFilter><sortOrder>BestMatch</sortOrder><paginationInput><entriesPerPage>100</entriesPerPage><pageNumber>1</pageNumber></paginationInput></findItemsByKeywords>";

                    //return Ok(body);

                    var r = client.PostAsync("http://svcs.ebay.com/services/search/FindingService/v1", new StringContent(body)).Result;


                    //return Ok(r.Content.ReadAsStringAsync().Result);

                    var response = r.Content.ReadAsStreamAsync();

                    XDocument doc = XDocument.Load(response.Result);

                    XNamespace ns = doc.Root.GetDefaultNamespace();

                    //Console.WriteLine( doc.Root.Descendants("searchResult").First().ToString() );

                    XElement sel = doc.Root.Element(ns + "searchResult");

                    List<AIn.Ventures.Models.AvailableProduct> products = new List<AIn.Ventures.Models.AvailableProduct>();


                    foreach (XElement el in sel.Elements(ns + "item"))
                    {
                        XElement salesData = el.Element(ns + "sellingStatus");

                        AIn.Ventures.Models.AvailableProduct p = new AIn.Ventures.Models.AvailableProduct()
                        {
                            OrderID = el.Element(ns + "itemId").Value,
                            Name = el.Element(ns + "title").Value,
                            Url = el.Element(ns + "viewItemURL").Value,
                            Price = Convert.ToDouble(salesData.Element(ns + "currentPrice").Value)
                            
                        };
                        p.Source = new AIn.Ventures.Models.Source();
                        p.Source.Name = OrderParameters.SourceName;
                        if (p.Url.Contains("item=0") == false)
                        {
                            availableProducts.Add(p);
                        }
                    }
                }
            }

            /*
            AIn.Ventures.Models.Source s = new AIn.Ventures.Models.Source();
            
            //s.Query = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><findCompletedItemsRequest xmlns=\"http://www.ebay.com/marketplace/search/v1/services\">   <keywords>iPhone</keywords>   <categoryId>9355</categoryId>   <itemFilter>      <name>Condition</name>      <value>1000</value>   </itemFilter>   <itemFilter>      <name>FreeShippingOnly</name>      <value>true</value>   </itemFilter>   <itemFilter>      <name>SoldItemsOnly</name>      <value>true</value>   </itemFilter>   <sortOrder>PricePlusShippingLowest</sortOrder>   <paginationInput>      <entriesPerPage>2</entriesPerPage>      <pageNumber>1</pageNumber>   </paginationInput></findCompletedItemsRequest>";
            //qpoint.Sources
            //System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            //doc.LoadXml(s.Query);

            if (OrderParameters.SourceName.ToLower() == "ebay")
            {
                AvailableProduct ap = new AvailableProduct()
                {
                    Name = "ebay-majig",
                    SKU = "EB-JIG-001",
                    Manufacturer = "JigCorp",
                    OrderID = Guid.NewGuid().ToString(),
                    Price = 0.021
                };
                availableProducts.Add(ap);
            }
            else if (OrderParameters.SourceName.ToLower() == "amazon")
            {
                AvailableProduct ap = new AvailableProduct()
                {
                    Name = "Amaze-Zonerficator 2.1",
                    SKU = "AZ-ZON-102",
                    Manufacturer = "Zones",
                    OrderID = Guid.NewGuid().ToString(),
                    Price = 220.0000000001
                };
                availableProducts.Add(ap);
            }
            else if (OrderParameters.SourceName.ToLower() == "amazon")
            {
                AvailableProduct ap = new AvailableProduct()
                {
                    Name = "Amaze-Zonerficator 2.1",
                    SKU = "AZ-ZON-102",
                    Manufacturer = "Zones",
                    OrderID = Guid.NewGuid().ToString(),
                    Price = 220.0000000001
                };
                availableProducts.Add(ap);
            }
            else if (OrderParameters.SourceName.ToLower() == "home_depot")
            {
                AvailableProduct ap = new AvailableProduct()
                {
                    Name = "Invisible Oxygen Hydride Dye",
                    SKU = "HD-PAI-AKA",
                    Manufacturer = "EddyScope",
                    OrderID = Guid.NewGuid().ToString(),
                    Price = 0.25
                };
                availableProducts.Add(ap);
            }
            else if (OrderParameters.SourceName.ToLower() == "radioshack")
            {
                AvailableProduct ap = new AvailableProduct()
                {
                    Name = "nvm",
                    SKU = "wow sutpid",
                    Manufacturer = ":)",
                    OrderID = Guid.NewGuid().ToString(),
                    Price = 0.00
                };
                availableProducts.Add(ap);
            }
            */

            //List<AvailableProduct> availableProducts = qpoint.getData(qpoint.Query);
            return Ok(availableProducts);
            //return Ok(doc.OuterXml);
        }
    }
}
