//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vanrise.Fzero.MobileCDRAnalysis
{
    using System;
    using System.Collections.Generic;
    
    public partial class Switch_DatabaseConnections
    {
        public int Id { get; set; }
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string UserId { get; set; }
        public string UserPassword { get; set; }
    
        public virtual SwitchProfile SwitchProfile { get; set; }
        public virtual SwitchProfile SwitchProfile1 { get; set; }
    }
}
