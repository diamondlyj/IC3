using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIX2.Acquisition.LDAP.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Type t = typeof(Program);

            string helpMessage = "To test mapping, you must specify a server and LDAP path to use as base source and a map file:\n\n\t" + t.Assembly.GetName().Name + " -source \"LDAP:\\\\MyLDAPServerAddress\\OU=MyUnit,DC=MySubDomain,DC=MyDomain\" -map \"c:\\MyDir\\MyMap.xml\"\n";

            if (args.Length < 4)
            {
                Console.WriteLine(helpMessage);
                return;
            }

            string map = string.Empty;
            string source = string.Empty;
            string username = string.Empty;
            string password = string.Empty;

            //Console.Write("Map:");
            //map = Console.ReadLine();

            //Console.Write("Source:");
            //source = Console.ReadLine();

            Console.Write("Username:");
            username = Console.ReadLine();

            Console.Write("Password:");
            password = Console.ReadLine();

            
            for (int i = 0; i < args.Length; i += 2)
            {
                switch (args[i])
                {
                    case "-source":
                        source = args[i + 1];
                        break;

                    case "-map":
                        map = args[i + 1];
                        break;

                    default:
                        Console.WriteLine("Invalid argument: " + args[i]);
                        return;
                }
            }
            

            if (source == string.Empty || map == string.Empty)
            {
                Console.WriteLine(helpMessage);
                return;
            }


            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            try
            {
                doc.Load(map);
            }
            catch
            {
                Console.WriteLine("The mapping file " + map + " could not be loaded. Make sure the file contains valid xml.");
            }

            System.Net.NetworkCredential cred = new System.Net.NetworkCredential(username, password);

            MIX2.Acquisition.LDAP.ObjectFactory factory = new ObjectFactory(source, new LDAP.Connection(cred, "path=" + source));

            //Load an ObjectClass to test with
            System.Xml.XmlNodeList classNodes = doc.DocumentElement.SelectNodes("./ObjectClass");

            foreach (System.Xml.XmlNode classNode in classNodes)
            {
                int n = 0;

                DataSourceSchema.ObjectClass objClass = new DataSourceSchema.ObjectClass(classNode.CreateNavigator());

                factory.Open(objClass);
                
                while (!factory.EOD)
                {
                    Console.WriteLine("Object " + n.ToString());
                    Console.WriteLine("-------------------------------------");
                    MIX2.Data.LocalObject obj = factory.GetNext();
                    Console.WriteLine(obj.GetXml());
                    Console.WriteLine("");

                    n++;
                }
            }
        }
    }
}
