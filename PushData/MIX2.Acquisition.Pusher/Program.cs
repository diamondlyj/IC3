//#define TRACE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Xml;
using System.Xml.XPath;
using System.Configuration;
using System.Threading;
using System.Net;
using System.Diagnostics;

using MIX2.Data;
using MIX2.Data.Client;

//using IntuitiveLabs.Data;
using IntuitiveLabs.Processes;
using IntuitiveLabs.Security;

namespace MIX2.Acquisition
{

    class Program
    {

        static private MIX2.Acquisition.Configuration.AcquisitionSection configAcquisition;
        static private IntuitiveLabs.Configuration.SecuritySection configSecurity;

        static private string s_DataSourceName;

        //static void AddTraceListeners()
        //{
        //    System.Diagnostics.Trace.Listeners.Clear();
        //    System.Diagnostics.Trace.Listeners.Clear();
        //    System.Diagnostics.Trace.Listeners.Add(
        //        new System.Diagnostics.TextWriterTraceListener(" "));

        //}


        static void LoadConfiguration()
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //string Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //Path += "\\OPX\\Pusher\\App.Config";
            //System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(Path);

            configAcquisition = (MIX2.Acquisition.Configuration.AcquisitionSection)config.GetSection("Acquisition");
            configSecurity = (IntuitiveLabs.Configuration.SecuritySection)config.GetSection("Security");
            if (configSecurity != null &&
             !configSecurity.SectionInformation.IsProtected)
            {
                configSecurity.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                configSecurity.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Minimal, true);
            }
        }

        static DataSource BuildDataSource(string DataSourceName)
        {
//            string DataSourceName = configAcquisition.DataSource.Name;

            if( !configAcquisition.DataSources.Exists(DataSourceName) )
            {
                Trace.WriteLine("DataSource:" + DataSourceName + " not found in the config file; Please check the case or spelling and restart the application. The closest matches are:");
                foreach(MIX2.Acquisition.Configuration.DataSource ds in configAcquisition.DataSources)
                {
                    if ( String.Compare(ds.Name, DataSourceName,StringComparison.InvariantCultureIgnoreCase) == 0 )
                        Trace.WriteLine(ds.Name);
                }
                throw new Exception("Data Source Name not found");
            }

            s_DataSourceName = DataSourceName;
            //string DbProvider = configAcquisition.DataSources[DataSourceName].Connection.Invariant;
            //string ConnectionString = configAcquisition.DataSources[DataSourceName].Connection.ConnectionString;
            string libName = configAcquisition.DataSources[DataSourceName].MIXLibrary.File;
            int CommandTimeout = configAcquisition.DataSources[DataSourceName].Connection.Timeout;

            Uri uriMap = configAcquisition.DataSources[DataSourceName].Map.Uri;

            string ncID = configAcquisition.DataSources[DataSourceName].Connection.NetworkCredentialID;

            NetworkCredential nc = new NetworkCredential();
            nc.Domain = configSecurity.NetworkCredentials[ncID].Domain;
            nc.UserName = configSecurity.NetworkCredentials[ncID].UserName;
            nc.Password = configSecurity.NetworkCredentials[ncID].Password;

            //ISimpleDatabaseAdapter DbAdapter = new SimpleDatabaseAdapter(DbProvider, ConnectionString, nc);

            XPathDocument xmlDataSourceSchema = new XPathDocument(uriMap.OriginalString);
            XPathNavigator navigator = xmlDataSourceSchema.CreateNavigator();

            DataSourceSchema dsSchema = new DataSourceSchema(DataSourceName, navigator);

            DataSource datasource;
            //Does it use a TokenProvider?

            if (configAcquisition.DataSources[DataSourceName].Connection.TokenProvider != null)
            {
                string tsName = configAcquisition.DataSources[DataSourceName].Connection.TokenProvider;

                TokenProvider tprov = new TokenProvider(configSecurity.TokenProviders[tsName].Name, nc, configSecurity.TokenProviders[tsName].Connection, configSecurity.TokenProviders[tsName].MIXLibrary.File);

                datasource = new DataSource(DataSourceName, nc, configAcquisition.DataSources[DataSourceName].Connection, dsSchema, libName, tprov );
            }
            else
                datasource = new DataSource(DataSourceName,nc,configAcquisition.DataSources[DataSourceName].Connection, dsSchema, libName );

            //datasource.ProviderName = DbProvider;
            //datasource.IdentifiersAcquired  += new IdentifiersAcquiredDelegat(datasource_IdentifiersAcquired);
            //datasource.DataChunkAcquired += new DataChunkAcquiredDelegat(datasource_DataChunkAcquired);
            //datasource.DataPointDropped += new DataPointDroppedDelegat(datasource_DataPointDropped);
            
            return datasource;
        }

        //static void datasource_DataPointDropped(string DataSourceName, DataPointIndex Index)
        //{
            //PerformanceCounter pc = PerformanceReporter.GetCounter("DroppedValues", DataSourceName, Index);
            //pc.RawValue += 1;
        //}

        static void datasource_IdentifiersAcquired(string DataSourceName, DataSourceSchema.ObjectClass ObjectClass, int Count)
        {
        }
 
        /*         
        static void datasource_DataChunkAcquired(string DataSourceName, DataPointIndex Index, int Count)
        {
            //PerformanceCounter pc = PerformanceReporter.GetCounter("AcquiredValues", DataSourceName, Index);
            //pc.RawValue = Count;
        }
         **/

        static Updater BuildUpdater()
        {
            Uri uriUpdater = configAcquisition.Updater.Url;
            int nMinTimeout = configAcquisition.Updater.MinTimeout;
            int nMaxTimeout = configAcquisition.Updater.MaxTimeout;
            int nAttempts = configAcquisition.Updater.Attempts;
            Updater upd = new Updater(uriUpdater, nMinTimeout, nMaxTimeout, nAttempts);
            upd.ObjectUpdateFailed += new ObjectUpdateFailedDelegat(upd_ObjectUpdateFailed);
            upd.ObjectUpdateSucceeded += new ObjectUpdateSucceededDelegat(upd_ObjectUpdateSucceeded);
            upd.NonTimeoutException += new NonTimeoutExceptionDelegate(upd_NonTimeoutException);
            upd.TimeoutUpperLimit += new TimeoutUpperLimitDelegate(upd_TimeoutUpperLimit);
            upd.TimeoutBackToNorm += new TimeoutBackToNormDelegate(upd_TimeoutBackToNorm);

            if (configAcquisition.Updater.NetworkCredentialID != null)
            {
                NetworkCredential nc = new NetworkCredential();
                nc.Domain = configSecurity.NetworkCredentials[configAcquisition.Updater.NetworkCredentialID].Domain;
                nc.UserName = configSecurity.NetworkCredentials[configAcquisition.Updater.NetworkCredentialID].UserName;
                nc.Password = configSecurity.NetworkCredentials[configAcquisition.Updater.NetworkCredentialID].Password;
                upd.Credential = nc;
            }

            return upd;
        }

        static void upd_NonTimeoutException(LocalObject obj, Exception ex)
        {
            PerformanceReporter.GetEventLog().WriteEntry("Non timeout exception: " + ex.Message + " for object: " + obj.GetXml(), EventLogEntryType.Error);
            Console.WriteLine("Non timeout exception: " + ex.Message + " for object: " + obj.GetXml());
        }

        static void upd_TimeoutBackToNorm(int Timeout)
        {
            PerformanceReporter.GetEventLog().WriteEntry("Timeout back to norm:" + Timeout.ToString());
            Console.WriteLine("Timeout back to norm:" + Timeout.ToString());
        }

        static void upd_TimeoutUpperLimit(int Timeout)
        {
            PerformanceReporter.GetEventLog().WriteEntry("Timeout reached upper limit:" + Timeout.ToString(), EventLogEntryType.Warning);
            Console.WriteLine("Timeout reached upper limit:" + Timeout.ToString());
        }


        static void upd_ObjectUpdateSucceeded(LocalObject obj)
        {
            Console.WriteLine("UpdateSucceeded");
            PerformanceCounter pc = PerformanceReporter.GetCounter("SucceededUpdates", obj.Source );
            pc.RawValue += 1;
        }

        static void upd_ObjectUpdateFailed(LocalObject obj)
        {
            PerformanceReporter.GetEventLog().WriteEntry("UpdateFailed:" + obj.GetXml());
            Console.WriteLine("UpdateFailed:" + obj.GetXml());
            //PerformanceCounter pc = PerformanceReporter.GetCounter("FailedUpdates", dp.DataSource, dp.Index);
            //pc.RawValue += 1;
            
        }


        static void Main(string[] args)
        {
            string DataSourceName = args[0];           

            if (DataSourceName == "")
            {
                Trace.WriteLine("DataSource Name is not defined.");
                return;
            }

            PerformanceReporter.GetEventLog().WriteEntry(DataSourceName + " is started");
           
            try
            {
                LoadConfiguration();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception when loading application configuration. Exception message:" + ex.Message);
                return;
            }

            Trace.WriteLine("DataSource: " + DataSourceName);

            DataSource datasource;
            try
            {
                datasource = BuildDataSource(DataSourceName);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception when loading data source data: connection, credentials, or schema(map). Original exception:" + ex.Message);
                return;
            }

            Updater updater;
            try
            {
                updater = BuildUpdater();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception when loading udater location. Original exception:" + ex.Message);
                return;
            }

            //Initialize performance counter
            PerformanceReporter.CreatePerformanceCounterCategory();

            //Create dirctory for app if there is none
            string dataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\MIX2";
            
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(dataDir);

            if (!dirInfo.Exists)
                dirInfo.Create();

            RSAProvider rsaProvider = RSAProvider.CreateFromName(configSecurity.Salt.Value + "MIX2");
            
            //Base and Extension, writing and reading to file is very similar cfor each can be place in method
            System.IO.FileInfo baseInfo = new System.IO.FileInfo(dataDir + "\\Base.mix");
            System.IO.FileInfo baseExtInfo = new System.IO.FileInfo(dataDir + "\\BaseExtension.mix");

            RijndaelProvider rijnProvider;

            if (!baseInfo.Exists)
            {
                rijnProvider = new RijndaelProvider();

                System.IO.FileStream stream = baseInfo.Create();

                System.IO.StreamWriter writer = new System.IO.StreamWriter(stream); // new System.IO.StreamWriter(fileName);
                writer.Write(Convert.ToBase64String(rsaProvider.Encrypt(rijnProvider.IV)));
                writer.Flush();
                writer.Close();

                stream.Close();

                System.IO.FileStream extstream;

                if (!baseExtInfo.Exists)
                    extstream = baseExtInfo.Create();
                else
                    extstream = baseExtInfo.OpenWrite();

                System.IO.StreamWriter extwriter = new System.IO.StreamWriter(extstream); // new System.IO.StreamWriter(fileName);
                extwriter.Write(Convert.ToBase64String(rsaProvider.Encrypt(rijnProvider.Key)));
                extwriter.Flush();
                extwriter.Close();

                extstream.Close();
            }
            else
            {
                System.IO.FileStream stream = baseInfo.OpenRead();
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                string iv = reader.ReadToEnd();
                reader.Close();
                stream.Close();

                System.IO.FileStream stream2 = baseExtInfo.OpenRead();
                System.IO.StreamReader reader2 = new System.IO.StreamReader(stream2);
                string key = reader2.ReadToEnd();
                reader2.Close();
                stream2.Close();

                rijnProvider = new RijndaelProvider( rsaProvider.Decrypt(Convert.FromBase64String(key)), rsaProvider.Decrypt(Convert.FromBase64String(iv)) );
            }

            System.IO.FileInfo sourceInfo = new System.IO.FileInfo(dataDir + "\\Source." + DataSourceName +  ".mix");

            if (!sourceInfo.Exists)
            {
                XmlDocument resultDoc = updater.RegisterSource(datasource.Name, configSecurity.Salt.Value);

                System.IO.FileStream stream = sourceInfo.Create();    

                System.IO.StreamWriter writer = new System.IO.StreamWriter(stream); // new System.IO.StreamWriter(fileName);
                writer.Write(Convert.ToBase64String(rijnProvider.Encrypt(resultDoc.OuterXml)));
                writer.Flush();
                writer.Close();

                stream.Close();
            }
            else
            {
                System.IO.FileStream stream = sourceInfo.OpenRead();
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                string sourceXml = rijnProvider.Decrypt(Convert.FromBase64String(reader.ReadToEnd()));
                reader.Close();
                stream.Close();

                XmlDocument sourceDoc = new XmlDocument();
                sourceDoc.LoadXml(sourceXml);

                RSAProvider signingProvider = RSAProvider.CreateFromKey(sourceDoc.DocumentElement.SelectSingleNode("Key").InnerXml );
                updater.SigningProvider = signingProvider;
                updater.SourceGUID = sourceDoc.DocumentElement.SelectSingleNode("SourceGUID").InnerXml;
                updater.Token = signingProvider.EncryptToString(sourceDoc.DocumentElement.SelectSingleNode("Token").InnerXml);
            }

            //Pusher pusher;

            ProcessScheduler scheduler = new ProcessScheduler();

            try
            {
                string ncID = configAcquisition.DataSources[DataSourceName].Connection.NetworkCredentialID;

                NetworkCredential nc = new NetworkCredential();
                nc.Domain = configSecurity.NetworkCredentials[ncID].Domain;
                nc.UserName = configSecurity.NetworkCredentials[ncID].UserName;
                nc.Password = configSecurity.NetworkCredentials[ncID].Password;

                TokenProvider tprov = null;

                if (configAcquisition.DataSources[DataSourceName].Connection.TokenProvider != null)
                {
                    string tsName = configAcquisition.DataSources[DataSourceName].Connection.TokenProvider;
                    tprov = new TokenProvider(configSecurity.TokenProviders[tsName].Name, nc, configSecurity.TokenProviders[tsName].Connection, configSecurity.TokenProviders[tsName].MIXLibrary.File);
                }

                string libName = configAcquisition.DataSources[DataSourceName].MIXLibrary.File;

                foreach (DataSourceSchema.ObjectClass oc in datasource.Schema.ObjectClasses)
                {
                    //Console.WriteLine(oc.Xml);

                    Pusher pusher;

                    if( tprov == null )
                        pusher = new Pusher(updater, datasource.Name, nc, configAcquisition.DataSources[DataSourceName].Connection, oc.Xml, libName);                   
                    else
                        pusher = new Pusher(updater, datasource.Name, nc, configAcquisition.DataSources[DataSourceName].Connection, oc.Xml, libName, tprov);

                    scheduler.AddProcess(pusher, oc.Name, oc.Period);
                }

                scheduler.Start();

            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception when transfering data from data source to OPX. Original exception:" + ex.Message);
                PerformanceReporter.GetEventLog().WriteEntry("Exception when transfering data from data source to OPX. Original exception:" + ex.Message, EventLogEntryType.Error);
                return;
            }
        }

        /*
        static void pusher_LongDuration(object sender, IActor actor, TimeSpan duration)
        {
            Pusher pusher = (Pusher)sender;

            PerformanceReporter.GetEventLog().WriteEntry("DataSource: " + pusher.Source.Name + " took too long to process " + actor.ID.ToString()+  " (" + duration.ToString() + ")", EventLogEntryType.Error);
        }

        static void pusher_OCPushFinished(string DataSourceName, DataSourceSchema.ObjectClass ObjectClass)
        {
            PerformanceReporter.GetEventLog().WriteEntry("DataSource: " + DataSourceName + " - Finished ObjectClass: " + ObjectClass.Name, EventLogEntryType.Information);
        }

        static void pusher_OCPushStarted(string DataSourceName, DataSourceSchema.ObjectClass ObjectClass)
        {
            PerformanceReporter.GetEventLog().WriteEntry("DataSource: " + DataSourceName + " - Started ObjectClass: " + ObjectClass.Name, EventLogEntryType.Information);
        }

        static void pusher_OCPushLongDuration(string DataSourceName, DataSourceSchema.ObjectClass ObjectClass, TimeSpan Duration)
        {
            PerformanceReporter.GetEventLog().WriteEntry("DataSource: " + DataSourceName + " took too long to process (" + Duration.ToString() + ")", EventLogEntryType.Error);
        }
        */

        /*
        static void pusher_IndexPushFinished(string DataSourceName, DataPointIndex Index)
        {
            PerformanceReporter.GetEventLog().WriteEntry("DataSource: " + DataSourceName + " - Finished Index: " + Index.ToString(), EventLogEntryType.Information);
        }

        static void pusher_IndexPushStarted(string DataSourceName, DataPointIndex Index)
        {
            PerformanceReporter.GetEventLog().WriteEntry("DataSource: " + DataSourceName + " - Started Index: " + Index.ToString(), EventLogEntryType.Information);
        }

        static void pusher_IndexPushLongDuration(string DataSourceName, DataPointIndex Index, TimeSpan Duration)
        {
            PerformanceReporter.GetEventLog().WriteEntry("DataSource: " + DataSourceName + " took too long to process (" + Duration.ToString() + ")", EventLogEntryType.Error);
        }
         * */
    }

}
