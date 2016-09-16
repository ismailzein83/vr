using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityServiceManager
    {
        public IEnumerable<SaleEntityDefaultService> GetDefaultServicesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            var dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityServiceDataManager>();
            return dataManager.GetDefaultServicesEffectiveAfter(ownerType, ownerId, minimumDate);
        }

        public IEnumerable<SaleEntityZoneService> GetZoneServicesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            var dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityServiceDataManager>();
            return dataManager.GetZoneServicesEffectiveAfter(ownerType, ownerId, minimumDate);
        }

        public int GetSaleEntityServiceTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSaleEntityServiceType());
        }

        public Type GetSaleEntityServiceType()
        {
            return this.GetType();
        }
    }
}
