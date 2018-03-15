using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IANumberSupplierCodeDataManager : IDataManager
    {
        IEnumerable<ANumberSupplierCode> GetFilteredANumberSupplierCodes(int aNumberGroupID, List<int> supplierIds);
        IEnumerable<ANumberSupplierCode> GetEffectiveAfterBySupplierId(int supplierId, DateTime effectiveOn);
        bool Insert(List<ANumberSupplierCodeToClose> listOfSupplierCodesToClose, long aNumberSupplierCodeId, int aNumberGroupID, int supplierID, string code, DateTime effectiveOn);
        ANumberSupplierCode GetANumberSupplierCode(long aNumberSupplierCodeId);
        bool Close(long aNumberSupplierCodeId, DateTime effectiveOn);

    }
}
