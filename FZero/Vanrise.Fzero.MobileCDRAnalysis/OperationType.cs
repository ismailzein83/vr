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
    
    public partial class OperationType
    {
        public OperationType()
        {
            this.ControlTables = new HashSet<ControlTable>();
            this.ControlTables1 = new HashSet<ControlTable>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string SPName { get; set; }
        public Nullable<int> StepOrder { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<ControlTable> ControlTables { get; set; }
        public virtual ICollection<ControlTable> ControlTables1 { get; set; }
    }
}
