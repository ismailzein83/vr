using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierCodeDataManager:IDataManager
    {
        List<SupplierCode> GetSupplierCodesEffectiveAfter(int supplierId, DateTime minimumDate);
        List<SupplierCode> GetSupplierCodes(int supplierId, DateTime effectiveOn);

        List<SupplierCode> GetSupplierCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture);
    }
}
