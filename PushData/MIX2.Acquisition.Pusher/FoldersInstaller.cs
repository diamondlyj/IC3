using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using System.ComponentModel;
using System.Configuration.Install;

namespace MIX.Acquisition
{
 

    [RunInstallerAttribute(true)]
    public class FoldersInstaller : Installer
    {
        protected void CreateApplicationDataFolder()
        {
//            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)

        }
        
        public override void Install(IDictionary stateSaver)
        {

            base.Install(stateSaver);
        }


        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);

        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
 
        }


    }
}
