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
    
    public partial class ControlTable
    {
        public int ID { get; set; }
        public int OperationTypeID { get; set; }
        public System.DateTime StartedDateTime { get; set; }
        public Nullable<System.DateTime> FinishedDateTime { get; set; }
        public Nullable<int> RowsPassed { get; set; }
        public Nullable<int> TotalRows { get; set; }
        public Nullable<int> StartID { get; set; }
        public Nullable<int> EndID { get; set; }
        public Nullable<System.DateTime> StartingUnitdate { get; set; }
        public Nullable<System.DateTime> EndingUnitdate { get; set; }
        public Nullable<int> Lastid { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> NumberOfProfileRecords { get; set; }
        public Nullable<int> StrategyId { get; set; }
        public Nullable<int> NumberOfCalls { get; set; }
    
        public virtual OperationType OperationType { get; set; }
        public virtual Period Period { get; set; }
    }
}
