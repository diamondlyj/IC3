//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AIn.Ventures.BaseLibrary
{
    using System;
    using System.Collections.Generic;
    
    public partial class Stakeholder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Stakeholder()
        {
            this.Projects = new HashSet<Project>();
        }
    
        public System.Guid GUID { get; set; }
        public Nullable<double> Shares { get; set; }
        public Nullable<System.Guid> ProjectGUID { get; set; }
        public bool IsUser { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Project> Projects { get; set; }
        public virtual Project Project { get; set; }
    }
}
