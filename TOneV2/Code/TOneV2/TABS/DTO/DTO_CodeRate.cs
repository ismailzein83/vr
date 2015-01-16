using System.Collections.Generic;

namespace TABS.DTO
{
    public class DTO_CodeRate : DTO_Rate
    {
        public Zone _OurZone;
        public List<DTO_SupplyRate> _SupplierRates;
        public List<DTO_SaleRate> _SaleRates;
        public Rate OurRate { get; set; } // used only for commission 
        public Zone OurZone { get { return _OurZone; } set { _OurZone = value; } }
        public string OurCode { get; set; }
        public List<DTO_SupplyRate> SupplierRates { get { return _SupplierRates; } set { _SupplierRates = value; } }
        public List<DTO_SaleRate> SaleRates { get { return _SaleRates; } set { _SaleRates = value; } }

        public int SupplyCount { get { return (_SupplierRates != null) ? _SupplierRates.Count : 0; } }
        public int SaleCount { get { return (_SaleRates != null) ? _SaleRates.Count : 0; } }

        public int ValidSupplyCount
        {
            get
            {
                if (_SupplierRates == null) return 0;
                else
                {
                    int count = 0;
                    foreach (DTO_SupplyRate supplyRate in _SupplierRates)
                        if (supplyRate.Normal < this.Normal)
                            count++;
                    return count;
                }
            }
        }
    }
}
