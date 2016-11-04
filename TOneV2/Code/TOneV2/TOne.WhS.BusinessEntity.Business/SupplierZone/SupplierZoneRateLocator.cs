using System;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneRateLocator
    {
        ISupplierRateReader _reader;

        public SupplierZoneRateLocator(ISupplierRateReader reader)
        {
            _reader = reader;
        }

        public SupplierZoneRate GetSupplierZoneRate(int supplierId, long supplierZoneId, DateTime effectiveOn)
        {
            var supplierRates = _reader.GetSupplierRates(supplierId, effectiveOn);
            if (supplierRates != null)
            {
                SupplierZoneRate supplierZoneRate;
                if(supplierRates.TryGetValue(supplierZoneId, out supplierZoneRate))
                {
                    return supplierZoneRate;
                }
            }
            return null;
        }
    }
}