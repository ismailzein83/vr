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
    
    public partial class Suspection_Level
    {
        public Suspection_Level()
        {
            this.Strategy_Suspection_Level = new HashSet<Strategy_Suspection_Level>();
            this.SubscriberThresholds = new HashSet<SubscriberThreshold>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<Strategy_Suspection_Level> Strategy_Suspection_Level { get; set; }
        public virtual ICollection<SubscriberThreshold> SubscriberThresholds { get; set; }
    }
}
