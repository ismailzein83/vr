using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Voice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation;

namespace Retail.Voice.Business
{
    public class VoiceChargingManager
    {
        #region Public Methods

        public List<VoiceEventPricedPart> PriceVoiceEvent(long accountId, Guid serviceTypeId, dynamic rawCDR, dynamic mappedCDR, decimal duration, DateTime eventTime)
        {
            List<VoiceEventPricedPart> eventPricingInfos = new List<VoiceEventPricedPart>();
            var voiceUsageChargers = GetVoiceUsageChargersByPriority(accountId, serviceTypeId, eventTime);
            Decimal remainingDurationToPrice = duration;
            foreach (var voiceUsageCharger in voiceUsageChargers)
            {
                var context = new VoiceUsageChargerContext
                {
                    ServiceTypeId = serviceTypeId,
                    RawCDR = rawCDR,
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
                        eventPricingInfos.Add(pricedPart);
                        remainingDurationToPrice -= pricedPart.PricedDuration;
                    });
                    if (remainingDurationToPrice <= 0)
                        break;
                }
            }
            if (remainingDurationToPrice > 0)
                throw new Exception(String.Format("Can't price entire duration. remaining duration '{0}'", remainingDurationToPrice));
            return eventPricingInfos;
        }

        public VoiceEventPricingInfo ApplyChargingPolicyToVoiceEvent(int chargingPolicyId, Guid serviceTypeId, dynamic rawCDR, dynamic mappedCDR, decimal duration, DateTime eventTime)
        {
            VoiceChargingPolicyEvaluator chargingPolicyEvaluator = GetVoiceChargingPolicyEvaluator(serviceTypeId);
            var context = new VoiceChargingPolicyEvaluatorContext
            {
                ServiceTypeId = serviceTypeId,
                ChargingPolicyId = chargingPolicyId,
                RawCDR = rawCDR,
                MappedCDR = mappedCDR,
                Duration = duration,
                EventTime = eventTime
            };
            chargingPolicyEvaluator.ApplyChargingPolicyToVoiceEvent(context);
            return context.EventPricingInfo;
        }

        #endregion

        #region Private Methods

        static ServiceTypeManager s_serviceTypeManager = new ServiceTypeManager();

        private struct GetVoiceUsageChargersByPriorityCacheName
        {
            public long AccountId { get; set; }

            public Guid ServiceTypeId { get; set; }

            public DateTime EventDate { get; set; }
        }

        List<VoiceUsageChargerWithParentPackage> GetVoiceUsageChargersByPriority(long accountId, Guid serviceTypeId, DateTime eventTime)
        {
            var cacheName = new GetVoiceUsageChargersByPriorityCacheName { AccountId = accountId, ServiceTypeId = serviceTypeId, EventDate = eventTime.Date };
            //needs caching
            List<Package> accountPackagesByPriority = null;//get account packages by priority
            List<VoiceUsageChargerWithParentPackage> voiceUsageChargersByPriority = new List<VoiceUsageChargerWithParentPackage>();
            foreach(var package in accountPackagesByPriority)
            {
                IPackageSettingVoiceUsageCharger packageSettingVoiceUsageCharger = package.Settings.ExtendedSettings as IPackageSettingVoiceUsageCharger;
                if(packageSettingVoiceUsageCharger != null)
                {
                    IPackageVoiceUsageCharger voiceUsageCharger;
                    if(packageSettingVoiceUsageCharger.TryGetVoiceUsageCharger(serviceTypeId, out voiceUsageCharger))
                    {
                        voiceUsageChargersByPriority.Add(new VoiceUsageChargerWithParentPackage
                            {
                                VoiceUsageCharger = voiceUsageCharger,
                                ParentPackage = package
                            });
                    }
                }
                else
                {
                    IPackageUsageChargingPolicy packageServiceUsageChargingPolicy = package.Settings.ExtendedSettings as IPackageUsageChargingPolicy;
                    if(packageServiceUsageChargingPolicy != null)
                    {
                        var context = new PackageServiceUsageChargingPolicyContext{ ServiceTypeId = serviceTypeId};
                        if (packageServiceUsageChargingPolicy.TryGetServiceUsageChargingPolicyId(context))
                        {
                            voiceUsageChargersByPriority.Add(new VoiceUsageChargerWithParentPackage
                            {
                                VoiceUsageCharger = new ChargingPolicyVoiceUsageCharger(context.ChargingPolicyId),
                                ParentPackage = package
                            });
                        }
                    }
                }
            }
            return voiceUsageChargersByPriority;
        }
        
        VoiceChargingPolicyEvaluator GetVoiceChargingPolicyEvaluator(Guid serviceTypeId)
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

        #endregion
    }

    public class VoiceUsageChargerContext : IVoiceUsageChargerContext
    {
        public Guid ServiceTypeId
        {
            get;
            set;
        }

        public dynamic RawCDR
        {
            get;
            set;
        }

        public dynamic MappedCDR
        {
            get;
            set;
        }

        public decimal Duration
        {
            get;
            set;
        }

        public DateTime EventTime { get; set; }
        
        public List<VoiceEventPricedPart> PricedPartInfos
        {
            get;
            set;
        }
    }

    public class VoiceChargingPolicyEvaluatorContext : IVoiceChargingPolicyEvaluatorContext
    {
        public Guid ServiceTypeId { get; set; }
        public int ChargingPolicyId
        {
            get;
            set;
        }

        public dynamic RawCDR
        {
            get;
            set;
        }

        public dynamic MappedCDR
        {
            get;
            set;
        }

        public decimal Duration
        {
            get;
            set;
        }

        public DateTime EventTime { get; set; }

        public Entities.VoiceEventPricingInfo EventPricingInfo
        {
            get;
            set;
        }
    }

    public class VoiceUsageChargerWithParentPackage
    {
        public IPackageVoiceUsageCharger VoiceUsageCharger { get; set; }

        public Retail.BusinessEntity.Entities.Package ParentPackage { get; set; }
    }
}
