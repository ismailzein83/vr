using System;

namespace TABS.Components
{
    [Serializable]
    public abstract class DateTimeEffectiveFlaggedServicesEntity: FlaggedServicesEntity, Interfaces.IDateTimeEffective
    {
        #region IDateTimeEffective Members

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

        public virtual void Deactivate(bool isRecursive, DateTime when)
        {
            EndEffectiveDate = when;
        }

        public virtual bool IsEffective
        {
            get
            {
                return DateTimeEffectiveEntity.GetIsEffective(this);
            }
        }

        public virtual bool IsEffectiveOn(DateTime when)
        {
            return DateTimeEffectiveEntity.GetIsEffective(this, when);
        }

        #endregion
    }
}
