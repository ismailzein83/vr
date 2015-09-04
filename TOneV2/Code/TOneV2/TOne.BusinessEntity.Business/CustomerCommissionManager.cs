using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;

namespace TOne.BusinessEntity.Business
{
    public class CustomerCommissionManager
    {
        public Vanrise.Entities.IDataRetrievalResult<string> GetCustomerCommissions(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            ICustomerCommissionDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerCommissionDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetCustomerCommissions(input));
        }
    }
}
