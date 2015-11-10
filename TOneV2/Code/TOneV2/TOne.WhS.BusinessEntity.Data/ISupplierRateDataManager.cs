using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierRateDataManager:IDataManager
    {
        List<SupplierRate> GetSupplierRates(int supplierId,DateTime minimumDate);

        List<SupplierRate> GetAllSupplierRates(DateTime? effectiveOn, bool isEffectiveInFuture);
        List<SupplierRate> GetEffectiveSupplierRates(int supplierId, DateTime effectiveDate);
        bool AreSupplierRatesUpdated(ref object updateHandle);
    }
}
