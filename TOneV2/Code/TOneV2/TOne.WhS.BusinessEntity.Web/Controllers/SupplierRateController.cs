using System.Web.Http;
using TOne.WhS.BusinessEntity.APIEntities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
	[JSONWithTypeAttribute]
	[RoutePrefix(Constants.ROUTE_PREFIX + "SupplierRate")]
	public class SupplierRateController : Vanrise.Web.Base.BaseAPIController
	{
		[HttpPost]
		[Route("GetFilteredSupplierRates")]
		public object GetFilteredSupplierRates(Vanrise.Entities.DataRetrievalInput<BaseSupplierRateQueryHandler> input)
		{
			SupplierRateManager manager = new SupplierRateManager();
			return GetWebResponse(input, manager.GetFilteredSupplierRates(input), "Supplier Rates");
		}

		[HttpPost]
		[Route("GetSupplierRateQueryHandlerInfo")]
		public object GetSupplierRateQueryHandlerInfo(Vanrise.Entities.DataRetrievalInput<SupplierRateQueryHandlerInfo> supplierRateQueryHandlerInfo)
		{
			SupplierRateQueryHandler supplierRateQuery = new SupplierRateQueryHandler
			{
				Query = new Entities.SupplierRateQuery
				{
					SupplierId = supplierRateQueryHandlerInfo.Query.SupplierId,
					SupplierZoneName= supplierRateQueryHandlerInfo.Query.SupplierZoneName,
					CountriesIds= supplierRateQueryHandlerInfo.Query.CountriesIds
				},
				EffectiveOn = supplierRateQueryHandlerInfo.Query.EffectiveOn,
			};


			DataRetrievalInput<BaseSupplierRateQueryHandler> mappedInput = new DataRetrievalInput<BaseSupplierRateQueryHandler>
			{
				IsAPICall = supplierRateQueryHandlerInfo.IsAPICall,
				GetSummary = supplierRateQueryHandlerInfo.GetSummary,
				IsSortDescending = supplierRateQueryHandlerInfo.IsSortDescending,
				ResultKey = supplierRateQueryHandlerInfo.ResultKey,
				SortByColumnName = supplierRateQueryHandlerInfo.SortByColumnName,
				DataRetrievalResultType = supplierRateQueryHandlerInfo.DataRetrievalResultType,
				FromRow = supplierRateQueryHandlerInfo.FromRow,
				ToRow = supplierRateQueryHandlerInfo.ToRow,
				Query = supplierRateQuery,
			};
			return this.GetFilteredSupplierRates(mappedInput);
		}

		[HttpPost]
		[Route("GetSupplierRateHistoryQueryHandlerInfo")]
		public object GetSupplierRateHistoryQueryHandlerInfo(Vanrise.Entities.DataRetrievalInput<SupplierRateHistoryQueryHandlerInfo> supplierRateHistoryQueryHandlerInfo)
		{
			SupplierRateHistoryQueryHandler supplierRateHistoryQuery = new SupplierRateHistoryQueryHandler
			{
				Query = new SupplierRateHistoryQuery
				{
					SupplierId = supplierRateHistoryQueryHandlerInfo.Query.SupplierId,
					SupplierZoneName = supplierRateHistoryQueryHandlerInfo.Query.SupplierZoneName,
				},
				EffectiveOn = supplierRateHistoryQueryHandlerInfo.Query.EffectiveOn,
			};

			DataRetrievalInput<BaseSupplierRateQueryHandler> mappedInput = new DataRetrievalInput<BaseSupplierRateQueryHandler>
			{
				IsAPICall = supplierRateHistoryQueryHandlerInfo.IsAPICall,
				GetSummary = supplierRateHistoryQueryHandlerInfo.GetSummary,
				IsSortDescending = supplierRateHistoryQueryHandlerInfo.IsSortDescending,
				ResultKey = supplierRateHistoryQueryHandlerInfo.ResultKey,
				SortByColumnName = supplierRateHistoryQueryHandlerInfo.SortByColumnName,
				DataRetrievalResultType = supplierRateHistoryQueryHandlerInfo.DataRetrievalResultType,
				FromRow = supplierRateHistoryQueryHandlerInfo.FromRow,
				ToRow = supplierRateHistoryQueryHandlerInfo.ToRow,
				Query = supplierRateHistoryQuery,
			};
			return this.GetFilteredSupplierRates(mappedInput);
		}
	}

}