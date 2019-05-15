using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Deal.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class CodeZoneMatchManager
    {
        #region Public Methods

        public IEnumerable<SaleZoneDefintion> GetSaleZonesMatchedToSupplierZones(IEnumerable<long> supplierZoneIds, int? sellingNumberPlanId = null, bool continueIfDBNotFound = false)
        {
            if (supplierZoneIds == null)
                throw new NullReferenceException("supplierZoneIds");

            ICodeZoneMatchDataManager dataManager = GetDataManager(continueIfDBNotFound);
            if (dataManager == null)
                return null;

            return dataManager.GetSaleZonesMatchedToSupplierZones(supplierZoneIds, sellingNumberPlanId);
        }

        public List<long> GetSaleZonesMatchingSupplierDeals(List<int> selectedSupplierDealIds, int sellingNumberPlanId, List<long> selectedSaleZoneIds)
        {
            if (selectedSupplierDealIds == null || selectedSupplierDealIds.Count == 0 || selectedSaleZoneIds == null || selectedSaleZoneIds.Count == 0)
                return null;

            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();

            List<long> allDealSupplierZoneIds = new List<long>();
            foreach (var selectedSupplierDealId in selectedSupplierDealIds)
            {
                var supplierDeal = dealDefinitionManager.GetDealDefinition(selectedSupplierDealId, true);
                List<long> supplierZoneIds = supplierDeal.Settings.GetDealSupplierZoneIds();
                if (supplierZoneIds != null)
                    allDealSupplierZoneIds.AddRange(supplierZoneIds);
            }

            var matchingSaleZoneDefinitions = GetSaleZonesMatchedToSupplierZones(allDealSupplierZoneIds, sellingNumberPlanId, true);
            if (matchingSaleZoneDefinitions == null || matchingSaleZoneDefinitions.Count() == 0)
                return null;

            List<long> matchingSaleZoneIds = matchingSaleZoneDefinitions.Select(itm => itm.SaleZoneId).ToList();

            List<long> results = new List<long>();

            foreach (var selectedSaleZoneId in selectedSaleZoneIds)
            {
                if (matchingSaleZoneIds.Contains(selectedSaleZoneId))
                    results.Add(selectedSaleZoneId);
            }

            return results.Count > 0 ? results : null;
        }

        public IEnumerable<CodeSupplierZoneMatch> GetSupplierZonesMatchedToSaleZones(IEnumerable<long> saleZoneIds, IEnumerable<int> supplierIds)
        {
            if (saleZoneIds == null)
                throw new NullReferenceException("saleZoneIds");
            ICodeZoneMatchDataManager dataManager = GetDataManager();
            return dataManager.GetSupplierZonesMatchedToSaleZones(saleZoneIds, supplierIds);
        }

        public IEnumerable<CodeSupplierZoneMatch> GetOtherSupplierZonesMatchedToSupplierZones(int supplierId, IEnumerable<long> supplierZoneIds, IEnumerable<int> otherSupplierIds)
        {
            if (supplierZoneIds == null)
                throw new NullReferenceException("supplierZoneIds");
            ICodeZoneMatchDataManager dataManager = GetDataManager();
            return dataManager.GetOtherSupplierZonesMatchedToSupplierZones(supplierId, supplierZoneIds, otherSupplierIds);
        }

        public IEnumerable<CodeSaleZoneMatch> GetSaleCodeMatchBySellingNumberPlanId(int sellingNumberPlanId, string codeStartWith)
        {
            if (sellingNumberPlanId == null)
                throw new NullReferenceException("sellingNumberPlanId");
            ICodeZoneMatchDataManager dataManager = GetDataManager();
            return dataManager.GetSaleZoneMatchBySellingNumberPlanId(sellingNumberPlanId, codeStartWith);
        }

        public IEnumerable<CodeSupplierZoneMatch> GetSupplierCodeMatchBysupplierIds(IEnumerable<long> supplierIds, string codeStartWith)
        {
            if (supplierIds == null)
                throw new NullReferenceException("supplierIds");
            ICodeZoneMatchDataManager dataManager = GetDataManager();
            return dataManager.GetSupplierZoneMatchBysupplierIds(supplierIds, codeStartWith);
        }

        public IEnumerable<CodeSupplierZoneMatch> GetSupplierZoneMatchBysupplierIdsAndSellingNumberPanId(int sellingNumberPlanId, IEnumerable<long> supplierIds, string codeStartWith)
        {
            if (supplierIds == null)
                throw new NullReferenceException("supplierIds");
            if (sellingNumberPlanId == null)
                throw new NullReferenceException("sellingNumberPlanId");

            ICodeZoneMatchDataManager dataManager = GetDataManager();
            return dataManager.GetSupplierZoneMatchBySupplierIdsAndSellingNumberPanId(sellingNumberPlanId, supplierIds, codeStartWith);
        }

        #endregion

        #region Private Methods

        private ICodeZoneMatchDataManager GetDataManager(bool continueIfDBNotFound = false)
        {
            RoutingDatabase rpRouteDatabase = new RoutingDatabaseManager().GetLatestRoutingDatabase(RoutingProcessType.RoutingProductRoute, RoutingDatabaseType.Current);
            if (rpRouteDatabase == null)
            {
                if (!continueIfDBNotFound)
                    throw new NullReferenceException("rpRouteDatabase");
                else
                    return null;
            }

            var dataManager = RoutingDataManagerFactory.GetDataManager<ICodeZoneMatchDataManager>();
            dataManager.RPRouteDatabase = rpRouteDatabase;
            return dataManager;
        }

        #endregion
    }
}