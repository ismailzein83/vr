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
    
    public partial class Strategy_Suspicion_Level
    {
        public int Id { get; set; }
        public Nullable<int> StrategyId { get; set; }
        public Nullable<int> LevelId { get; set; }
        public Nullable<int> CriteriaId1 { get; set; }
        public Nullable<decimal> Cr1Per { get; set; }
        public Nullable<int> CriteriaId2 { get; set; }
        public Nullable<decimal> Cr2Per { get; set; }
        public Nullable<int> CriteriaId3 { get; set; }
        public Nullable<decimal> Cr3Per { get; set; }
        public Nullable<int> CriteriaId4 { get; set; }
        public Nullable<decimal> Cr4Per { get; set; }
        public Nullable<int> CriteriaId5 { get; set; }
        public Nullable<decimal> Cr5Per { get; set; }
        public Nullable<int> CriteriaId6 { get; set; }
        public Nullable<decimal> Cr6Per { get; set; }
        public Nullable<int> CriteriaId7 { get; set; }
        public Nullable<decimal> Cr7Per { get; set; }
        public Nullable<int> CriteriaId8 { get; set; }
        public Nullable<decimal> Cr8Per { get; set; }
        public Nullable<int> CriteriaId9 { get; set; }
        public Nullable<decimal> Cr9Per { get; set; }
        public Nullable<int> CriteriaId10 { get; set; }
        public Nullable<decimal> Cr10Per { get; set; }
        public Nullable<int> CriteriaId11 { get; set; }
        public Nullable<decimal> Cr11Per { get; set; }
        public Nullable<int> CriteriaId12 { get; set; }
        public Nullable<decimal> Cr12Per { get; set; }
        public Nullable<int> CriteriaId13 { get; set; }
        public Nullable<decimal> Cr13Per { get; set; }
        public Nullable<int> CriteriaId14 { get; set; }
        public Nullable<decimal> Cr14Per { get; set; }
        public Nullable<int> CriteriaId15 { get; set; }
        public Nullable<decimal> Cr15Per { get; set; }
    
        public virtual Strategy Strategy { get; set; }
        public virtual Strategy Strategy1 { get; set; }
        public virtual Suspicion_Level Suspicion_Level { get; set; }
        public virtual Suspicion_Level Suspicion_Level1 { get; set; }
    }
}
