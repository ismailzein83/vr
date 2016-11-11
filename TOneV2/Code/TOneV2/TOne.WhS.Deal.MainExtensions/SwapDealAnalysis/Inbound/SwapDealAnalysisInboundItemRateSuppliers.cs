using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.MainExtensions
{
	public class SwapDealAnalysisInboundItemRateSuppliers : SwapDealAnalysisInboundItemRateCalcMethod
	{
		public CarrierFilterType SupplierFilterType { get; set; }

		public IEnumerable<int> SupplierIds { get; set; }

		public ZoneRateEvaluationType RateEvaluationType { get; set; }

		public override decimal? Execute(ISwapDealAnalysisInboundRateCalcMethodContext context)
		{
			var codeZoneMatchManager = new CodeZoneMatchManager();
			IEnumerable<int> supplierIds = GetSupplierIds();
			IEnumerable<CodeSupplierZoneMatch> codeSupplierZoneMatches = codeZoneMatchManager.GetSupplierZonesMatchedToSaleZones(context.SaleZoneIds, supplierIds);

			if (codeSupplierZoneMatches == null || codeSupplierZoneMatches.Count() == 0)
				return null;

			var supplierRates = new List<decimal>();
			var supplierRateManager = new SupplierRateManager();
			DateTime effectiveOn = DateTime.Now;

			foreach (CodeSupplierZoneMatch codeSupplierZoneMatch in codeSupplierZoneMatches)
			{
				SupplierZoneRate supplierRate = supplierRateManager.GetCachedSupplierZoneRate(codeSupplierZoneMatch.SupplierId, codeSupplierZoneMatch.SupplierZoneId, effectiveOn);
				if (supplierRate == null || supplierRate.Rate == null)
					continue;
				supplierRates.Add(supplierRate.Rate.Rate);
			}

			return GetCalculatedRate(supplierRates);
		}

		#region Private Methods

		private IEnumerable<int> GetSupplierIds()
		{
			switch (SupplierFilterType)
			{
				case CarrierFilterType.All:
					return null;
				case CarrierFilterType.Specific:
					RequireSupplierIds();
					return SupplierIds;
				case CarrierFilterType.AllExcept:
					RequireSupplierIds();
					var carrierAccountManager = new CarrierAccountManager();
					IEnumerable<CarrierAccount> allSuppliers = carrierAccountManager.GetAllSuppliers();
					if (allSuppliers == null)
						throw new NullReferenceException("allSuppliers");
					return allSuppliers.MapRecords(x => x.CarrierAccountId, x => !SupplierIds.Contains(x.CarrierAccountId));
			}
			throw new Exception("SupplierFilterType");
		}

		private void RequireSupplierIds()
		{
			if (SupplierIds == null)
				throw new NullReferenceException("SupplierIds");
		}

		private decimal? GetCalculatedRate(IEnumerable<decimal> rates)
		{
			if (rates.Count() == 0)
				return null;
			switch (RateEvaluationType)
			{
				case ZoneRateEvaluationType.AverageRate:
					return rates.Average();
				case ZoneRateEvaluationType.MaximumRate:
					return rates.Max();
				case ZoneRateEvaluationType.BasedOnTraffic:
					throw new NotImplementedException();
			}
			throw new Exception("ZoneRateEvaluationType");
		}
		
		#endregion
	}
}
