using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface ISaleZoneManager : IBEManager
    {
		//List<SaleZone> GetPackageZones(int sellingNumberPlan);

		//List<SaleZone> GetZones(IEnumerable<long> zoneIds);

		SaleZone GetSaleZone(long saleZoneId);
    }
}
