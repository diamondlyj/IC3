using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.IO;
using System.Linq;
using System.Text;

using Xcellence;

namespace MIX2.Action
{
    class Program
    {
        static void Main(string[] args)
        {
            //PerformanceCounters.Create();
            Console.WriteLine("Launching scripts.");

            bool isTest = false;
            string scriptPath = string.Empty;

            int m = 0;

            if (args.Length > 0)
            {
                scriptPath = args[0];

                if (scriptPath.IndexOf('-') != 0)
                {
                    //Console.WriteLine("!!!!!!!!!!!!!!!!");
                    m++;
                }
                else
                    scriptPath = string.Empty;
            }

            for (int i = m; i < args.Length; i++)
            {
                string arg = args[i];

                switch (arg)
                {
                    case "-test":
                    case "-t":
                        isTest = true;

                        /*
                        if ((i + 1) >= args.Length)
                        {
                            Console.WriteLine("-t (or -t) must be followed by a file path");
                            return;
                        }
                        */

                        //i++;
                        break;

                    default:
                        Console.WriteLine("The argument " + args[i] + " is not valid.");
                        return;
                }
            }



            if (scriptPath != string.Empty)
            {
                Process p = new Process("Process 1", isTest);
                p.LoopScript = scriptPath;
                p.LoopExecute();
                //p.Execute(scriptPath);
            }
            else
            {
                Mutex mutex = new Mutex();

                int maxThreads = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxThreads"]);

                for (int i = 0; i < maxThreads; i++)
                {
                    Process p = new Process("Process " + i.ToString(), isTest);

                    p.Mutex = mutex;

                    Thread t = new Thread(p.Run);

                    t.Start();

                    Thread.Sleep(1000);

                    //Thread.CurrentThread.Join(1000);
                }
            }
        }


        public static void StartScript(string test)
        {
            Console.WriteLine(test);
        }
    }
}
