//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vanrise.Fzero.CDRAnalysis.Mobile
{
    using System;
    using System.Collections.Generic;
    
    public partial class Permission
    {
        public Permission()
        {
            this.Permissions1 = new HashSet<Permission>();
            this.UserPermissions = new HashSet<UserPermission>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public Nullable<int> ParentID { get; set; }
    
        public virtual ICollection<Permission> Permissions1 { get; set; }
        public virtual Permission Permission1 { get; set; }
        public virtual ICollection<UserPermission> UserPermissions { get; set; }
    }
}
