using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Voice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;

namespace Retail.Voice.Business
{
    public class VoiceChargingManager
    {
        #region Variables/Ctor

        static AccountPackageManager s_accountPackageManager = new AccountPackageManager();
        static ServiceTypeManager s_serviceTypeManager = new ServiceTypeManager();
        static PackageUsageVolumeCombinationManager s_packageUsageVolumeCombinationManager = new PackageUsageVolumeCombinationManager();
        static AccountPackageUsageVolumeBalanceManager s_accountPackageUsageVolumeBalanceManager = new AccountPackageUsageVolumeBalanceManager();

        #endregion

        #region Public Methods

        public VoiceEventPrice PriceVoiceEvent(Guid accountBEDefinitionId, long accountId, Guid serviceTypeId, dynamic mappedCDR, decimal duration, DateTime eventTime)
        {
            VoiceEventPrice voiceEventPrice = new VoiceEventPrice();
            voiceEventPrice.VoiceEventPricedParts = new List<VoiceEventPricedPart>();

            var voiceUsageChargers = GetVoiceUsageChargersByPriority(accountBEDefinitionId, accountId, serviceTypeId, eventTime);
            if (voiceUsageChargers == null || voiceUsageChargers.Count == 0)
                return null;

            Decimal remainingDurationToPrice = duration;
            Dictionary<IPackageVoiceUsageCharger, Object> chargersChargingInfos = new Dictionary<IPackageVoiceUsageCharger, object>();

            foreach (var voiceUsageCharger in voiceUsageChargers)
            {
                var context = new VoiceUsageChargerContext
                {
                    AccountBEDefinitionId = accountBEDefinitionId,
                    AccountId = accountId,
                    PackageAccountId = voiceUsageCharger.ParentPackageAccountId,
                    ServiceTypeId = serviceTypeId,
                    MappedCDR = mappedCDR,
                    Duration = remainingDurationToPrice,
                    EventTime = eventTime
                };

                voiceUsageCharger.VoiceUsageCharger.TryChargeVoiceEvent(context);
                if (context.PricedPartInfos != null)
                {
                    context.PricedPartInfos.ForEach(pricedPart =>
                    {
                        pricedPart.PackageId = voiceUsageCharger.ParentPackage.PackageId;
                        voiceEventPrice.VoiceEventPricedParts.Add(pricedPart);

                        remainingDurationToPrice -= pricedPart.PricedDuration;
                    });
                    chargersChargingInfos.Add(voiceUsageCharger.VoiceUsageCharger, context.ChargeInfo);

                    if (remainingDurationToPrice <= 0)
                        break;
                }
            }

            if (voiceEventPrice.VoiceEventPricedParts.Count > 1)
                throw new NotSupportedException("Case of multipler VoiceEventPricedParts not supported yet.");

            ConfigManager configManager = new ConfigManager();
            int? saleAmountPrecision = configManager.GetSaleAmountPrecision();

            if (voiceEventPrice.VoiceEventPricedParts.Count > 0)
            {
                if (remainingDurationToPrice > 0)
                    throw new Exception(String.Format("Can't price entire duration. remaining duration '{0}'", remainingDurationToPrice));

                var voiceEventPricedPart = voiceEventPrice.VoiceEventPricedParts[0];
                voiceEventPrice.PackageId = voiceEventPricedPart.PackageId;
                voiceEventPrice.UsageChargingPolicyId = voiceEventPricedPart.UsageChargingPolicyId;
                voiceEventPrice.SaleRate = voiceEventPricedPart.SaleRate;
                voiceEventPrice.SaleRateTypeId = voiceEventPricedPart.SaleRateTypeId;
                voiceEventPrice.SaleCurrencyId = voiceEventPricedPart.SaleCurrencyId;
                voiceEventPrice.SaleDurationInSeconds = voiceEventPricedPart.SaleDurationInSeconds;
                voiceEventPrice.SaleRateValueRuleId = voiceEventPricedPart.SaleRateValueRuleId;
                voiceEventPrice.SaleRateTypeRuleId = voiceEventPricedPart.SaleRateTypeRuleId;
                voiceEventPrice.SaleTariffRuleId = voiceEventPricedPart.SaleTariffRuleId;
                voiceEventPrice.SaleExtraChargeRuleId = voiceEventPricedPart.SaleExtraChargeRuleId;

                if (saleAmountPrecision.HasValue && voiceEventPricedPart.SaleAmount.HasValue)
                    voiceEventPrice.SaleAmount = Math.Round(voiceEventPricedPart.SaleAmount.Value, saleAmountPrecision.Value, MidpointRounding.AwayFromZero);
                else
                    voiceEventPrice.SaleAmount = voiceEventPricedPart.SaleAmount;

                foreach (var entry in chargersChargingInfos)
                {
                    entry.Key.DeductFromBalances(new VoiceUsageChargerDeductFromBalanceContext
                    {
                        AccountId = accountId,
                        ServiceTypeId = serviceTypeId,
                        ChargeInfo = entry.Value
                    });
                }
            }

            return voiceEventPrice;
        }

