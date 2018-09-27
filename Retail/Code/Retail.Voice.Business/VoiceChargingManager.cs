using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Voice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Transformation;
using Vanrise.Common;

namespace Retail.Voice.Business
{
    public class VoiceChargingManager
    {
        #region Variables/Ctor

        AccountPackageManager _accountPackageManager = new AccountPackageManager();
        static PackageUsageVolumeCombinationManager s_packageUsageVolumeCombinationManager = new PackageUsageVolumeCombinationManager();
        static AccountPackageUsageVolumeManager s_accountPackageUsageVolumeManager = new AccountPackageUsageVolumeManager();

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

        public CDRPricingInfo PriceCDR(Guid accountBEDefinitionId, long accountId, Guid serviceTypeId, Guid recordTypeId, dynamic cdr, decimal durationInSec, DateTime cdrTime)
        {
            CDRPricingInfo pricingInfo = new CDRPricingInfo();
            Dictionary<int, List<Guid>> volumePackageItemIdsByPackageId = null;
            _accountPackageManager.LoadAccountPackagesByPriority(accountBEDefinitionId, accountId, cdrTime, true,
                (processedAccountPackage, handle) =>
                {
                    var package = processedAccountPackage.Package;

                    var packageVolumeCharging = package.Settings.ExtendedSettings as IPackageUsageVolume;
                    if (packageVolumeCharging != null)
                    {
                        var volumeChargingIsApplicableToEventContext = new PackageVolumeChargingIsApplicableToEventContext(accountBEDefinitionId, accountId, processedAccountPackage.AccountPackage,
                            processedAccountPackage.Package, serviceTypeId, recordTypeId, cdr, cdrTime);
                        if (packageVolumeCharging.IsApplicableToEvent(volumeChargingIsApplicableToEventContext))
                        {
                            if (volumePackageItemIdsByPackageId == null)
                                volumePackageItemIdsByPackageId = new Dictionary<int, List<Guid>>();
                            List<Guid> volumePackageItemIds = volumePackageItemIdsByPackageId.GetOrCreateItem(package.PackageId);
                            foreach(var itemId in volumeChargingIsApplicableToEventContext.ApplicableItemIds)
                            {
                                if (!volumePackageItemIds.Contains(itemId))
                                    volumePackageItemIds.Add(itemId);
                            }
                        }
                    }

                    IPackageUsageChargingPolicy packageServiceUsageChargingPolicy = package.Settings.ExtendedSettings as IPackageUsageChargingPolicy;
                    if (packageServiceUsageChargingPolicy != null)
                    {
                        var context = new PackageServiceUsageChargingPolicyContext { ServiceTypeId = serviceTypeId };
                        if (packageServiceUsageChargingPolicy.TryGetServiceUsageChargingPolicyId(context))
                        {
                            VoiceEventPricingInfo pricingInfoFromChargingPolicy = ApplyChargingPolicyToVoiceEvent(context.ChargingPolicyId, serviceTypeId, cdr, durationInSec, cdrTime, accountBEDefinitionId, processedAccountPackage.AccountPackage.AccountId);
                            if (pricingInfoFromChargingPolicy != null && pricingInfoFromChargingPolicy.SaleAmount.HasValue)
                            {
                                pricingInfo.PackageId = package.PackageId;
                                pricingInfo.UsageChargingPolicyId = pricingInfoFromChargingPolicy.ChargingPolicyId;
                                pricingInfo.SaleAmount = pricingInfoFromChargingPolicy.SaleAmount;
                                pricingInfo.SaleCurrencyId = pricingInfoFromChargingPolicy.SaleCurrencyId;
                                pricingInfo.SaleDurationInSeconds = pricingInfoFromChargingPolicy.SaleDurationInSeconds;
                                pricingInfo.SaleRate = pricingInfoFromChargingPolicy.SaleRate;
                                pricingInfo.SaleRateTypeId = pricingInfoFromChargingPolicy.SaleRateTypeId;
                                pricingInfo.SaleTariffRuleId = pricingInfoFromChargingPolicy.SaleTariffRuleId;
                                pricingInfo.SaleRateTypeRuleId = pricingInfoFromChargingPolicy.SaleRateTypeRuleId;
                                pricingInfo.SaleRateValueRuleId = pricingInfoFromChargingPolicy.SaleRateValueRuleId;
                                pricingInfo.SaleExtraChargeRuleId = pricingInfoFromChargingPolicy.SaleExtraChargeRuleId;
                                handle.Stop = true;
                            }
                        }
                    }
                });
            if (volumePackageItemIdsByPackageId != null)
                pricingInfo.PackageUsageVolumeCombinationId = s_packageUsageVolumeCombinationManager.GetCombinationId(volumePackageItemIdsByPackageId);
            return pricingInfo;
        }

