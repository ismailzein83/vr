using System;
using System.Linq;
using Vanrise.Common;
using TOne.WhS.Deal.Entities;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;
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

            if (settings.Inbounds == null || settings.Outbounds == null || !settings.Inbounds.Any() || !settings.Outbounds.Any())
                return null;

            var result = new SwapDealAnalysisResult();
            int dealPeriodInDays = (settings.ToDate - settings.FromDate).Days + 1;

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
        public UpdateOperationOutput<GenericBusinessEntityDetail> UpdateDealAnalysis(int dealId, int genericBusinessEntityId, Guid businessEntityDefinitionId)
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            var genericBusinessEntityToUpdate = new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = businessEntityDefinitionId,
                GenericBusinessEntityId = genericBusinessEntityId
            };
            var fieldValues = new Dictionary<string, object>();
            fieldValues.Add("SwapDealId", dealId);
            genericBusinessEntityToUpdate.FieldValues = fieldValues;
            return genericBusinessEntityManager.UpdateGenericBusinessEntity(genericBusinessEntityToUpdate);
        }
        public Dictionary<int, SwapDealAnalysisDefinition> GetSwapDealAnalysisSettingById(Guid BeDefinitionId)
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetSwapDealAnalysisSettings", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                var swapDealAnalysisesById = new Dictionary<int, SwapDealAnalysisDefinition>();
                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        SwapDealAnalysisDefinition item = SwapDealAnalysisMapper(genericBusinessEntity.FieldValues);
                        if (item == null)
                            continue;

                        if (!swapDealAnalysisesById.ContainsKey(item.SwapDealAnalysisDefinitionId))
                            swapDealAnalysisesById.Add(item.SwapDealAnalysisDefinitionId, item);
                    }
                }
                return swapDealAnalysisesById;
            });
        }

        public SwapDealAnalysisDefinition GetSwapDealAnalysis(int genericBusinessEntityId, Guid businessEntityDefinitionId)
        {
            var swapDealAnalysisDefinitionById = GetSwapDealAnalysisSettingById(businessEntityDefinitionId);
            return swapDealAnalysisDefinitionById.GetRecord(genericBusinessEntityId);
        }

        #endregion

        #region Private Methods

        private SwapDealAnalysisDefinition SwapDealAnalysisMapper(Dictionary<string, object> fieldValues)
        {
            if (fieldValues == null)
                return null;

            SwapDealAnalysisDefinition analysisDefinition = new SwapDealAnalysisDefinition
            {
                Name = (string)fieldValues.GetRecord("Name"),
                SwapDealAnalysisDefinitionId = (int)fieldValues.GetRecord("Id"),
            };
            var analysisSettings = new SwapDealAnalysisSettings
            {
                CarrierAccountId = (int)fieldValues.GetRecord("CarrierId"),
                FromDate = (DateTime)fieldValues.GetRecord("DealBED"),
                ToDate = (DateTime)fieldValues.GetRecord("DealEED"),
                SwapDealId = (int?)fieldValues.GetRecord("SwapDealId")
            };
            var inbounds = fieldValues.GetRecord("Inbounds").CastWithValidate<SwapDealAnalysisInbound>("Inbounds", analysisDefinition.SwapDealAnalysisDefinitionId);
            if (inbounds != null && inbounds.SwapDealAnalysisInbounds != null && inbounds.SwapDealAnalysisInbounds.Any())
            {
                List<SwapDealAnalysisInboundSettings> swapDealAnalysisSettings = new List<SwapDealAnalysisInboundSettings>();
                foreach (var inbound in inbounds.SwapDealAnalysisInbounds)
                {
                    swapDealAnalysisSettings.Add(new SwapDealAnalysisInboundSettings
                    {
                        CountryId = inbound.CountryId,
                        DealRate = inbound.DealRate,
                        ItemCalculationMethod = inbound.ItemCalculationMethod,
                        GroupName = inbound.GroupName,
                        SaleZoneIds = inbound.SaleZoneIds,
                        Volume = inbound.Volume,
                    });
                }
                analysisSettings.Inbounds = swapDealAnalysisSettings;
            }

            var outbounds = fieldValues.GetRecord("Outbounds").CastWithValidate<SwapDealAnalysisOutbound>("outbounds", analysisDefinition.SwapDealAnalysisDefinitionId);
            if (outbounds != null && outbounds.SwapDealAnalysisOutbounds != null && outbounds.SwapDealAnalysisOutbounds.Any())
            {
                List<SwapDealAnalysisOutboundSettings> swapDealAnalysisSettings = new List<SwapDealAnalysisOutboundSettings>();
                foreach (var inbound in outbounds.SwapDealAnalysisOutbounds)
                {
                    swapDealAnalysisSettings.Add(new SwapDealAnalysisOutboundSettings
                    {
                        CountryId = inbound.CountryId,
                        DealRate = inbound.DealRate,
                        ItemCalculationMethod = inbound.ItemCalculationMethod,
                        GroupName = inbound.GroupName,
                        SupplierZoneIds = inbound.SupplierZoneIds,
                        Volume = inbound.Volume,
                    });
                }
                analysisSettings.Outbounds = swapDealAnalysisSettings;
            }
            analysisDefinition.Settings = analysisSettings;
            return analysisDefinition;
        }
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
                    GroupName = inboundSettings.GroupName,
                    Volume = inboundSettings.Volume,
                    DealRate = inboundSettings.DealRate,
                    ItemCalculationMethod = inboundSettings.ItemCalculationMethod,
                    SaleZoneIds = inboundSettings.SaleZoneIds,
                    CountryId = inboundSettings.CountryId,
                    CalculationMethodId = inboundSettings.CalculationMethodId
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
                    GroupName = outboundSettings.GroupName,
                    Volume = outboundSettings.Volume,
                    DealRate = outboundSettings.DealRate,
                    ItemCalculationMethod = outboundSettings.ItemCalculationMethod,
                    SupplierZoneIds = outboundSettings.SupplierZoneIds,
                    CountryId = outboundSettings.CountryId,
                    CalculationMethodId = outboundSettings.CalculationMethodId
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
