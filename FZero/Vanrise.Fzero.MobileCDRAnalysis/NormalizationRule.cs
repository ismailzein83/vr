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
    
    public partial class NormalizationRule
    {
        public int Id { get; set; }
        public string Prefix { get; set; }
        public Nullable<int> CallLength { get; set; }
        public Nullable<decimal> Durations { get; set; }
        public Nullable<int> CallsCount { get; set; }
        public Nullable<int> Ignore { get; set; }
        public string Party { get; set; }
        public Nullable<int> SwitchId { get; set; }
        public string SwitchName { get; set; }
        public string PrefixToAdd { get; set; }
        public Nullable<int> SubstringStartIndex { get; set; }
        public Nullable<int> SubstringLength { get; set; }
        public string SuffixToAdd { get; set; }
    
        public virtual SwitchProfile SwitchProfile { get; set; }
        public virtual SwitchProfile SwitchProfile1 { get; set; }
    }
}
