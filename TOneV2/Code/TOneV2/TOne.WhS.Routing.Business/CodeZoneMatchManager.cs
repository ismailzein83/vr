using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
	public class CodeZoneMatchManager
	{
		#region Public Methods
       

		public IEnumerable<CodeSaleZoneMatch> GetSaleZonesMatchedToSupplierZones(IEnumerable<long> supplierZoneIds)
		{
			if (supplierZoneIds == null)
				throw new NullReferenceException("supplierZoneIds");
			ICodeZoneMatchDataManager dataManager = GetDataManager();
			return dataManager.GetSaleZonesMatchedToSupplierZones(supplierZoneIds);
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

        public IEnumerable<CodeSaleZoneMatch> GetSaleZoneMatchBySellingNumberPlanId(int sellingNumberPlanId,string codeStartWith)
        { 
            if(sellingNumberPlanId==null)
                throw new NullReferenceException("sellingNumberPlanId");
            ICodeZoneMatchDataManager dataManager = GetDataManager();
            return dataManager.GetSaleZoneMatchBySellingNumberPlanId(sellingNumberPlanId, codeStartWith);
        }

        public IEnumerable<CodeSupplierZoneMatch> GetSupplierZoneMatchBysupplierIds(IEnumerable<long> supplierIds, string codeStartWith)
        {
            if (supplierIds == null)
                throw new NullReferenceException("supplierIds");
            ICodeZoneMatchDataManager dataManager = GetDataManager();
            return dataManager.GetSupplierZoneMatchBysupplierIds(supplierIds, codeStartWith);
        }

		#endregion

		#region Private Methods

		private ICodeZoneMatchDataManager GetDataManager()
		{
			var dataManager = RoutingDataManagerFactory.GetDataManager<ICodeZoneMatchDataManager>();

			var routingDatabaseManager = new RoutingDatabaseManager();
			RoutingDatabase rpRouteDatabase = routingDatabaseManager.GetLatestRoutingDatabase(RoutingProcessType.RoutingProductRoute, RoutingDatabaseType.Current);
			if (rpRouteDatabase == null)
				throw new NullReferenceException("rpRouteDatabase");

			dataManager.RPRouteDatabase = rpRouteDatabase;
			return dataManager;
		}

		#endregion
	}
}
