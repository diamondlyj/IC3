using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Deployment.WindowsInstaller;

public class CustomActions
{
    [CustomAction]
    public static ActionResult CreateCounters(Session session)
    {
        //return ActionResult.Success;

        string source = "MIX2.Action";

        bool hasCorrectCounters = true;
        bool categoryExists = true;

        if (!PerformanceCounterCategory.Exists("MIX2.Action"))
        {
            hasCorrectCounters = false;
            categoryExists = false;
        }
        else
        {
            PerformanceCounterCategory cat = new PerformanceCounterCategory("MIX2.Action");

            if (!cat.CounterExists("Objects read/second") || !cat.CounterExists("Categorizations/second") || cat.CategoryType != PerformanceCounterCategoryType.MultiInstance)
                hasCorrectCounters = false;
        }

        System.Diagnostics.EventLog.WriteEntry(source, "Checked counters: " + ((hasCorrectCounters) ? "correct" : "incorrect"), EventLogEntryType.Warning);

        if (!hasCorrectCounters)
        {
            if (categoryExists)
            {
                PerformanceCounterCategory.Delete("MIX2.Action");
                System.Diagnostics.EventLog.WriteEntry(source, "Deleted performance counters.", EventLogEntryType.Warning);
            }

            CounterCreationDataCollection counters = new CounterCreationDataCollection();

            CounterCreationData rc = new CounterCreationData("Objects read/second", "Measures number of read objects per second.", PerformanceCounterType.RateOfCountsPerSecond64);
            counters.Add(rc);

            CounterCreationData cc = new CounterCreationData("Objects categorized/second", "Measures number of objects categorized per second.", PerformanceCounterType.RateOfCountsPerSecond64);
            counters.Add(cc);

            PerformanceCounterCategory.Create("MIX2.Action", "Measurements of processes that catalog objects in MIX2.", PerformanceCounterCategoryType.MultiInstance, counters);

            System.Diagnostics.EventLog.WriteEntry(source, "Recreated performance counters.", EventLogEntryType.Warning);

            return ActionResult.Success;
        }

        return ActionResult.Success;
    }

}
