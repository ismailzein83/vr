//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vanrise.Fzero.CDRAnalysis
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserPermission
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int PermissionID { get; set; }
        public System.DateTime CreationDate { get; set; }
        public int CreatedBy { get; set; }
    
        public virtual Permission Permission { get; set; }
        public virtual User User { get; set; }
    }
}
