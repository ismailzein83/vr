using System;
using System.Collections.Generic;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class SwitchManager
    {
        static Guid beDefinitionId = new Guid("596C4348-ABF1-4A6A-B388-8DC371355D0D");

        #region Public Methods

        /// <summary>
        /// Used In DataTransformation
        /// </summary>
        /// <param name="switchId"></param>
        /// <param name="inputReceiver"></param>
        /// <param name="receiverIn"></param>
        /// <param name="receiverOut"></param>
        /// <returns></returns>
        /// 
        
        public SMSReceiversForIdentification GetSMSReceiversForIdentification(string inputReceiver, string receiverIn, string receiverOut)
        {
            Dictionary<SMSReceiver, ReceiverIdentification> mappingResults = GetCachedMappingSMSReceivers();

            string customerReceiver = GetReceiverValueForIdentification(mappingResults.GetRecord(SMSReceiver.CustomerReceiver), inputReceiver, receiverIn, receiverOut);
            string supplierReceiver = GetReceiverValueForIdentification(mappingResults.GetRecord(SMSReceiver.SupplierReceiver), inputReceiver, receiverIn, receiverOut);
            string outputReceiver = GetReceiverValueForIdentification(mappingResults.GetRecord(SMSReceiver.Receiver), inputReceiver, receiverIn, receiverOut);

            return new SMSReceiversForIdentification
            {
                CustomerReceiver = customerReceiver,
                SupplierReceiver = supplierReceiver,
                OutputReceiver = outputReceiver
            };

        }

        /// <summary>
        /// Used In DataTransformation
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="receiverIn"></param>
        /// <param name="receiverOut"></param>
        /// <param name="normalizationRuleDefinitionId"></param>
        /// <param name="effectiveTime"></param>
        /// <param name="switchId"></param>
        /// <param name="customerId"></param>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        public SMSReceiversForMobileNetworkMatch GetSMSReceiversForMobileNetworkMatch(string receiver, string receiverIn, string receiverOut, Guid normalizationRuleDefinitionId,
            DateTime effectiveTime, int switchId, int? customerId, int? supplierId)
        {
            Dictionary<SMSReceiver, ReceiverIdentification> mappingResults = GetCachedMappingSMSReceivers();

            string mobileNetworkReceiver = GetReceiverValueForMobileNetworkMatch(mappingResults.GetRecord(SMSReceiver.MobileNetwork), receiver, receiverIn, receiverOut,
                normalizationRuleDefinitionId, effectiveTime, switchId, customerId, supplierId);

            return new SMSReceiversForMobileNetworkMatch
            {
                MobileNetworkReceiver = mobileNetworkReceiver
            };
        }

        #endregion

        #region Private Methods

        private Dictionary<SMSReceiver, ReceiverIdentification> GetCachedMappingSMSReceivers()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SettingManager.CacheManager>().GetOrCreateObject("GetMappingSMSReceivers",
                () =>
                {
                    ConfigManager configManager = new ConfigManager();
                    Dictionary<SMSReceiver, ReceiverIdentification> mappingResults = new Dictionary<SMSReceiver, ReceiverIdentification>();

                    mappingResults.Add(SMSReceiver.Receiver, configManager.GetGeneralReceiverIndentification());
                    mappingResults.Add(SMSReceiver.CustomerReceiver, configManager.GetCustomerReceiverIdentification());
                    mappingResults.Add(SMSReceiver.SupplierReceiver, configManager.GetSupplierReceiverIdentification());
                    mappingResults.Add(SMSReceiver.MobileNetwork, configManager.GetMobileNetworkReceiverIdentification());

                    return mappingResults;
                });
        }

        private ReceiverIdentification GetCorrespondingReceiverIdentification(ReceiverIdentification? receiverIdentification, ReceiverIdentification defaultReceiverIdentification)
        {
            if (receiverIdentification != null && receiverIdentification.HasValue)
                return receiverIdentification.Value;

            return defaultReceiverIdentification;
        }

        private string GetReceiverValueForIdentification(ReceiverIdentification receiverIdentification, string receiver, string receiverIn, string receiverOut)
        {
            switch (receiverIdentification)
            {
                case ReceiverIdentification.Receiver: return receiver;
                case ReceiverIdentification.ReceiverIn: return receiverIn;
                case ReceiverIdentification.ReceiverOut: return receiverOut;
                default: throw new NotSupportedException("receiverIdentification");
            }
        }

        private string GetReceiverValueForMobileNetworkMatch(ReceiverIdentification receiverIdentification, string receiver, string receiverIn, string receiverOut,
            Guid normalizationRuleDefinitionId, DateTime effectiveTime, int switchId, int? customerId, int? supplierId)
        {
            switch (receiverIdentification)
            {
                case ReceiverIdentification.Receiver: return receiver;
                case ReceiverIdentification.ReceiverIn: return receiverIn;
                case ReceiverIdentification.ReceiverOut: return receiverOut;
                case ReceiverIdentification.NormalizedReceiver: return GetReceiverNormalizedValue(receiver, normalizationRuleDefinitionId, effectiveTime, switchId, customerId, supplierId);
                case ReceiverIdentification.NormalizedReceiverIn: return GetReceiverNormalizedValue(receiverIn, normalizationRuleDefinitionId, effectiveTime, switchId, customerId, supplierId);
                case ReceiverIdentification.NormalizedReceiverOut: return GetReceiverNormalizedValue(receiverOut, normalizationRuleDefinitionId, effectiveTime, switchId, customerId, supplierId);
                default: throw new NotSupportedException("receiverIdentification");
            }
        }

        private string GetReceiverNormalizedValue(string receiver, Guid normalizationRuleDefinitionId, DateTime effectiveTime, int switchId, int? customerId, int? supplierId)
        {
            int receiverChoiceValue = 1;

            var normalizeRuleContext = new Vanrise.GenericData.Normalization.NormalizeRuleContext();
            normalizeRuleContext.Value = receiver;

            var genericRuleTarget = new Vanrise.GenericData.Entities.GenericRuleTarget();
            genericRuleTarget.EffectiveOn = effectiveTime;
            genericRuleTarget.TargetFieldValues = new Dictionary<string, object>();
            genericRuleTarget.TargetFieldValues.Add("NumberType", receiverChoiceValue);
            genericRuleTarget.TargetFieldValues.Add("Switch", switchId);
            genericRuleTarget.TargetFieldValues.Add("Customer", customerId);
            genericRuleTarget.TargetFieldValues.Add("Supplier", supplierId);
            if (!string.IsNullOrEmpty(receiver))
            {
                genericRuleTarget.TargetFieldValues.Add("NumberPrefix", receiver);
                genericRuleTarget.TargetFieldValues.Add("NumberLength", receiver.Length);
            }

            var normalizationRuleManager = new Vanrise.GenericData.Normalization.NormalizationRuleManager();
            normalizationRuleManager.ApplyNormalizationRule(normalizeRuleContext, normalizationRuleDefinitionId, genericRuleTarget);

            return normalizeRuleContext.NormalizedValue;
        }

        #endregion

    }
}
