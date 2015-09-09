using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ISupplierTODDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<SupplierTODConsiderationInfo> GetSupplierToDConsiderationByCriteria(Vanrise.Entities.DataRetrievalInput<TODQuery> input, List<string> suppliersAMUIds);
        List<SupplierTODConsiderationInfo> GetSuppliersToDConsideration(DateTime when);
    }
}
