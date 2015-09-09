using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ISupplierCommissionDataManager:IDataManager
    {
        Vanrise.Entities.BigResult<SupplierCommission> GetSupplierCommissions(Vanrise.Entities.DataRetrievalInput<SupplierCommissionQuery> input);
        List<SupplierCommission> GetSupplierCommissions(DateTime when);
    }
}
