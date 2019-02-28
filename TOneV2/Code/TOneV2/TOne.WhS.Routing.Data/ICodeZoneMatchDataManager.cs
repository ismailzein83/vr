using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data
{
    public interface ICodeZoneMatchDataManager : IDataManager
	{
		RoutingDatabase RPRouteDatabase { get; set; }

		IEnumerable<SaleZoneDefintion> GetSaleZonesMatchedToSupplierZones(IEnumerable<long> supplierZoneIds, int? sellingNumberPlanId);

		IEnumerable<CodeSupplierZoneMatch> GetSupplierZonesMatchedToSaleZones(IEnumerable<long> saleZoneIds, IEnumerable<int> supplierIds);

		IEnumerable<CodeSupplierZoneMatch> GetOtherSupplierZonesMatchedToSupplierZones(int supplierId, IEnumerable<long> supplierZoneIds, IEnumerable<int> otherSupplierIds);

        IEnumerable<CodeSupplierZoneMatch> GetSupplierZoneMatchBysupplierIds(IEnumerable<long> supplierIds, string codeStartWith);

        IEnumerable<CodeSaleZoneMatch> GetSaleZoneMatchBySellingNumberPlanId(int sellingNumberPlanId, string codeStartWith);

        IEnumerable<CodeSupplierZoneMatch> GetSupplierZoneMatchBySupplierIdsAndSellingNumberPanId(int sellingNumberPlanId, IEnumerable<long> supplierIds, string codeStartWith);
	}
}