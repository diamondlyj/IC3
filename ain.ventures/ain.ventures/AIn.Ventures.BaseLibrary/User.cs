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
    
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.RoleToProjects = new HashSet<RoleToProject>();
            this.Colleagues = new HashSet<Colleague>();
            this.Colleagues1 = new HashSet<Colleague>();
        }
    
        public System.Guid GUID { get; set; }
        public string GivenNames { get; set; }
        public string Surname { get; set; }
        public string Pwd { get; set; }
        public bool IsConfirmed { get; set; }
        public System.DateTime Created { get; set; }
        public string EmailAddress { get; set; }
        public int SystemRights { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoleToProject> RoleToProjects { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Colleague> Colleagues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Colleague> Colleagues1 { get; set; }
    }
}
