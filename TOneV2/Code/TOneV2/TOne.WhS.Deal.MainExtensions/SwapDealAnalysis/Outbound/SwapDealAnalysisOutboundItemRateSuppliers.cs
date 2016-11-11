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
	public class SwapDealAnalysisOutboundItemRateSuppliers : SwapDealAnalysisOutboundItemRateCalcMethod
	{
		public CarrierFilterType SupplierFilterType { get; set; }

		public IEnumerable<int> SupplierIds { get; set; }

		public ZoneRateEvaluationType RateEvaluationType { get; set; }

		public override decimal? Execute(ISwapDealAnalysisOutboundRateCalcMethodContext context)
		{
			var codeZoneMatchManager = new CodeZoneMatchManager();
			IEnumerable<int> otherSupplierIds = GetOtherSupplierIds();
			IEnumerable<CodeSupplierZoneMatch> codeSupplierZoneMatches =
				codeZoneMatchManager.GetOtherSupplierZonesMatchedToSupplierZones(context.SupplierId, context.SupplierZoneIds, otherSupplierIds);

			if (codeSupplierZoneMatches == null || codeSupplierZoneMatches.Count() == 0)
				return null;

			var rates = new List<decimal>();
			var supplierRateManager = new SupplierRateManager();

			foreach (CodeSupplierZoneMatch codeSupplierZoneMatch in codeSupplierZoneMatches)
			{
				var rate = supplierRateManager.GetCachedSupplierZoneRate(codeSupplierZoneMatch.SupplierId, codeSupplierZoneMatch.SupplierZoneId, DateTime.Now);
				if (rate != null && rate.Rate != null)
					rates.Add(rate.Rate.Rate);
			}

			return GetCalculatedRate(rates);
		}

		#region Private Methods

		private IEnumerable<int> GetOtherSupplierIds()
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
					throw new NotImplementedException("ZoneRateEvaluationType.BasedOnTraffic");
			}
			throw new Exception("RateEvaluationType");
		}
		
		#endregion
	}
}
