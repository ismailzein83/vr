using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;

namespace TOne.BusinessEntity.Business
{
    public class SupplierCommissionManager
    {
        public Vanrise.Entities.IDataRetrievalResult<string> GetSupplierCommissions(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            ISupplierCommissionDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierCommissionDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetSupplierCommissions(input));
        }
    }
}
