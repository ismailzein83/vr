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
    
    public partial class Period
    {
        public Period()
        {
            this.StrategyPeriods = new HashSet<StrategyPeriod>();
        }
    
        public int Id { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<StrategyPeriod> StrategyPeriods { get; set; }
    }
}
