using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business
{
    public class SupplierTariffManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SupplierTariff> GetFilteredSupplierTariffs(Vanrise.Entities.DataRetrievalInput<SupplierTariffQuery> input)
        {
            ISupplierTariffDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<ISupplierTariffDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredSupplierTariffs(input));
        }
    }
}