        public List<CDRVolumePricingOutput> ApplyVolumePricingToCDRs(Guid accountBEDefinitionId, List<CDRVolumePricingInput> cdrs)
        {
            HashSet<PackageUsageVolumeBalanceKey> volumeBalanceKeys;
            List<VolumePricingCDRInProcess> cdrsInProcess = GenerateVolumePricingCDRsInProcess(accountBEDefinitionId, cdrs, out volumeBalanceKeys);

            Dictionary<PackageUsageVolumeBalanceKey, AccountPackageUsageVolumeBalanceInProcess> volumeBalancesByKey = s_accountPackageUsageVolumeManager.GetVolumeBalances(volumeBalanceKeys);

            List<CDRVolumePricingOutput> cdrPricingInfos = GenerateFinalCDRVolumePricingOutputs(cdrsInProcess, volumeBalancesByKey);

            s_accountPackageUsageVolumeManager.UpdateVolumeBalancesInDB(volumeBalancesByKey.Values);

            return cdrPricingInfos;
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

        static ServiceTypeManager s_serviceTypeManager = new ServiceTypeManager();

        private struct GetVoiceUsageChargersByPriorityCacheName
        {
            public Guid AccountDefinitionId { get; set; }

            public long AccountId { get; set; }

            public Guid ServiceTypeId { get; set; }

            public DateTime EventDate { get; set; }
        }

        private List<VoiceUsageChargerWithParentPackage> GetVoiceUsageChargersByPriority(Guid accountBEDefinitionId, long accountId, Guid serviceTypeId, DateTime eventTime)
        {
            var cacheName = new GetVoiceUsageChargersByPriorityCacheName { AccountDefinitionId = accountBEDefinitionId, AccountId = accountId, ServiceTypeId = serviceTypeId, EventDate = eventTime.Date };

            //needs caching
            List<ProcessedAccountPackage> processedAccountPackagesByPriority = GetProcessedAccountPackagesByPriority(accountBEDefinitionId, accountId, eventTime, true); //get account packages by priority

            if (processedAccountPackagesByPriority == null)
                return null;

            List<VoiceUsageChargerWithParentPackage> voiceUsageChargersByPriority = new List<VoiceUsageChargerWithParentPackage>();
            foreach (var processedAccountPackage in processedAccountPackagesByPriority)
            {
                IPackageSettingVoiceUsageCharger packageSettingVoiceUsageCharger = processedAccountPackage.Package.Settings.ExtendedSettings as IPackageSettingVoiceUsageCharger;
                if (packageSettingVoiceUsageCharger != null)
                {
                    IPackageVoiceUsageCharger voiceUsageCharger;
                    if (packageSettingVoiceUsageCharger.TryGetVoiceUsageCharger(serviceTypeId, out voiceUsageCharger))
                    {
                        voiceUsageChargersByPriority.Add(new VoiceUsageChargerWithParentPackage
                        {
                            VoiceUsageCharger = voiceUsageCharger,
                            ParentPackage = processedAccountPackage.Package,
                            ParentPackageAccountId = processedAccountPackage.AccountPackage.AccountId
                        });
                    }
                }
                else
                {
                    IPackageUsageChargingPolicy packageServiceUsageChargingPolicy = processedAccountPackage.Package.Settings.ExtendedSettings as IPackageUsageChargingPolicy;
                    if (packageServiceUsageChargingPolicy != null)
                    {
                        var context = new PackageServiceUsageChargingPolicyContext { ServiceTypeId = serviceTypeId };
                        if (packageServiceUsageChargingPolicy.TryGetServiceUsageChargingPolicyId(context))
                        {
                            voiceUsageChargersByPriority.Add(new VoiceUsageChargerWithParentPackage
                            {
                                VoiceUsageCharger = new ChargingPolicyVoiceUsageCharger(context.ChargingPolicyId),
                                ParentPackage = processedAccountPackage.Package,
                                ParentPackageAccountId = processedAccountPackage.AccountPackage.AccountId
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

        private List<ProcessedAccountPackage> GetProcessedAccountPackagesByPriority(Guid accountBEDefinitionId, long accountId, DateTime effectiveTime, bool withInheritence)
        {
            List<ProcessedAccountPackage> processedAccountPackages = new List<ProcessedAccountPackage>();

            new AccountPackageManager().LoadAccountPackagesByPriority(accountBEDefinitionId, accountId, effectiveTime, true, (processedAccountPackage, handle) =>
            {
                processedAccountPackages.Add(processedAccountPackage);
            });

            return processedAccountPackages;
        }

        private List<VolumePricingCDRInProcess> GenerateVolumePricingCDRsInProcess(Guid accountBEDefinitionId, List<CDRVolumePricingInput> cdrInputs, out HashSet<PackageUsageVolumeBalanceKey> volumeBalanceKeys)
        {
            var cdrsInProcess = new List<VolumePricingCDRInProcess>();
            var volumeBalanceKeys_local = new HashSet<PackageUsageVolumeBalanceKey>();//to use inside anonymous method(s)
            volumeBalanceKeys = volumeBalanceKeys_local;
            foreach (var cdrInput in cdrInputs)
            {
                var cdrInProcess = new VolumePricingCDRInProcess { CDRInput = cdrInput, VolumeItems = new List<PackageUsageVolumeItemInProcess>() };
                cdrsInProcess.Add(cdrInProcess);
                DateTime cdrTime = cdrInput.CDRTime;
                long accountId = cdrInput.AccountId;
                var packageUsageVolumeCombination = s_packageUsageVolumeCombinationManager.GetPackageUsageVolumeCombination(cdrInput.PackageUsageVolumeCombinationId);
                packageUsageVolumeCombination.ThrowIfNull("packageUsageVolumeCombination", cdrInput.PackageUsageVolumeCombinationId);
                packageUsageVolumeCombination.PackageItemsByPackageId.ThrowIfNull("packageUsageVolumeCombination.PackageItemsByPackageId", cdrInput.PackageUsageVolumeCombinationId);
                _accountPackageManager.LoadAccountPackagesByPriority(accountBEDefinitionId, accountId, cdrTime, true,
                    (processedAccountPackage, handle) =>
                    {
                        var package = processedAccountPackage.Package;
                        List<Guid> packageItemsIds;
                        if(packageUsageVolumeCombination.PackageItemsByPackageId.TryGetValue(package.PackageId, out packageItemsIds))
                        {
                            var packageVolumeCharging = package.Settings.ExtendedSettings as IPackageUsageVolume;
                            if (packageVolumeCharging != null)
                            {
                                var getPackageItemsInfoContext = new PackageUsageVolumeGetPackageItemsInfoContext(processedAccountPackage.AccountPackage, packageItemsIds, cdrTime);
                                packageVolumeCharging.GetPackageItemsInfo(getPackageItemsInfoContext);
                                if(getPackageItemsInfoContext.Items != null && getPackageItemsInfoContext.Items.Count > 0)
                                {
                                    foreach(var itm in getPackageItemsInfoContext.Items)
                                    {
                                        var balanceKey = new PackageUsageVolumeBalanceKey
                                        {
                                            AccountPackageId = processedAccountPackage.AccountPackage.AccountPackageId,
                                            PackageItemId = itm.ItemId,
                                            ItemFromTime = getPackageItemsInfoContext.FromTime
                                        };
                                        cdrInProcess.VolumeItems.Add(new PackageUsageVolumeItemInProcess
                                        {
                                            VolumeItem = itm,
                                            FromTime = getPackageItemsInfoContext.FromTime,
                                            ToTime = getPackageItemsInfoContext.ToTime,
                                            BalanceKey = balanceKey,
                                            ProcessedAccountPackage = processedAccountPackage
                                        });
                                        volumeBalanceKeys_local.Add(balanceKey);
                                    }
                                }
                            }
                        }
                    });
            }
            return cdrsInProcess;
        }

        private List<CDRVolumePricingOutput> GenerateFinalCDRVolumePricingOutputs(List<VolumePricingCDRInProcess> cdrsInProcess, Dictionary<PackageUsageVolumeBalanceKey, AccountPackageUsageVolumeBalanceInProcess> volumeBalancesByKey)
        {
            List<CDRVolumePricingOutput> cdrPricingInfos = new List<CDRVolumePricingOutput>();
            foreach (var cdrInProcess in cdrsInProcess)
            {
                CDRVolumePricingOutput pricingInfo = new CDRVolumePricingOutput { CDRInput = cdrInProcess.CDRInput, Items = new List<CDRVolumePricingOutputItem>() };
                Decimal pricingDurationInSec = cdrInProcess.CDRInput.PricingDurationInSec;
                Decimal remainingDurationInSec = pricingDurationInSec;

                foreach (var volumeItem in cdrInProcess.VolumeItems)
                {
                    AccountPackageUsageVolumeBalanceInProcess matchBalanceInProcess;
                    if (!volumeBalancesByKey.TryGetValue(volumeItem.BalanceKey, out matchBalanceInProcess))
                    {
                        matchBalanceInProcess = new AccountPackageUsageVolumeBalanceInProcess
                        {
                            Balance = new AccountPackageUsageVolumeBalance
                            {
                                AccountPackageId = volumeItem.ProcessedAccountPackage.AccountPackage.AccountPackageId,
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
                    if (matchBalance.UsedVolume < matchBalance.ItemVolume)
                    {
                        var pricedDurationInSec = Math.Min(volumeItem.VolumeItem.Volume, remainingDurationInSec);
                        var pricingInfoItem = new CDRVolumePricingOutputItem
                        {
                            AccountPackageId = volumeItem.ProcessedAccountPackage.AccountPackage.AccountPackageId,
                            PackageId = volumeItem.ProcessedAccountPackage.Package.PackageId,
                            PricedDurationInSec = Math.Min(pricedDurationInSec, remainingDurationInSec)
                        };
                        pricingInfo.Items.Add(pricingInfoItem);
                        if (!matchBalanceInProcess.ShouldAdd)
                            matchBalanceInProcess.ShouldUpdate = true;

                        remainingDurationInSec -= pricingInfoItem.PricedDurationInSec;
                        if (remainingDurationInSec == 0)
                            break;
                    }
                }
                pricingInfo.RemainingDurationInSec = remainingDurationInSec;
                cdrPricingInfos.Add(pricingInfo);
            }
            return cdrPricingInfos;
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

        private class PackageUsageVolumeItemInProcess
        {
            public PackageUsageVolumeItem VolumeItem { get; set; }

            public DateTime FromTime { get; set; }

            public DateTime ToTime { get; set; }

            public PackageUsageVolumeBalanceKey BalanceKey { get; set; }

            public ProcessedAccountPackage ProcessedAccountPackage { get; set; }
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
    }

    public class CDRVolumePricingOutput
    {
        public CDRVolumePricingInput CDRInput { get; set; }

        public List<CDRVolumePricingOutputItem> Items { get; set; }

        public decimal RemainingDurationInSec { get; set; }
    }

    public class CDRVolumePricingOutputItem
    {
        public decimal PricedDurationInSec { get; set; }

        public long AccountPackageId { get; set; }

        public int PackageId { get; set; }
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
        public long AccountId
        {
            get;
            set;
        }

        public Guid ServiceTypeId
        {
            get;
            set;
        }

        public object ChargeInfo
        {
            get;
            set;
        }
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
