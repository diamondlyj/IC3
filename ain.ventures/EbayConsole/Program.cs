using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace EbayConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new HttpClient())
            {
                var form = new Dictionary<string, string>
               {
                   {"grant_type", "password"},
                   {"username", "john5.doe@ain.ventures"},
                   {"password", "somePwd3456!"},
               };

                //client.DefaultRequestHeaders.Add.
                //var r = client.GetAsync("http://svcs.ebay.com/services/search/FindingService/v1?OPERATION-NAME=findItemsByProduct&SECURITYAPPNAME=JulienOl-testapp-SBX-bcd409a3e-ef2ce0f8D&RESPONSE-DATA-FORMAT=XML&REST-PAYLOAD&productIdd.@type=UPC&productId=024543611363&itemFilter%280%29.name=Condition&itemFilter%280%29.value=New&itemFilter%281%29.name=ListingType&itemFilter%281%29.value=FixedPrice&sortOrder=PricePlusShippingLowest").Result;
                client.DefaultRequestHeaders.Add("X-EBAY-SOA-SECURITY-APPNAME", "JulienOl-testapp-SBX-bcd409a3e-ef2ce0f8");
                client.DefaultRequestHeaders.Add("X-EBAY-SOA-OPERATION-NAME", "findCompletedItems");

                Console.WriteLine(client.DefaultRequestHeaders);

                //var p = client.PostAsync
                //String body = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><findCompletedItemsRequest xmlns=\"http://www.ebay.com/marketplace/search/v1/services\">   <keywords>iPhone</keywords>   <categoryId>9355</categoryId>   <itemFilter>      <name>Condition</name>      <value>1000</value>   </itemFilter>   <itemFilter>      <name>FreeShippingOnly</name>      <value>true</value>   </itemFilter>   <itemFilter>      <name>SoldItemsOnly</name>      <value>true</value>   </itemFilter>   <sortOrder>PricePlusShippingLowest</sortOrder>   <paginationInput>      <entriesPerPage>2</entriesPerPage>      <pageNumber>1</pageNumber>   </paginationInput></findCompletedItemsRequest>";

                String body = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><findCompletedItemsRequest xmlns=\"http://www.ebay.com/marketplace/search/v1/services\">   <keywords>resistor 100 ohms</keywords>         <sortOrder>PricePlusShippingLowest</sortOrder>   <paginationInput>      <entriesPerPage>2</entriesPerPage>      <pageNumber>1</pageNumber>   </paginationInput></findCompletedItemsRequest>";
                var r = client.PostAsync("http://svcs.sandbox.ebay.com/services/search/FindingService/v1", new StringContent(body)).Result;

                var response = r.Content.ReadAsStreamAsync();

                //System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                //doc.LoadXml(r.Content.ReadAsStringAsync().Result);
                
                XDocument doc = XDocument.Load(response.Result);

                XNamespace ns = doc.Root.GetDefaultNamespace();

                //Console.WriteLine( doc.Root.Descendants("searchResult").First().ToString() );
                
                XElement sel = doc.Root.Element(ns + "searchResult");

                List<AIn.Ventures.Models.AvailableProduct> products = new List<AIn.Ventures.Models.AvailableProduct>();

                foreach(XElement el in sel.Elements(ns+ "item"))
                {
                    XElement salesData = el.Element(ns + "sellingStatus");

                    AIn.Ventures.Models.AvailableProduct p = new AIn.Ventures.Models.AvailableProduct()
                    {
                        OrderID = el.Element(ns + "itemId").Value,
                        Name = el.Element(ns+"title").Value,
                        Price = Convert.ToDouble(salesData.Element(ns + "currentPrice").Value)
                    };

                    Console.WriteLine(p.Price);
                    Console.WriteLine("\n\n");
                }
                
                
                Console.WriteLine(r.Content.ReadAsStringAsync().Result);
            }
            //Console.WriteLine("smesty");
        }
    }
}