        /// <summary>
        /// Should Be Replacing PriceVoiceEvent
        /// </summary>
        public CDRPricingInfo PriceCDR(Guid accountBEDefinitionId, long accountId, Guid serviceTypeId, decimal durationInSec, DateTime eventTime, dynamic billingCDR, Dictionary<string, dynamic> recordsByName)
        {
            PriceCDRInput priceCDRInput = new PriceCDRInput()
            {
                AccountId = accountId,
                ServiceTypeId = serviceTypeId,
                DurationInSec = durationInSec,
                EventTime = eventTime,
                BillingCDR = billingCDR,
                RecordsByName = recordsByName
            };
            List<PriceCDROutput> priceCDROutputList = this.PriceCDRs(accountBEDefinitionId, new List<PriceCDRInput> { priceCDRInput });
            if (priceCDROutputList == null || priceCDROutputList.Count == 0)
                return null;

            PriceCDROutput priceCDROutput = priceCDROutputList.First();
            return priceCDROutput.PricingInfo;
        }

        public List<PriceCDROutput> PriceCDRs(Guid accountBEDefinitionId, List<PriceCDRInput> priceCDRInputs)
        {
            if (priceCDRInputs == null || priceCDRInputs.Count == 0)
                return null;

            AccountPackageProvider accountPackageProvider = this.GetAccountPackageProvider(accountBEDefinitionId);
            if (accountPackageProvider == null)
                return new List<PriceCDROutput>(priceCDRInputs.Select(itm => new PriceCDROutput() { PriceCDRInput = itm }));

            List<AccountEventTime> accountEventTimeList = priceCDRInputs.Select(itm => new AccountEventTime() { AccountId = itm.AccountId, EventTime = itm.EventTime }).ToList();

            var getRetailAccountPackagesContext = new AccountPackageProviderGetRetailAccountPackagesContext() { AccountBEDefinitionId = accountBEDefinitionId, AccountEventTimeList = accountEventTimeList };
            Dictionary<AccountEventTime, List<RetailAccountPackage>> retailAccountPackagesByAccountEventTime = accountPackageProvider.GetRetailAccountPackages(getRetailAccountPackagesContext);
            if (retailAccountPackagesByAccountEventTime == null || retailAccountPackagesByAccountEventTime.Count == 0)
                return new List<PriceCDROutput>(priceCDRInputs.Select(itm => new PriceCDROutput() { PriceCDRInput = itm }));

            PackageManager packageManager = new PackageManager();
            List<PriceCDROutput> priceCDROutputList = new List<PriceCDROutput>();

            var packageCombinationsDict = new Dictionary<string, BasePackageUsageVolumeCombination>();
            var cdrPricingInfoListByPackageCombinations = new Dictionary<string, List<CDRPricingInfo>>();

            foreach (var priceCDRInput in priceCDRInputs)
            {
                AccountEventTime accountEventTime = new AccountEventTime() { AccountId = priceCDRInput.AccountId, EventTime = priceCDRInput.EventTime };

                List<RetailAccountPackage> retailAccountPackages;
                if (!retailAccountPackagesByAccountEventTime.TryGetValue(accountEventTime, out retailAccountPackages))
                    continue;

                CDRPricingInfo cdrPricingInfo = new CDRPricingInfo();
                priceCDROutputList.Add(new PriceCDROutput() { PriceCDRInput = priceCDRInput, PricingInfo = cdrPricingInfo });

                Dictionary<int, List<Guid>> volumePackageItemsByPackageId = null;

                foreach (var retailAccountPackage in retailAccountPackages)
                {
                    var package = packageManager.GetPackage(retailAccountPackage.PackageId);

                    IPackageUsageVolume packageVolumeCharging = package.Settings.ExtendedSettings as IPackageUsageVolume;
                    if (packageVolumeCharging != null)
                    {
                        var volumeChargingIsApplicableToEventContext = new PackageVolumeChargingIsApplicableToEventContext(accountBEDefinitionId, priceCDRInput.AccountId, retailAccountPackage,
                            package, priceCDRInput.ServiceTypeId, priceCDRInput.EventTime, package.Settings.PackageDefinitionId, priceCDRInput.RecordsByName);

                        if (packageVolumeCharging.IsApplicableToEvent(volumeChargingIsApplicableToEventContext))
                        {
                            if (volumePackageItemsByPackageId == null)
                                volumePackageItemsByPackageId = new Dictionary<int, List<Guid>>();

                            List<Guid> volumePackageItemIds = volumePackageItemsByPackageId.GetOrCreateItem(package.PackageId);

                            foreach (var itemId in volumeChargingIsApplicableToEventContext.ApplicableItemIds)
                            {
                                if (!volumePackageItemIds.Contains(itemId))
                                    volumePackageItemIds.Add(itemId);
                            }
                        }
                    }

                    IPackageUsageChargingPolicy packageServiceUsageChargingPolicy = package.Settings.ExtendedSettings as IPackageUsageChargingPolicy;
                    if (packageServiceUsageChargingPolicy != null)
                    {
                        var context = new PackageServiceUsageChargingPolicyContext { ServiceTypeId = priceCDRInput.ServiceTypeId };
                        if (packageServiceUsageChargingPolicy.TryGetServiceUsageChargingPolicyId(context))
                        {
                            VoiceEventPricingInfo pricingInfoFromChargingPolicy = ApplyChargingPolicyToVoiceEvent(context.ChargingPolicyId, priceCDRInput.ServiceTypeId, priceCDRInput.BillingCDR,
                                priceCDRInput.DurationInSec, priceCDRInput.EventTime, accountBEDefinitionId, retailAccountPackage.AccountId);

                            if (pricingInfoFromChargingPolicy != null && pricingInfoFromChargingPolicy.SaleAmount.HasValue)
                            {
                                cdrPricingInfo.PackageId = package.PackageId;
                                cdrPricingInfo.UsageChargingPolicyId = pricingInfoFromChargingPolicy.ChargingPolicyId;
                                cdrPricingInfo.SaleAmount = pricingInfoFromChargingPolicy.SaleAmount;
                                cdrPricingInfo.SaleCurrencyId = pricingInfoFromChargingPolicy.SaleCurrencyId;
                                cdrPricingInfo.SaleDurationInSeconds = pricingInfoFromChargingPolicy.SaleDurationInSeconds;
                                cdrPricingInfo.SaleRate = pricingInfoFromChargingPolicy.SaleRate;
                                cdrPricingInfo.SaleRateTypeId = pricingInfoFromChargingPolicy.SaleRateTypeId;
                                cdrPricingInfo.SaleTariffRuleId = pricingInfoFromChargingPolicy.SaleTariffRuleId;
                                cdrPricingInfo.SaleRateTypeRuleId = pricingInfoFromChargingPolicy.SaleRateTypeRuleId;
                                cdrPricingInfo.SaleRateValueRuleId = pricingInfoFromChargingPolicy.SaleRateValueRuleId;
                                cdrPricingInfo.SaleExtraChargeRuleId = pricingInfoFromChargingPolicy.SaleExtraChargeRuleId;
                            }
                        }
                    }
                }

                if (volumePackageItemsByPackageId != null)
                {
                    string packageCombinations = Helper.SerializePackageCombinations(volumePackageItemsByPackageId);
                    if (!packageCombinationsDict.ContainsKey(packageCombinations))
                        packageCombinationsDict.Add(packageCombinations, new BasePackageUsageVolumeCombination() { PackageItemsByPackageId = volumePackageItemsByPackageId });

                    List<CDRPricingInfo> currentCDRPricingInfoList = cdrPricingInfoListByPackageCombinations.GetOrCreateItem(packageCombinations);
                    currentCDRPricingInfoList.Add(cdrPricingInfo);
                }
            }

            if (packageCombinationsDict.Count > 0)
            {
                Dictionary<string, int> combinationIdByPackageCombinations = s_packageUsageVolumeCombinationManager.InsertAndGetCombinationIds(packageCombinationsDict);

                foreach (var itm in cdrPricingInfoListByPackageCombinations)
                {
                    string packageCombinations = itm.Key;
                    List<CDRPricingInfo> cdrPricingInfos = itm.Value;

                    int packageUsageVolumeCombinationId;
                    if (!combinationIdByPackageCombinations.TryGetValue(packageCombinations, out packageUsageVolumeCombinationId))
                        throw new Exception($"combinationIdByPackageCombinations does not contain packageUsageVolumeCombination {packageCombinations}");

                    foreach (var currentCDRPricingInfo in cdrPricingInfos)
                        currentCDRPricingInfo.PackageUsageVolumeCombinationId = packageUsageVolumeCombinationId;
                }
            }

            return priceCDROutputList;
        }

