using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIn.Ventures.BaseLibrary.Models
{
    public class PopulateDBModel
    {
        public List<Component> ComponentList { get; set; }
        public List<ComponentToComponent> CtcList { get; set; }
        public List<Project> ProjectList { get; set; }
       // public List<AIn.Ventures.BaseLibrary.ProjectToProduct> PtpList { get; set; }
    }

    public class PopulateProjectsModel
    {
        public List<Project> ProjectList { get; set; }

    }
}
