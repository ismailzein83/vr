using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;


namespace TOne.BusinessEntity.Business
{
    public class SupplierTODManager : BaseTODManager<SupplierTODConsiderationInfo>
    {
        public Vanrise.Entities.IDataRetrievalResult<SupplierTODConsiderationInfo> GetSupplierTODFiltered(Vanrise.Entities.DataRetrievalInput<TODQuery> input)
        {
            AccountManagerManager am = new AccountManagerManager();
            List<string> suppliersAMUIds = am.GetMyAssignedSupplierIds();
            ISupplierTODDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierTODDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetSupplierToDConsiderationByCriteria(input, suppliersAMUIds));
        }
    }
}
