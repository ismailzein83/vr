using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Entities.Settings;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Business
{
	public class SwapDealAnalysisManager
	{
		#region Public Methods

		public SwapDealAnalysisResult AnalyzeDeal(SwapDealAnalysisSettings settings)
		{
			if (settings == null)
				throw new NullReferenceException("settings");

			var result = new SwapDealAnalysisResult();
			int dealPeriodInDays = (settings.ToDate - settings.FromDate).Days;

			decimal? totalSaleMargin;
			decimal totalSaleRevenue;

			decimal? totalCostMargin;
			decimal totalCostRevenue;

			result.Inbounds = GetInboundResults(settings.Inbounds, settings.CarrierAccountId, dealPeriodInDays, out totalSaleMargin, out totalSaleRevenue);
			result.Outbounds = GetOutboundResults(settings.Outbounds, settings.CarrierAccountId, dealPeriodInDays, out totalCostMargin, out totalCostRevenue);

			result.DealPeriodInDays = dealPeriodInDays;

			result.TotalSaleMargin = totalSaleMargin;
			result.TotalSaleRevenue = totalSaleRevenue;

			result.TotalCostMargin = totalCostMargin;
			result.TotalCostRevenue = totalCostRevenue;

			if (totalSaleMargin.HasValue && totalSaleMargin.HasValue)
			{
				result.OverallProfit = totalSaleMargin.Value + totalCostMargin.Value;
				result.Margins = (result.OverallProfit.Value / totalSaleRevenue) * 100;
			}
			result.OverallRevenue = totalSaleRevenue - totalCostRevenue;

			return result;
		}

		public IEnumerable<SwapDealAnalysisInboundRateCalcMethodConfig> GetInboundRateCalcMethodExtensionConfigs()
		{
			var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
			return extensionConfigManager.GetExtensionConfigurations<SwapDealAnalysisInboundRateCalcMethodConfig>(SwapDealAnalysisInboundRateCalcMethodConfig.EXTENSION_TYPE).OrderBy(x => x.Title);
		}

		public IEnumerable<SwapDealAnalysisOutboundRateCalcMethodConfig> GetOutboundRateCalcMethodExtensionConfigs()
		{
			var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
			return extensionConfigManager.GetExtensionConfigurations<SwapDealAnalysisOutboundRateCalcMethodConfig>(SwapDealAnalysisOutboundRateCalcMethodConfig.EXTENSION_TYPE).OrderBy(x => x.Title);
		}

		public decimal? CalculateInboundRate(CalculateInboundRateInput input)
		{
			if (input.InboundItemRateCalcMethod == null)
				throw new NullReferenceException("input.InboundItemRateCalcMethod");

			var context = new SwapDealAnalysisInboundRateCalcMethodContext()
			{
				CustomerId = input.CustomerId,
				CountryId = input.CountryId,
				SaleZoneIds = input.SaleZoneIds
			};

			decimal? calculatedRate = input.InboundItemRateCalcMethod.Execute(context);
			return RoundNullableDecimal(calculatedRate);
		}

		public decimal? CalculateOutboundRate(CalculateOutboundRateInput input)
		{
			if (input.OutboundItemRateCalcMethod == null)
				throw new NullReferenceException("input.OutboundItemRateCalcMethod");

			var context = new SwapDealAnalysisOutboundRateCalcMethodContext()
			{
				SupplierId = input.SupplierId,
				SupplierZoneIds = input.SupplierZoneIds,
				CountryId = input.CountryId
			};

			decimal? calculatedRate = input.OutboundItemRateCalcMethod.Execute(context);
			return RoundNullableDecimal(calculatedRate);
		}
		
		#endregion

		#region Private Methods

		private List<SwapDealAnalysisResultInbound> GetInboundResults(IEnumerable<SwapDealAnalysisInboundSettings> inboundSettingsList, int carrierAccountId, int dealPeriodInDays, out decimal? totalSaleMargin, out decimal totalSaleRevenue)
		{
			totalSaleRevenue = 0;
			totalSaleMargin = 0;

			if (inboundSettingsList == null)
				return null;

			var inboundResults = new List<SwapDealAnalysisResultInbound>();

			foreach (SwapDealAnalysisInboundSettings inboundSettings in inboundSettingsList)
			{
				var inboundResult = new SwapDealAnalysisResultInbound()
				{
					Name = inboundSettings.Name
				};

				inboundResult.DailyVolume = inboundSettings.Volume / dealPeriodInDays;

				if (inboundSettings.ItemCalculationMethod == null)
					throw new NullReferenceException("inboundSettings.ItemCalculationMethod");
				var context = new SwapDealAnalysisInboundRateCalcMethodContext()
				{
					CustomerId = carrierAccountId,
					CountryId = inboundSettings.CountryId,
					SaleZoneIds = inboundSettings.SaleZoneIds
				};
				inboundResult.CurrentRate = inboundSettings.ItemCalculationMethod.Execute(context);

				if (inboundResult.CurrentRate.HasValue)
				{
					decimal rateProfitPerMinute = inboundSettings.DealRate - inboundResult.CurrentRate.Value;
					inboundResult.RateProfit = rateProfitPerMinute;
					inboundResult.Profit = rateProfitPerMinute * inboundSettings.Volume;
					totalSaleMargin += inboundResult.Profit;
				}
				else
					totalSaleMargin = null;

				inboundResult.Revenue = inboundSettings.DealRate * inboundSettings.Volume;
				totalSaleRevenue += inboundResult.Revenue;

				inboundResults.Add(inboundResult);
			}

			return inboundResults;
		}

		private List<SwapDealAnalysisResultOutbound> GetOutboundResults(IEnumerable<SwapDealAnalysisOutboundSettings> outboundSettingsList, int carrierAccountId, int dealPeriodInDays, out decimal? totalCostMargin, out decimal totalCostRevenue)
		{
			totalCostRevenue = 0;
			totalCostMargin = 0;

			if (outboundSettingsList == null)
				return null;

			var outboundResults = new List<SwapDealAnalysisResultOutbound>();

			foreach (SwapDealAnalysisOutboundSettings outboundSettings in outboundSettingsList)
			{
				var outboundResult = new SwapDealAnalysisResultOutbound()
				{
					Name = outboundSettings.Name
				};

				outboundResult.DailyVolume = outboundSettings.Volume / dealPeriodInDays;

				if (outboundSettings.ItemCalculationMethod == null)
					throw new NullReferenceException("inboundSettings.ItemCalculationMethod");
				var context = new SwapDealAnalysisOutboundRateCalcMethodContext()
				{
					SupplierId = carrierAccountId,
					CountryId = outboundSettings.CountryId,
					SupplierZoneIds = outboundSettings.SupplierZoneIds
				};
				outboundResult.CurrentRate = outboundSettings.ItemCalculationMethod.Execute(context);

				if (outboundResult.CurrentRate.HasValue)
				{
					decimal rateSavingsPerMinute = outboundResult.CurrentRate.Value - outboundSettings.DealRate;
					outboundResult.RateSavings = rateSavingsPerMinute;
					outboundResult.Savings = rateSavingsPerMinute * outboundSettings.Volume;
					totalCostMargin += outboundResult.Savings;
				}
				else
					totalCostMargin = null;

				outboundResult.Revenue = outboundSettings.DealRate * outboundSettings.Volume;
				totalCostRevenue += outboundResult.Revenue;

				outboundResults.Add(outboundResult);
			}

			return outboundResults;
		}

		private decimal? RoundNullableDecimal(decimal? value)
		{
			if (value.HasValue)
				return decimal.Round(value.Value, 4);
			return null;
		}

		#endregion
	}
}
