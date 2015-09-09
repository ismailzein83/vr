using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ICustomerTariffDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<CustomerTariff> GetFilteredCustomerTariffs(Vanrise.Entities.DataRetrievalInput<CustomerTariffQuery> input);
        List<CustomerTariff> GetSaleTariffs(DateTime when);
    }
}
