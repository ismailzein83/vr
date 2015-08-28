using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;


namespace TOne.BusinessEntity.Business
{
    public class CustomerTODManager : BaseTODManager<CustomerTODConsiderationInfo>
    {
        public Vanrise.Entities.IDataRetrievalResult<CustomerTODConsiderationInfo> GetCustomerTODFiltered(Vanrise.Entities.DataRetrievalInput<TODQuery> input)
        {

            ICustomerTODDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerTODDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetCustomerToDConsiderationByCriteria(input));
        }
    }
}
