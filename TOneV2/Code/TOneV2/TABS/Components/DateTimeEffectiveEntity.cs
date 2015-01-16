using System;

namespace TABS.Components
{
    [Serializable]
    public abstract class DateTimeEffectiveEntity : BaseEntity, Interfaces.IDateTimeEffective
    {
        #region IDateTimeEffective

        protected Nullable<DateTime> _BeginEffectiveDate;
        protected Nullable<DateTime> _EndEffectiveDate;

        public virtual Nullable<DateTime> BeginEffectiveDate
        {
            get { return _BeginEffectiveDate; }
            set { _BeginEffectiveDate = value; }
        }

        public virtual Nullable<DateTime> EndEffectiveDate
        {
            get { return _EndEffectiveDate; }
            set { _EndEffectiveDate = value; }
        }

        public virtual bool IsEffective
        {
            get
            {
                return IsEffectiveOn(DateTime.Now);
            }
        }

        public virtual bool IsEffectiveOn(DateTime when)
        {
            return GetIsEffective(this, when);
        }

        public virtual void Deactivate(bool isRecursive, DateTime when)
        {
            EndEffectiveDate = when;
        }
        
        #endregion IDateTimeEffective

        /// <summary>
        /// Returns true if the instance is effective at the current date and time
        /// </summary>
        /// <param name="instance">The instance to check</param>
        /// <param name="when">When to checks</param>
        /// <returns>True if the instance is effective at the given date and time</returns>
        public static bool GetIsEffective(Interfaces.IDateTimeEffective instance, DateTime when)
        {
            bool isEffective = instance.BeginEffectiveDate.HasValue ? instance.BeginEffectiveDate.Value <= when : true;
            if (isEffective)
                isEffective = instance.EndEffectiveDate.HasValue ? instance.EndEffectiveDate.Value >= when : true;
            return isEffective;
        } 

        /// <summary>
        /// Check if the instance is effective "Now"
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool GetIsEffective(Interfaces.IDateTimeEffective instance)
        {
            return GetIsEffective(instance, DateTime.Now);
        }
    }
}
