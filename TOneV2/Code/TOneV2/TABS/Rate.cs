using System;

namespace TABS
{
    /// <summary>
    /// Defines Actual Rates given in Official Pricelists.
    /// </summary>
    [Serializable]
    public class Rate : RateBase, IComparable<TABS.Rate>
    {
        public override string Identifier { get { return "Rate:" + (ID > 0 ? ID.ToString() : (Zone != null ? Zone.Name : "Dummy Rate")); } }

        /// <summary>
        /// The non existing rate
        /// </summary>
        public static Rate None = new Rate();

        public virtual PriceList PriceList
        {
            get
            {
                return base.PriceListBase as PriceList;
            }
            set
            {
                base.PriceListBase = value;
            }
        }

        #region IComparable<Rate> Members

        public int CompareTo(Rate other)
        {
            return base.CompareTo(other);
        }

        #endregion
    }
}