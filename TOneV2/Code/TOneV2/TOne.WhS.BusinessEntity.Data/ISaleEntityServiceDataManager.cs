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
        bool AreSaleEntityServicesUpdated(ref object updateHandle);
    }
}
