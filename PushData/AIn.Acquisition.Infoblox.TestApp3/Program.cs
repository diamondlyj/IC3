using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIX2.Acquisition;
using System.Net;
using System.IO;

namespace AIn.Acquisition.Infoblox.TestApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            string username = string.Empty;
            string password = string.Empty;
            string message = string.Empty;

            string url = "https://infoblox.tribunemedia.com/wapi/v1.6/grid";
           
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
            Console.Write("Bypassing SSL check...");

            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 400000;
            request.Credentials = new System.Net.NetworkCredential(username, password);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();                  Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    Console.WriteLine(responseFromServer);
                    reader.Close();
                    response.Close();
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }


    }
}
    

