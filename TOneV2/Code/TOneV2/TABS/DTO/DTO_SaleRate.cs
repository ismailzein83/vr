using System;

namespace TABS.DTO
{
	public class DTO_SaleRate : DTO_Rate, IComparable<DTO_SaleRate>
	{
		public TABS.CarrierAccount Customer { get; set; }
        public TABS.Zone Zone { get; set; }

        #region IComparable<DTO_SupplyRate> Members

        public int CompareTo(DTO_SaleRate other)
        {
            return base.CompareTo(other);
        }

        #endregion
    }
}