        public List<CDRVolumePricingOutput> ApplyVolumePricingToCDRs(Guid accountBEDefinitionId, List<CDRVolumePricingInput> cdrs, Guid? cdrPricingDetailTypeId = null)
        {
            HashSet<PackageUsageVolumeBalanceKey> volumeBalanceKeys;
            List<VolumePricingCDRInProcess> cdrsInProcess = GenerateVolumePricingCDRsInProcess(accountBEDefinitionId, cdrs, out volumeBalanceKeys);

            Dictionary<PackageUsageVolumeBalanceKey, AccountPackageUsageVolumeBalanceInProcess> volumeBalancesByKey = s_accountPackageUsageVolumeBalanceManager.GetVolumeBalances(volumeBalanceKeys);

            List<CDRVolumePricingOutput> cdrVolumePricingOutputList = GenerateFinalCDRVolumePricingOutputs(cdrsInProcess, volumeBalancesByKey, cdrPricingDetailTypeId);

            s_accountPackageUsageVolumeBalanceManager.UpdateVolumeBalancesInDB(volumeBalancesByKey.Values);

            return cdrVolumePricingOutputList;
        }

        public List<dynamic> GetBillingStatsCDRRecords(CDRVolumePricingOutput cdrVolumePricingOutput, Guid billingCDRTypeId)
        {
            List<dynamic> billingStats = new List<dynamic>();

            dynamic billingCDR = cdrVolumePricingOutput.CDRInput.CDR;

            List<CDRVolumePricingOutputItem> cdrVolumePricingOutputItems = cdrVolumePricingOutput.CDRVolumePricingOutputItems;
            if (cdrVolumePricingOutputItems == null || cdrVolumePricingOutputItems.Count == 0)
                throw new NullReferenceException("cdrVolumePricingOutput.CDRVolumePricingOutputItems");

            foreach (var cdrVolumePricingOutputItem in cdrVolumePricingOutputItems)
            {
                dynamic clonedBillingCDR = billingCDR.CloneRecord(billingCDRTypeId);
                RemovePricingFieldValues(clonedBillingCDR);

                clonedBillingCDR.PackageId = cdrVolumePricingOutputItem.PackageId;
                clonedBillingCDR.PackageItemId = cdrVolumePricingOutputItem.PackageItemId; //should be added at billingCDR, billingStat and trafficStat
                clonedBillingCDR.SaleDurationInSec = cdrVolumePricingOutputItem.PricedDurationInSec;
                clonedBillingCDR.ChargedDurationInSec = 0;

                billingStats.Add(clonedBillingCDR);
            }

            CDRPricingOutputItem cdrPricingOutputItem = cdrVolumePricingOutput.CDRPricingOutputItem;
            if (cdrPricingOutputItem != null)
            {
                dynamic clonedBillingCDR = billingCDR.CloneRecord(billingCDRTypeId);

                clonedBillingCDR.PackageId = cdrPricingOutputItem.PackageId;
                clonedBillingCDR.SaleDurationInSec = cdrPricingOutputItem.PricedDurationInSec;
                clonedBillingCDR.ChargedDurationInSec = cdrPricingOutputItem.PricedDurationInSec;
                clonedBillingCDR.SaleAmount = cdrPricingOutputItem.SaleAmount;

                billingStats.Add(clonedBillingCDR);
            }

            return billingStats;
        }

