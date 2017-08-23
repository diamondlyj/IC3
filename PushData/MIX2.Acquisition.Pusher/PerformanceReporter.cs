using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;

//using MIX.Data;

namespace MIX.Acquisition
{

    public class PerformanceReporter
    {

        static string LogName = "OPX";
        static string CategoryName = "OPX.Pusher";
        static string CategoryHelp = "Monitors performanc for pushing data from DataSoutce to Updates";

        
        static CounterCreationDataCollection ccdc;       


        public static void CreatePerformanceCounterCategory()
        {
            DeletePerformanceCounterCategory();

            ccdc = new CounterCreationDataCollection();
            ccdc.Add(new CounterCreationData("AcquiredValues", "DataPointValues dropped during the clean up", PerformanceCounterType.NumberOfItems64));
            ccdc.Add(new CounterCreationData("DroppedValues", "DataPointValues dropped during the clean up", PerformanceCounterType.NumberOfItems64));
            ccdc.Add(new CounterCreationData("SucceededUpdates", "DataPointValues successfully sent to Updater", PerformanceCounterType.RateOfCountsPerSecond32));
            ccdc.Add(new CounterCreationData("FailedUpdates", "DataPointValues failed to send to Updater", PerformanceCounterType.RateOfCountsPerSecond32));

            PerformanceCounterCategory.Create(CategoryName, CategoryHelp, PerformanceCounterCategoryType.MultiInstance, ccdc);
        }

        public static void DeletePerformanceCounterCategory()
        {
            if (PerformanceCounterCategory.Exists(CategoryName))
            {
                PerformanceCounterCategory.Delete(CategoryName);
            }

        }

        protected static string GetInstanceName(string DataSourceName)
        {
            return DataSourceName;
        }

        /*
        protected static string GetInstanceName(string DataSourceName, DataPointIndex Index)
        {
            return DataSourceName; // +"-" + Index.ToString();
        }
        */

        public static PerformanceCounter GetCounter(string CounterName, string DataSourceName)
        {
            return new PerformanceCounter(CategoryName, CounterName, GetInstanceName(DataSourceName), false);
        }

        /*
        public static PerformanceCounter GetCounter(string CounterName, string DataSourceName, DataPointIndex Index)
        {
                return new PerformanceCounter(CategoryName, CounterName, GetInstanceName(DataSourceName, Index), false);
        }
         * */

        public static void DeleteInstance(string InstanceName)
        {
            foreach (CounterCreationData ccd in ccdc)
            {
                PerformanceCounter pc = new PerformanceCounter(CategoryName, ccd.CounterName, InstanceName, false);
                pc.RemoveInstance();
            }
        }


        public static void CreateEventSource()
        {
            DeleteEventSource();
            EventLog.CreateEventSource(CategoryName, LogName);
        }

        public static void DeleteEventSource()
        {
            if (EventLog.SourceExists(CategoryName))
            {
                EventLog.DeleteEventSource(CategoryName);
            }

            if (EventLog.Exists(LogName))
            {
                EventLog.Delete(LogName);
            }
        }


        public static EventLog GetEventLog()
        {
            EventLog log = new EventLog(LogName);
            log.Source = CategoryName;
            return log;
        }

    }
}
