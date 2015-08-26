using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class CustomerTariffManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CustomerTariff> GetFilteredCustomerTariffs(Vanrise.Entities.DataRetrievalInput<CustomerTariffQuery> input)
        {
            ICustomerTariffDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerTariffDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredCustomerTariffs(input));
        }
    }
}