        public void FillOrigValues(dynamic billingCDR)
        {
            billingCDR.OrigPackageId = billingCDR.PackageId;
            billingCDR.OrigChargingPolicyId = billingCDR.ChargingPolicyId;
            billingCDR.OrigSaleRate = billingCDR.SaleRate;
            billingCDR.OrigSaleAmount = billingCDR.SaleAmount;
            billingCDR.OrigSaleRateTypeId = billingCDR.SaleRateTypeId;
            billingCDR.OrigSaleCurrencyId = billingCDR.SaleCurrencyId;
            billingCDR.OrigSaleRateValueRuleId = billingCDR.SaleRateValueRuleId;
            billingCDR.OrigSaleRateTypeRuleId = billingCDR.SaleRateTypeRuleId;
            billingCDR.OrigSaleTariffRuleId = billingCDR.SaleTariffRuleId;
            billingCDR.OrigSaleExtraChargeRuleId = billingCDR.SaleExtraChargeRuleId;
        }

        public void RemovePricingFieldValues(dynamic billingCDR)
        {
            billingCDR.ChargingPolicyId = null;
            billingCDR.SaleRate = null;
            billingCDR.SaleAmount = null;
            billingCDR.SaleRateTypeId = null;
            billingCDR.SaleCurrencyId = null;
            billingCDR.ChargedDurationInSeconds = null;
            billingCDR.SaleRateValueRuleId = null;
            billingCDR.SaleRateTypeRuleId = null;
            billingCDR.SaleTariffRuleId = null;
            billingCDR.SaleExtraChargeRuleId = null;
        }

        public VoiceEventPricingInfo ApplyChargingPolicyToVoiceEvent(int chargingPolicyId, Guid serviceTypeId, dynamic mappedCDR, decimal duration, DateTime eventTime, Guid accountBEDefinitionId, long packageAccountId)
        {
            VoiceChargingPolicyEvaluator chargingPolicyEvaluator = GetVoiceChargingPolicyEvaluator(serviceTypeId);
            var context = new VoiceChargingPolicyEvaluatorContext
            {
                ServiceTypeId = serviceTypeId,
                ChargingPolicyId = chargingPolicyId,
                MappedCDR = mappedCDR,
                Duration = duration,
                EventTime = eventTime,
                AccountBEDefinitionId = accountBEDefinitionId,
                PackageAccountId = packageAccountId
            };
            chargingPolicyEvaluator.ApplyChargingPolicyToVoiceEvent(context);
            return context.EventPricingInfo;
        }

        #endregion

        #region Private Methods

        private List<VoiceUsageChargerWithParentPackage> GetVoiceUsageChargersByPriority(Guid accountBEDefinitionId, long accountId, Guid serviceTypeId, DateTime eventTime)
        {
            var cacheName = new GetVoiceUsageChargersByPriorityCacheName { AccountDefinitionId = accountBEDefinitionId, AccountId = accountId, ServiceTypeId = serviceTypeId, EventDate = eventTime.Date };

            //needs caching
            List<ProcessedRetailAccountPackage> processedRetailAccountPackagesByPriority = GetProcessedAccountPackagesByPriority(accountBEDefinitionId, accountId, eventTime, true); //get account packages by priority
            if (processedRetailAccountPackagesByPriority == null)
                return null;

            List<VoiceUsageChargerWithParentPackage> voiceUsageChargersByPriority = new List<VoiceUsageChargerWithParentPackage>();
            foreach (var processedRetailAccountPackage in processedRetailAccountPackagesByPriority)
            {
                IPackageSettingVoiceUsageCharger packageSettingVoiceUsageCharger = processedRetailAccountPackage.Package.Settings.ExtendedSettings as IPackageSettingVoiceUsageCharger;
                if (packageSettingVoiceUsageCharger != null)
                {
                    IPackageVoiceUsageCharger voiceUsageCharger;
                    if (packageSettingVoiceUsageCharger.TryGetVoiceUsageCharger(serviceTypeId, out voiceUsageCharger))
                    {
                        voiceUsageChargersByPriority.Add(new VoiceUsageChargerWithParentPackage
                        {
                            VoiceUsageCharger = voiceUsageCharger,
                            ParentPackage = processedRetailAccountPackage.Package,
                            ParentPackageAccountId = processedRetailAccountPackage.RetailAccountPackage.AccountId
                        });
                    }
                }
                else
                {
                    IPackageUsageChargingPolicy packageServiceUsageChargingPolicy = processedRetailAccountPackage.Package.Settings.ExtendedSettings as IPackageUsageChargingPolicy;
                    if (packageServiceUsageChargingPolicy != null)
                    {
                        var context = new PackageServiceUsageChargingPolicyContext { ServiceTypeId = serviceTypeId };
                        if (packageServiceUsageChargingPolicy.TryGetServiceUsageChargingPolicyId(context))
                        {
                            voiceUsageChargersByPriority.Add(new VoiceUsageChargerWithParentPackage
                            {
                                VoiceUsageCharger = new ChargingPolicyVoiceUsageCharger(context.ChargingPolicyId),
                                ParentPackage = processedRetailAccountPackage.Package,
                                ParentPackageAccountId = processedRetailAccountPackage.RetailAccountPackage.AccountId
                            });
                        }
                    }
                }
            }

            return voiceUsageChargersByPriority;
        }

