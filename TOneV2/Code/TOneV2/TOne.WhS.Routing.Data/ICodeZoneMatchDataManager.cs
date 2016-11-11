using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data
{
	public interface ICodeZoneMatchDataManager : IDataManager
	{
		RoutingDatabase RPRouteDatabase { get; set; }

		IEnumerable<CodeSaleZoneMatch> GetSaleZonesMatchedToSupplierZones(IEnumerable<long> supplierZoneIds);

		IEnumerable<CodeSupplierZoneMatch> GetSupplierZonesMatchedToSaleZones(IEnumerable<long> saleZoneIds, IEnumerable<int> supplierIds);

		IEnumerable<CodeSupplierZoneMatch> GetOtherSupplierZonesMatchedToSupplierZones(int supplierId, IEnumerable<long> supplierZoneIds, IEnumerable<int> otherSupplierIds);
	}
}
