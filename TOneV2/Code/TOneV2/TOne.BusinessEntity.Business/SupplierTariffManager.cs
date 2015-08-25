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
            Vanrise.Entities.BigResult<SupplierTariff> tariffs=dataManager.GetFilteredSupplierTariffs(input);
            GetNames(tariffs);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, tariffs);
        }
        private void GetNames(Vanrise.Entities.BigResult<SupplierTariff> tariffs)
        {
            BusinessEntityInfoManager manager = new BusinessEntityInfoManager();

            foreach (SupplierTariff tariff in tariffs.Data)
            {
                if (tariff.ZoneID != 0)
                    tariff.ZoneName = manager.GetZoneName(tariff.ZoneID);

                if (tariff.SupplierID != null)
                    tariff.SupplierName = manager.GetCarrirAccountName(tariff.SupplierID);
            }
        }
    }
}
