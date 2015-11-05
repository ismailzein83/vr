using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneRateLocator
    {
        ISupplierRateReader _reader;
        SupplierPriceListManager _supplierPriceListManager;

        public SupplierZoneRateLocator(ISupplierRateReader reader)
        {
            _reader = reader;
            _supplierPriceListManager = new SupplierPriceListManager();
        }

        public SupplierZoneRate GetSupplierZoneRate(int supplierId, long supplierZoneId)
        {
            SupplierZoneRate supplierZoneRate = null;
            var supplierRates = _reader.GetSupplierRates(supplierId);
            if (supplierRates != null)
            {
                SupplierRate supplierRate;
                if(supplierRates.TryGetValue(supplierZoneId, out supplierRate))
                {
                    supplierZoneRate = new SupplierZoneRate
                    {
                         Rate = supplierRate,
                         PriceList = _supplierPriceListManager.GetPriceList(supplierRate.PriceListId)
                    };
                }
            }
            return supplierZoneRate;
        }
    }
}
