using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class SupplierTariffManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SupplierTariff> GetFilteredSupplierTariffs(Vanrise.Entities.DataRetrievalInput<SupplierTariffQuery> input)
        {
            ISupplierTariffDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierTariffDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredSupplierTariffs(input));
        }
    }
}
