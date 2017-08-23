using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.Net;

using MIX2.Data;
using MIX2.Data.Client;

using IntuitiveLabs.Processes;

namespace MIX2.Acquisition
{
    public delegate void OCPushStartedDelegat(string DataSourceName, DataSourceSchema.ObjectClass ObjectClass);
    public delegate void OCPushFinishedDelegat(string DataSourceName, DataSourceSchema.ObjectClass ObjectClass);
   
    public class PusherContext
    {
        private Updater updater;
        private DataSource source;

        public PusherContext( Updater updater, DataSource source)
        {
            this.updater = updater;
            this.source = source;
        }

        public Updater Updater
        {
            get { return this.updater; }
        }

        public DataSource Source
        {
            get { return this.source; }
        }
    }

    public class Pusher : IActor
    {
        private string sourceName;
        private string assemblyName;
        private ITokenProvider tokenProvider;
        private NetworkCredential networkCredential;
        private MIX2.Acquisition.Configuration.Connection connection;
        private string objectClassMapping;
        private Updater updater;

        public Pusher(Updater updater, string sourceName, NetworkCredential networkCredential, MIX2.Acquisition.Configuration.Connection connection, string objectClassMapping, string assemblyName)
        {
            this.updater = updater;
            this.sourceName = sourceName;
            this.networkCredential = networkCredential;
            this.connection = connection;
            this.objectClassMapping = objectClassMapping;
            this.assemblyName = assemblyName;
        }

        public Pusher(Updater updater, string sourceName, NetworkCredential networkCredential, MIX2.Acquisition.Configuration.Connection connection, string objectClassMapping, string assemblyName, ITokenProvider tokenProvider)
        {
            this.updater = updater;
            this.sourceName = sourceName;
            this.networkCredential = networkCredential;
            this.connection = connection;
            this.objectClassMapping = objectClassMapping;
            this.assemblyName = assemblyName;
            this.tokenProvider = tokenProvider;
        }

        public string ID
        {
            get { return ""; }
        }

        public void Perform()
        {
            throw new NotImplementedException();
        }

        public object ThreadContext
        {
            get
            {               
                if( this.tokenProvider == null )
                    return new PusherContext( this.updater, new DataSource(this.sourceName, this.networkCredential, this.connection, this.objectClassMapping, this.assemblyName) );
                else
                    return new PusherContext( this.updater, new DataSource(this.sourceName, this.networkCredential, this.connection, this.objectClassMapping, this.assemblyName, this.tokenProvider) );
            }
        }

        public void Perform(object threadContext)
        {
            PusherContext context = (PusherContext)threadContext;

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(context.Source.RawSourceMapping);

            System.Xml.XPath.XPathNavigator nav = doc.DocumentElement.CreateNavigator();

            DataSourceSchema.ObjectClass oc = new DataSourceSchema.ObjectClass(nav);
                        
            try
            {
                //if (this.PushStarted != null)
                //    this.PushStarted(m_DataSource.Name, m_ObjectClass);

                string mes = String.Empty;
                
                /*
                if (!context.Source.MIXFactory.IsAlive(out mes))
                {
                    Console.WriteLine("Source is not alive: " + mes);
                    return;
                }
                */
                
                context.Source.Open(oc);

                while (!context.Source.MIXFactory.EOD)
                {
                    LocalObject obj = context.Source.MIXFactory.GetNext();
                    obj.ObjectClass = oc.Name;

                    if (obj != null)
                    {
                        string Result = context.Updater.SendObject(obj);
                        Console.WriteLine(Result);
                    }
                }

                context.Source.Close();

                /*
                if (this.PushFinished != null)
                    this.PushFinished(m_DataSource.Name, m_ObjectClass);
                 * */
            }
            catch( Exception exc )
            {
                Trace.WriteLine( exc.Message + ":" + exc.StackTrace  );

                EventLog log = PerformanceReporter.GetEventLog();
                log.WriteEntry("A failure occured while pushing data from [" + context.Source.Name + "]: " + exc.Message, EventLogEntryType.Error);
            }
        }
    }
}
