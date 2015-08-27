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
        public Vanrise.Entities.IDataRetrievalResult<SupplierTODConsiderationInfo> GetSupplierTODInfos(Vanrise.Entities.DataRetrievalInput<TODQuery> input)
        {

            ISupplierTODDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierTODDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetSupplierTODInfos(input));
        }
    }
}
