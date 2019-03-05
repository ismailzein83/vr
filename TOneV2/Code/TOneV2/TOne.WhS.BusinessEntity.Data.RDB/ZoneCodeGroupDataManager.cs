using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.RDB;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class ZoneCodeGroupDataManager : IZoneCodeGroupDataManager
    {
        public List<ZoneCodeGroup> GetCostZoneCodeGroups(DateTime? effectiveOn, bool isFuture)
        {
            return new SupplierCodeDataManager().GetCostZoneCodeGroups(effectiveOn, isFuture);
        }

        public List<ZoneCodeGroup> GetSaleZoneCodeGroups(DateTime? effectiveOn, bool isFuture)
        {
            return new SaleCodeDataManager().GetSaleZoneCodeGroups(effectiveOn, isFuture);
        }
    }
}