        private VoiceChargingPolicyEvaluator GetVoiceChargingPolicyEvaluator(Guid serviceTypeId)
        {
            //Needs caching
            var serviceType = s_serviceTypeManager.GetServiceType(serviceTypeId);
            if (serviceType == null)
                throw new NullReferenceException(String.Format("serviceType '{0}'", serviceTypeId));
            if (serviceType.Settings == null)
                throw new NullReferenceException(String.Format("serviceType.Settings '{0}'", serviceTypeId));
            if (serviceType.Settings.ExtendedSettings == null)
                throw new NullReferenceException(String.Format("serviceType.Settings.ExtendedSettings '{0}'", serviceTypeId));
            IServiceVoiceChargingPolicyEvaluator serviceVoiceChargingPolicyEvaluator = serviceType.Settings.ExtendedSettings as IServiceVoiceChargingPolicyEvaluator;
            if (serviceVoiceChargingPolicyEvaluator == null)
                throw new Exception(String.Format("serviceType.Settings.ExtendedSettings is not of type IServiceVoiceChargingPolicyEvaluator. it is of type '{0}'. Service Type Id '{1}'", serviceType.Settings.ExtendedSettings.GetType(), serviceTypeId));
            VoiceChargingPolicyEvaluator chargingPolicyEvaluator = serviceVoiceChargingPolicyEvaluator.GetChargingPolicyEvaluator();
            if (chargingPolicyEvaluator == null)
                throw new NullReferenceException(String.Format("VoiceChargingPolicyEvaluator '{0}'", serviceTypeId));
            return chargingPolicyEvaluator;
        }

        private List<ProcessedRetailAccountPackage> GetProcessedAccountPackagesByPriority(Guid accountBEDefinitionId, long accountId, DateTime effectiveTime, bool withInheritence)
        {
            AccountPackageProvider accountPackageProvider = this.GetAccountPackageProvider(accountBEDefinitionId);
            if (accountPackageProvider == null)
                return null;

            List<AccountEventTime> accountEventTimeList = new List<AccountEventTime>() { new AccountEventTime() { AccountId = accountId, EventTime = effectiveTime } };

            var getRetailAccountPackagesContext = new AccountPackageProviderGetRetailAccountPackagesContext() { AccountBEDefinitionId = accountBEDefinitionId, AccountEventTimeList = accountEventTimeList };
            Dictionary<AccountEventTime, List<RetailAccountPackage>> retailAccountPackagesByAccountEventTime = accountPackageProvider.GetRetailAccountPackages(getRetailAccountPackagesContext);
            if (retailAccountPackagesByAccountEventTime == null || retailAccountPackagesByAccountEventTime.Count == 0)
                return null;

            PackageManager packageManager = new PackageManager();
            List<ProcessedRetailAccountPackage> processedRetailAccountPackages = new List<ProcessedRetailAccountPackage>();

            foreach (var retailAccountPackages in retailAccountPackagesByAccountEventTime.Values)
            {
                foreach (var retailAccountPackage in retailAccountPackages)
                {
                    var processedRetailAccountPackage = new ProcessedRetailAccountPackage()
                    {
                        RetailAccountPackage = retailAccountPackage,
                        Package = packageManager.GetPackage(retailAccountPackage.PackageId)
                    };
                    processedRetailAccountPackages.Add(processedRetailAccountPackage);
                }
            }

            return processedRetailAccountPackages;
        }

