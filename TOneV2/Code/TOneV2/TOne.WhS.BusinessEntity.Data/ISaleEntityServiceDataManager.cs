using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleEntityServiceDataManager : IDataManager
    {
        IEnumerable<SaleEntityDefaultService> GetEffectiveSaleEntityDefaultServices(DateTime effectiveOn);
        IEnumerable<SaleEntityZoneService> GetEffectiveSaleEntityZoneServices(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn);
        IEnumerable<SaleEntityDefaultService> GetDefaultServicesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate);
        IEnumerable<SaleEntityZoneService> GetZoneServicesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate);
        bool AreSaleEntityServicesUpdated(ref object updateHandle);
    }
}
