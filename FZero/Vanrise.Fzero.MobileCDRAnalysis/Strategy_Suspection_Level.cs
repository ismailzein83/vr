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
    
    public partial class Strategy_Suspection_Level
    {
        public int Id { get; set; }
        public Nullable<int> StrategyId { get; set; }
        public Nullable<int> LevelId { get; set; }
        public Nullable<int> CriteriaId1 { get; set; }
        public Nullable<int> CriteriaId2 { get; set; }
        public Nullable<int> CriteriaId3 { get; set; }
        public Nullable<int> CriteriaId4 { get; set; }
        public Nullable<int> CriteriaId5 { get; set; }
        public Nullable<int> CriteriaId6 { get; set; }
    
        public virtual Strategy Strategy { get; set; }
        public virtual Suspection_Level Suspection_Level { get; set; }
    }
}
