using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface IToDConsiderationDataManager : IDataManager
    {
        List<ToDConsideration> GetToDConsideration(string customerId, int zoneId, DateTime when);
        Vanrise.Entities.BigResult<BaseTODConsiderationInfo> GetCustomerToDConsiderationByCriteria(Vanrise.Entities.DataRetrievalInput<TODQuery> input);

        Vanrise.Entities.BigResult<BaseTODConsiderationInfo> GetSupplierToDConsiderationByCriteria(Vanrise.Entities.DataRetrievalInput<TODQuery> input , List<string> suppliersAMUids);
    }
}
