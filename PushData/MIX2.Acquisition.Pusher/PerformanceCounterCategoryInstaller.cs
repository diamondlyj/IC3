using System;
using System.Collections.Generic;
using System.Collections;

using System.Text;

using System.ComponentModel;
using System.Configuration.Install;


namespace MIX.Acquisition
{
    [RunInstallerAttribute(true)]
    public class PerformanceCounterCategoryInstaller : Installer
    {
        public override void Install(IDictionary stateSaver)
        {
            PerformanceReporter.CreateEventSource();
            PerformanceReporter.CreatePerformanceCounterCategory();
            base.Install(stateSaver);
        }


        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
            PerformanceReporter.DeletePerformanceCounterCategory();
            PerformanceReporter.DeleteEventSource();
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            PerformanceReporter.DeletePerformanceCounterCategory();
            PerformanceReporter.DeleteEventSource();
        }


    }
}