        private List<VolumePricingCDRInProcess> GenerateVolumePricingCDRsInProcess(Guid accountBEDefinitionId, List<CDRVolumePricingInput> cdrInputs, out HashSet<PackageUsageVolumeBalanceKey> volumeBalanceKeys)
        {
            PackageManager packageManager = new PackageManager();
            var cdrsInProcess = new List<VolumePricingCDRInProcess>();
            var _volumeBalanceKeys = new HashSet<PackageUsageVolumeBalanceKey>(); //to use inside anonymous method(s)
            volumeBalanceKeys = _volumeBalanceKeys;

            AccountPackageProvider accountPackageProvider = this.GetAccountPackageProvider(accountBEDefinitionId);
            if (accountPackageProvider == null)
                return new List<VolumePricingCDRInProcess>(cdrInputs.Select(cdrInput => new VolumePricingCDRInProcess { CDRInput = cdrInput }));

            foreach (var cdrInput in cdrInputs)
            {
                var cdrInProcess = new VolumePricingCDRInProcess { CDRInput = cdrInput, VolumeItems = new List<PackageUsageVolumeItemInProcess>() };
                cdrsInProcess.Add(cdrInProcess);

                DateTime cdrTime = cdrInput.CDRTime;
                long accountId = cdrInput.AccountId;

                var packageUsageVolumeCombination = s_packageUsageVolumeCombinationManager.GetPackageUsageVolumeCombination(cdrInput.PackageUsageVolumeCombinationId);
                packageUsageVolumeCombination.ThrowIfNull("packageUsageVolumeCombination", cdrInput.PackageUsageVolumeCombinationId);
                packageUsageVolumeCombination.PackageItemsByPackageId.ThrowIfNull("packageUsageVolumeCombination.PackageItemsByPackageId", cdrInput.PackageUsageVolumeCombinationId);

                List<AccountEventTime> accountEventTimeList = new List<AccountEventTime>() { new AccountEventTime() { AccountId = accountId, EventTime = cdrTime } };

                var getRetailAccountPackagesContext = new AccountPackageProviderGetRetailAccountPackagesContext() { AccountBEDefinitionId = accountBEDefinitionId, AccountEventTimeList = accountEventTimeList };
                Dictionary<AccountEventTime, List<RetailAccountPackage>> retailAccountPackagesByAccountEventTime = accountPackageProvider.GetRetailAccountPackages(getRetailAccountPackagesContext);
                if (retailAccountPackagesByAccountEventTime == null || retailAccountPackagesByAccountEventTime.Count == 0)
                    return null;

                foreach (var retailAccountPackages in retailAccountPackagesByAccountEventTime.Values)
                {
                    foreach (var retailAccountPackage in retailAccountPackages)
                    {
                        List<Guid> packageItemsIds;
                        if (!packageUsageVolumeCombination.PackageItemsByPackageId.TryGetValue(retailAccountPackage.PackageId, out packageItemsIds))
                            continue;

                        var package = packageManager.GetPackage(retailAccountPackage.PackageId);
                        package.ThrowIfNull("package", retailAccountPackage.PackageId);
                        package.Settings.ThrowIfNull("package.Settings", retailAccountPackage.PackageId);
                        package.Settings.ExtendedSettings.ThrowIfNull("package.Settings.ExtendedSettings", retailAccountPackage.PackageId);

                        var packageVolumeCharging = package.Settings.ExtendedSettings as IPackageUsageVolume;
                        if (packageVolumeCharging == null)
                            continue;

                        var getPackageItemsInfoContext = new PackageUsageVolumeGetPackageItemsInfoContext(retailAccountPackage, packageItemsIds, cdrTime, package.Settings.PackageDefinitionId);
                        packageVolumeCharging.GetPackageItemsInfo(getPackageItemsInfoContext);

                        if (getPackageItemsInfoContext.Items == null || getPackageItemsInfoContext.Items.Count == 0)
                            continue;

                        foreach (var itm in getPackageItemsInfoContext.Items)
                        {
                            var balanceKey = new PackageUsageVolumeBalanceKey
                            {
                                AccountPackageId = retailAccountPackage.AccountPackageId,
                                PackageItemId = itm.ItemId,
                                ItemFromTime = getPackageItemsInfoContext.FromTime
                            };
                            _volumeBalanceKeys.Add(balanceKey);

                            cdrInProcess.VolumeItems.Add(new PackageUsageVolumeItemInProcess
                            {
                                VolumeItem = itm,
                                FromTime = getPackageItemsInfoContext.FromTime,
                                ToTime = getPackageItemsInfoContext.ToTime,
                                BalanceKey = balanceKey,
                                ProcessedRetailAccountPackage = new ProcessedRetailAccountPackage() { RetailAccountPackage = retailAccountPackage, Package = package }
                            });
                        }
                    }
                }
            }

            return cdrsInProcess;
        }

