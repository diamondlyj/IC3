using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace MIX2.Action.WindowsService
{
    public partial class ActionService : ServiceBase
    {
        List<Thread> threads;

        public ActionService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

            //PerformanceCounters.Create();

            string source = "MIX2.Action";
            string log = "MIX2";

            if (!System.Diagnostics.EventLog.SourceExists(source))
            {
                System.Diagnostics.EventLog.CreateEventSource(source, log);
                Thread.Sleep(1000);
            }
            
            this.threads = new List<Thread>();

            Mutex mutex = new Mutex();

            int maxThreads = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxThreads"]);

            //System.Diagnostics.EventLog.WriteEntry(source, System.Environment.CurrentDirectory );

            for (int i = 0; i < maxThreads; i++)
            {
                DirectoryInfo dinfo = new DirectoryInfo("Scripts");

                FileInfo[] files = dinfo.GetFiles();

                for (int j = 0; j < files.Length; i++)
                {
                    string name = files[j].Name;
                    int index = name.LastIndexOf('.');

                    string ext = string.Empty;

                    if (index > -1)
                    {
                        ext = name.Substring(index);
                    }

                    if (ext.ToLower() == "xcs")
                    {
                        Process p = new Process("Process " + i.ToString(), false);
                        p.LoopScript = name;

                        p.Mutex = mutex;

                        Thread t = new Thread(p.LoopExecute);

                        this.threads.Add(t);

                        t.Start();
                    }                    
                }

                /*
                Process p = new Process("Process " + i.ToString(), false);

                p.Mutex = mutex;

                Thread t = new Thread(p.Run);

                this.threads.Add(t);

                t.Start();

                Thread.Sleep(1000);
                 * */
            }
                 
        }

        protected override void OnStop()
        {
            if (this.threads != null)
            {
                foreach (Thread t in this.threads)
                {
                    t.Abort();                   
                }
            }
        }
    }
}
