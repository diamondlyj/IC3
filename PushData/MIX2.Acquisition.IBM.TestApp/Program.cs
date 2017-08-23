using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIX2.Acquisition.IBM.TestApp
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
                    case "-url":
                        source = args[i + 1];
                        break;


                    case "-username":
                        username = args[i + 1];
                        break;


                    default:
                        Console.WriteLine("Invalid argument: " + args[i]);
                        return;
                }
            }

            Console.WriteLine("Enter your credentials for BigFix.");

            if (String.IsNullOrEmpty(username))
            {
                Console.Write("Username:");
                username = Console.ReadLine();
            }

            Console.Write("Password:");

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            while( keyInfo.Key != ConsoleKey.Enter )
            {
                password += keyInfo.KeyChar;
                Console.Write("*");

                keyInfo = Console.ReadKey(true);
            }

            Console.WriteLine();

            //string connString = "url=" + source + "/computers; timeout=900000; DefaultExpression=number of bes computers; Username=" + username + "; Password=" + password + "; service type=MIX2.Acquisition.IBM.Tivoli.EndpointService.RelevanceBinding;";
            string connString = "url=" + source + "; timeout=900000;";

            //System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFrom( "MIX2.Acquisition.IBM.dll");
            //MIX2.Acquisition.ServiceConnection conn = new ServiceConnection(new System.Net.NetworkCredential("x","x",null), connString, ass );

            System.Net.NetworkCredential creds = new System.Net.NetworkCredential(username, password);
            MIX2.Acquisition.WebConnection conn = new WebConnection(creds, connString);

            Console.Write("Bypassing SSL check...");

            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            //.RelevanceBinding b = new Tivoli.EndpointService.RelevanceBinding();

            //Tivoli.EndpointFactory factory = new Tivoli.EndpointFactory("Tivoli Endpoint Web Reports", conn);
            BigFix.Factory factory = new BigFix.Factory("BigFix", conn);

            string mes = string.Empty;

            bool isAlive = factory.IsAlive(out mes);

            Console.WriteLine(mes);

            if( isAlive )
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load("BigFix.xml");

                System.Xml.XPath.XPathNavigator nav = doc.CreateNavigator();

                MIX2.Acquisition.DataSourceSchema schema = new MIX2.Acquisition.DataSourceSchema("BigFix", nav);


                Console.WriteLine("Iterating");

                factory.Open(schema.ObjectClasses.First());
                
                
                while (!factory.EOD)
                {
                    Data.LocalObject obj = factory.GetNext();
                    //Console.WriteLine(obj.GetXml().ToString());
                }
                

                factory.Close();

                /*
                string relExp = "(id of it,(values of it) of ((property results of it) whose (((name of it) of property of it) = \"DNS Servers - Windows\"))) of bes computers";

                string[] data = (string[])conn.GetData("GetRelevanceResult", new object[] { relExp, "[$Username]", "[$Password]" });

                for (int i = 0; i < data.Length; i++)
                {
                    Console.WriteLine(data[i]);
                }
                */

                /*
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load("TivoliEndpoint.xml");

                System.Xml.XPath.XPathNavigator nav = doc.CreateNavigator();

                MIX2.Acquisition.DataSourceSchema schema = new MIX2.Acquisition.DataSourceSchema("TivoliEndpoint", nav);

                factory.Open(schema.ObjectClasses.First());

                
                while (!factory.EOD)
                {
                    Data.LocalObject obj = factory.GetNext();
                        Console.WriteLine(obj.GetXml().ToString());
                }
                

                factory.Close();
                */
            }
        }
    }
}
