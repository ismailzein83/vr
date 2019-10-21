using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data.RDB
{
    public class CodeZoneMatchDataManager : RoutingDataManager, ICodeZoneMatchDataManager
    {
        public RoutingDatabase RPRouteDatabase { get; set; }

        public IEnumerable<SaleZoneDefintion> GetSaleZonesMatchedToSupplierZones(IEnumerable<long> supplierZoneIds, int? sellingNumberPlanId)
        {
            return new CodeSaleZoneMatchDataManager().GetSaleZonesMatchedToSupplierZones(supplierZoneIds, sellingNumberPlanId);
        }

        public IEnumerable<CodeSaleZoneMatch> GetSaleZoneMatchBySellingNumberPlanId(int sellingNumberPlanId, string codeStartWith)
        {
            return new CodeSaleZoneMatchDataManager().GetSaleZoneMatchBySellingNumberPlanId(sellingNumberPlanId, codeStartWith);
        }

        public IEnumerable<CodeSupplierZoneMatch> GetSupplierZoneMatchBysupplierIds(IEnumerable<long> supplierIds, string codeStartWith)
        {
            return new CodeSupplierZoneMatchDataManager().GetSupplierZoneMatchBysupplierIds(supplierIds, codeStartWith);
        }

        public IEnumerable<CodeSupplierZoneMatch> GetSupplierZoneMatchBySupplierIdsAndSellingNumberPanId(int sellingNumberPlanId, IEnumerable<long> supplierIds, string codeStartWith)
        {
            return new CodeSupplierZoneMatchDataManager().GetSupplierZoneMatchBySupplierIdsAndSellingNumberPanId(sellingNumberPlanId, supplierIds, codeStartWith);
        }

        public IEnumerable<CodeSupplierZoneMatch> GetSupplierZonesMatchedToSaleZones(IEnumerable<long> saleZoneIds, IEnumerable<int> supplierIds)
        {
            return new CodeSupplierZoneMatchDataManager().GetSupplierZonesMatchedToSaleZones(saleZoneIds, supplierIds);
        }

        public IEnumerable<CodeSupplierZoneMatch> GetOtherSupplierZonesMatchedToSupplierZones(int supplierId, IEnumerable<long> supplierZoneIds, IEnumerable<int> otherSupplierIds)
        {
            return new CodeSupplierZoneMatchDataManager().GetOtherSupplierZonesMatchedToSupplierZones(supplierId, supplierZoneIds, otherSupplierIds);
        }
    }
}