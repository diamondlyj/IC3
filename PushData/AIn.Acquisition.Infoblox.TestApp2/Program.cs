
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MIX2.Acquisition;

namespace AIn.Acquisition.Infoblox.TestApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            string source = string.Empty;
            string username = string.Empty;
            string password = string.Empty;

            for (int i = 0; i < args.Length; i += 2)
            {
                switch (args[i])
                {
                    case "-source":
                        source = args[i + 1];
                        break;

                    default:
                        Console.WriteLine("Invalid argument: " + args[i]);
                        return;
                }
            }

            Console.WriteLine("Enter your credentials for Infoblox.");

            Console.Write("Username:");
            username = Console.ReadLine();

            Console.Write("Password:");

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            while (keyInfo.Key != ConsoleKey.Enter)
            {
                password += keyInfo.KeyChar;
                Console.Write("*");

                keyInfo = Console.ReadKey(true);
            }

            Console.WriteLine();

            string connString = "url=https://infoblox.tribunemedia.com/wapi/v1.7.1/;";
            MIX2.Acquisition.WebConnection conn = new WebConnection(new System.Net.NetworkCredential(username, password), connString);
            Console.Write("Bypassing SSL check...");

            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            /*
            System.Net.WebRequest req = System.Net.WebRequest.Create(source);
            req.Credentials = new System.Net.NetworkCredential(username, password);
            req.ContentType = "application/json";
            req.Method = "GET";
            */

            //  System.Net.WebResponse resp = req.GetResponse();
            //System.IO.Stream stream = resp.GetResponseStream();
            // System.IO.StreamReader reader = new System.IO.StreamReader(stream);

            //Console.WriteLine(reader.ReadToEnd());
            InfobloxFactory factory = new InfobloxFactory("Infoblox", conn);

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load("Infoblox.xml");

            System.Xml.XPath.XPathNavigator nav = doc.CreateNavigator();

            MIX2.Acquisition.DataSourceSchema schema = new MIX2.Acquisition.DataSourceSchema("BigFix", nav);



            string mes = string.Empty;

            bool isAlive = factory.IsAlive(out mes);

            Console.WriteLine(mes);


            if (isAlive)
            {
                //factory.getData();
                factory.Open(schema.ObjectClasses.First());
                int i = 0;
                do
                {
                    XmlFileClass xml = factory.xml;
                    Console.WriteLine("Page {0}", i);
                    for (int j = 0; j < xml.list.Count; j++)
                    {
                        valueObject value = xml.list[j];
                        Console.WriteLine("========= Data Object {0} Start =============", j);
                        Console.WriteLine("          comment: {0}", value.Comment);
                        Console.WriteLine("          network: {0}", value.Network);
                        Console.WriteLine("          network_view: {0}", value.Network_view);
                        Console.WriteLine("          _ref: {0}", value._Ref);
                    }
                    i++;
                    if (factory.EOD) break;

                    factory.GetNextData();
                    Console.ReadLine();
                }
                while (true);
                factory.Close();
/*
                for ( int i = 0; i < factory.list.Count; i ++)
                {
                    XmlFileClass xml = factory.list[i];
                    Console.WriteLine("Page {0}", i);
                    for ( int j = 0; j < xml.list.Count; j ++)
                    {
                        valueObject value = xml.list[j];
                        Console.WriteLine("========= Data Object {0} Start =============", j);
                        Console.WriteLine("          comment: {0}", value.Comment);
                        Console.WriteLine("          network: {0}", value.Network);
                        Console.WriteLine("          network_view: {0}", value.Network_view);
                        Console.WriteLine("          _ref: {0}", value._Ref);
                    }
                    Console.ReadLine();
                } 
                */
            }
        }
    }
}
