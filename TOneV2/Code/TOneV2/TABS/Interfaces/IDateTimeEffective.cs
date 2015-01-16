using System;

namespace TABS.Interfaces
{
    public interface IDateTimeEffective
    {
        Nullable<DateTime> BeginEffectiveDate { get;  set; }
        Nullable<DateTime> EndEffectiveDate { get; set; }
        bool IsEffective { get; }
        
        /// <summary>
        /// Mark this instance as Inactive
        /// </summary>
        /// <param name="isRecursive">Recursively Deactivate child elements if any</param>
        /// <param name="when">When the instance will be End Effective</param>
        void Deactivate(bool isRecursive, DateTime when); 
    }
}