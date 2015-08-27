using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class TODManager
    {
        IToDConsiderationDataManager _dataManager;

        public TODManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<IToDConsiderationDataManager>();
        }

        public Vanrise.Entities.IDataRetrievalResult<BaseTODConsiderationInfo> GetFilteredCustomerTOD(Vanrise.Entities.DataRetrievalInput<TODQuery> input)
        {
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _dataManager.GetCustomerToDConsiderationByCriteria(input));
        }


        public Vanrise.Entities.IDataRetrievalResult<BaseTODConsiderationInfo> GetFilteredSupplierTOD(Vanrise.Entities.DataRetrievalInput<TODQuery> input)
        {
             AccountManagerManager am = new AccountManagerManager();

            List<string> suppliersAMUIds = am.GetMyAssignedSupplierIds();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _dataManager.GetSupplierToDConsiderationByCriteria(input, suppliersAMUIds));
        }
    }
}