        private List<CDRVolumePricingOutput> GenerateFinalCDRVolumePricingOutputs(List<VolumePricingCDRInProcess> cdrsInProcess,
            Dictionary<PackageUsageVolumeBalanceKey, AccountPackageUsageVolumeBalanceInProcess> volumeBalancesByKey, Guid? cdrPricingDetailTypeId)
        {
            List<CDRVolumePricingOutput> cdrVolumePricingOutputList = new List<CDRVolumePricingOutput>();

            Type cdrPricingDetailType = null;
            if (cdrPricingDetailTypeId.HasValue)
            {
                DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
                cdrPricingDetailType = dataRecordTypeManager.GetDataRecordRuntimeType(cdrPricingDetailTypeId.Value);
            }

            foreach (var cdrInProcess in cdrsInProcess)
            {
                CDRVolumePricingInput cdrInput = cdrInProcess.CDRInput;
                CDRVolumePricingOutput cdrOutput = new CDRVolumePricingOutput { CDRInput = cdrInput, CDRVolumePricingOutputItems = new List<CDRVolumePricingOutputItem>() };

                Decimal remainingDurationInSec = cdrInput.PricingDurationInSec;

                foreach (var volumeItem in cdrInProcess.VolumeItems)
                {
                    AccountPackageUsageVolumeBalanceInProcess matchBalanceInProcess;
                    if (!volumeBalancesByKey.TryGetValue(volumeItem.BalanceKey, out matchBalanceInProcess))
                    {
                        matchBalanceInProcess = new AccountPackageUsageVolumeBalanceInProcess
                        {
                            Balance = new AccountPackageUsageVolumeBalance
                            {
                                AccountPackageId = volumeItem.ProcessedRetailAccountPackage.RetailAccountPackage.AccountPackageId,
                                PackageItemId = volumeItem.VolumeItem.ItemId,
                                FromTime = volumeItem.FromTime,
                                ToTime = volumeItem.ToTime,
                                ItemVolume = volumeItem.VolumeItem.Volume * 60
                            },
                            ShouldAdd = true
                        };
                        volumeBalancesByKey.Add(volumeItem.BalanceKey, matchBalanceInProcess);
                    }

                    var matchBalance = matchBalanceInProcess.Balance;
                    decimal matchBalanceRemaniningVolume = matchBalance.ItemVolume - matchBalance.UsedVolume;
                    if (matchBalanceRemaniningVolume > 0)
                    {
                        var pricedDurationInSec = Math.Min(matchBalanceRemaniningVolume, remainingDurationInSec);
                        matchBalance.UsedVolume += pricedDurationInSec;

                        var cdrVolumePricingOutputItem = new CDRVolumePricingOutputItem
                        {
                            AccountPackageId = volumeItem.ProcessedRetailAccountPackage.RetailAccountPackage.AccountPackageId,
                            PackageId = volumeItem.ProcessedRetailAccountPackage.Package.PackageId,
                            PackageItemId = volumeItem.VolumeItem.ItemId,
                            PricedDurationInSec = pricedDurationInSec
                        };
                        cdrOutput.CDRVolumePricingOutputItems.Add(cdrVolumePricingOutputItem);

                        if (!matchBalanceInProcess.ShouldAdd)
                            matchBalanceInProcess.ShouldUpdate = true;

                        remainingDurationInSec -= pricedDurationInSec;
                        if (remainingDurationInSec == 0)
                            break;
                    }
                }

                if (remainingDurationInSec > 0)
                {
                    cdrOutput.CDRPricingOutputItem = new CDRPricingOutputItem()
                    {
                        PackageId = cdrInput.PackageId,
                        PricedDurationInSec = remainingDurationInSec,
                        SaleAmount = (remainingDurationInSec / cdrInput.PricingDurationInSec) * cdrInput.SaleAmount
                    };
                }

                if (cdrPricingDetailTypeId.HasValue)
                {
                    cdrOutput.CDRPricingDetails = new List<dynamic>();

                    if (cdrOutput.CDRVolumePricingOutputItems.Count > 0)
                    {
                        foreach (var cdrVolumePricingOutputItem in cdrOutput.CDRVolumePricingOutputItems)
                        {
                            dynamic cdrPricingDetail = Activator.CreateInstance(cdrPricingDetailType);
                            cdrPricingDetail.PackageId = cdrVolumePricingOutputItem.PackageId;
                            cdrPricingDetail.PackageItemId = cdrVolumePricingOutputItem.PackageItemId;
                            cdrPricingDetail.PricedDurationInSec = cdrVolumePricingOutputItem.PricedDurationInSec;

                            cdrOutput.CDRPricingDetails.Add(cdrPricingDetail);
                        }
                    }

                    if (cdrOutput.CDRPricingOutputItem != null)
                    {
                        dynamic cdrPricingDetail = Activator.CreateInstance(cdrPricingDetailType);
                        cdrPricingDetail.PackageId = cdrOutput.CDRPricingOutputItem.PackageId;
                        cdrPricingDetail.PricedDurationInSec = cdrOutput.CDRPricingOutputItem.PricedDurationInSec;
                        cdrPricingDetail.SaleAmount = cdrOutput.CDRPricingOutputItem.SaleAmount;

                        cdrOutput.CDRPricingDetails.Add(cdrPricingDetail);
                    }
                }

                cdrVolumePricingOutputList.Add(cdrOutput);
            }

            return cdrVolumePricingOutputList;
        }

        private AccountPackageProvider GetAccountPackageProvider(Guid accountBEDefinitionId)
        {
            var beDefinition = new BusinessEntityDefinitionManager().GetBusinessEntityDefinition(accountBEDefinitionId);
            beDefinition.ThrowIfNull("beDefinition", accountBEDefinitionId);
            beDefinition.Settings.ThrowIfNull("beDefinition.Settings", accountBEDefinitionId);

            var additionalSettings = beDefinition.Settings.GetAdditionalSettings(new BEDefinitionSettingsGetAdditionalSettingsContext());
            if (additionalSettings == null)
                return null;

            var accountPackageProvider = additionalSettings.GetRecord("AccountPackageProvider");
            if (accountPackageProvider == null)
                return null;

            return accountPackageProvider as AccountPackageProvider;
        }

        //private List<Package> GetAccountPackagesByPriority(long accountId, DateTime effectiveTime)
        //{
        //    IEnumerable<int> accountPackagesIds = _accountPackageManager.GetPackageIdsAssignedToAccount(accountId, effectiveTime);

        //    if (accountPackagesIds == null || accountPackagesIds.Count() == 0)
        //        return null;

        //    List<Package> accountPackages = new PackageManager().GetPackagesByIds(accountPackagesIds);

        //    return accountPackages;
        //}

        //private List<Package> GetAllAccountPackagesByPriority(long accountId)
        //{
        //    List<int> accountPackagesIds = null;
        //    GetAccountPackagesByPriority(accountId, accountPackagesIds);
        //    List<Package> accountPackages = new PackageManager().GetPackagesByIds(accountPackagesIds);

        //    return accountPackages;
        //}

        //private void GetAccountPackagesByPriority(long? accountId, List<int> accountPackagesIds)
        //{
        //    if (!accountId.HasValue)
        //        return;

        //    if (accountPackagesIds == null)
        //        accountPackagesIds = new List<int>();

        //    IEnumerable<int> tempAccountPackagesIds = _accountPackageManager.GetPackageIdsAssignedToAccount(accountId.Value);
        //    if (tempAccountPackagesIds != null)
        //        accountPackagesIds.AddRange(tempAccountPackagesIds);

        //    Account account = _accountManager.GetAccount(accountId.Value);
        //    if (account == null)
        //        return;

        //    GetAccountPackagesByPriority(account.ParentAccountId, accountPackagesIds);
        //}

        #endregion

        #region Private Classes

        private struct GetVoiceUsageChargersByPriorityCacheName
        {
            public Guid AccountDefinitionId { get; set; }

            public long AccountId { get; set; }

            public Guid ServiceTypeId { get; set; }

            public DateTime EventDate { get; set; }
        }

        private class PackageUsageVolumeItemInProcess
        {
            public PackageUsageVolumeItem VolumeItem { get; set; }

            public DateTime FromTime { get; set; }

            public DateTime ToTime { get; set; }

            public PackageUsageVolumeBalanceKey BalanceKey { get; set; }

            public ProcessedRetailAccountPackage ProcessedRetailAccountPackage { get; set; }
        }

        private class VolumePricingCDRInProcess
        {
            public CDRVolumePricingInput CDRInput { get; set; }

            public List<PackageUsageVolumeItemInProcess> VolumeItems { get; set; }
        }

        #endregion
    }

    public class CDRPricingInfo
    {
        public int? PackageId { get; set; }

        public int? UsageChargingPolicyId { get; set; }

        public Decimal? SaleRate { get; set; }

        public Decimal? SaleAmount { get; set; }

        public int? SaleRateTypeId { get; set; }

        public int? SaleCurrencyId { get; set; }

        public Decimal? SaleDurationInSeconds { get; set; }

        public int? SaleRateValueRuleId { get; set; }

        public int? SaleRateTypeRuleId { get; set; }

        public int? SaleTariffRuleId { get; set; }

        public int? SaleExtraChargeRuleId { get; set; }

        public int? PackageUsageVolumeCombinationId { get; set; }
    }

    public class PriceCDRInput
    {
        public long AccountId { get; set; }

        public Guid ServiceTypeId { get; set; }

        public decimal DurationInSec { get; set; }

        public DateTime EventTime { get; set; }

        public dynamic BillingCDR { get; set; }

        public Dictionary<string, dynamic> RecordsByName { get; set; }
    }

    public class PriceCDROutput
    {
        public PriceCDRInput PriceCDRInput { get; set; }

        public CDRPricingInfo PricingInfo { get; set; }
    }

    //ToBeDeleted
    public class ProcessedRetailAccountPackage
    {
        public RetailAccountPackage RetailAccountPackage { get; set; }

        public Package Package { get; set; }
    }

    public class CDRVolumePricingInput
    {
        public CDRVolumePricingInput(dynamic cdr, long accountId, DateTime cdrTime, Decimal pricingDurationInSec, int packageUsageVolumeCombinationId)
        {
            this.CDR = cdr;
            this.AccountId = accountId;
            this.CDRTime = cdrTime;
            this.PricingDurationInSec = pricingDurationInSec;
            this.PackageUsageVolumeCombinationId = packageUsageVolumeCombinationId;
        }

        public dynamic CDR { get; set; }

        public long AccountId { get; private set; }

        public DateTime CDRTime { get; private set; }

        public Decimal PricingDurationInSec { get; private set; }

        public int PackageUsageVolumeCombinationId { get; private set; }

        public Decimal SaleAmount { get; private set; }

        public int PackageId { get; private set; }
    }

    public class CDRVolumePricingOutput
    {
        public CDRVolumePricingInput CDRInput { get; set; }

        public List<CDRVolumePricingOutputItem> CDRVolumePricingOutputItems { get; set; }

        public CDRPricingOutputItem CDRPricingOutputItem { get; set; }

        public List<dynamic> CDRPricingDetails { get; set; }
    }

    public class CDRVolumePricingOutputItem
    {
        public long AccountPackageId { get; set; }

        public int PackageId { get; set; }

        public Guid PackageItemId { get; set; }

        public decimal PricedDurationInSec { get; set; }
    }

    public class CDRPricingOutputItem
    {
        public int PackageId { get; set; }

        public decimal PricedDurationInSec { get; set; }

        public decimal SaleAmount { get; set; }
    }

    public class VoiceUsageChargerContext : IVoiceUsageChargerContext
    {
        public Guid AccountBEDefinitionId { get; set; }

        public long AccountId { get; set; }

        public long PackageAccountId { get; set; }

        public Guid ServiceTypeId { get; set; }

        public dynamic MappedCDR { get; set; }

        public decimal Duration { get; set; }

        public DateTime EventTime { get; set; }

        public List<VoiceEventPricedPart> PricedPartInfos { get; set; }

        public object ChargeInfo { get; set; }
    }

    public class VoiceUsageChargerDeductFromBalanceContext : IVoiceUsageChargerDeductFromBalanceContext
    {
        public long AccountId { get; set; }

        public Guid ServiceTypeId { get; set; }

        public object ChargeInfo { get; set; }
    }

    public class VoiceChargingPolicyEvaluatorContext : IVoiceChargingPolicyEvaluatorContext
    {
        public Guid ServiceTypeId { get; set; }

        public int ChargingPolicyId { get; set; }

        public dynamic MappedCDR { get; set; }

        public decimal Duration { get; set; }

        public DateTime EventTime { get; set; }

        public Guid AccountBEDefinitionId { get; set; }

        public long PackageAccountId { get; set; }

        public Entities.VoiceEventPricingInfo EventPricingInfo { get; set; }
    }

    public class VoiceUsageChargerWithParentPackage
    {
        public IPackageVoiceUsageCharger VoiceUsageCharger { get; set; }

        public Retail.BusinessEntity.Entities.Package ParentPackage { get; set; }

        public long ParentPackageAccountId { get; set; }
    }
}