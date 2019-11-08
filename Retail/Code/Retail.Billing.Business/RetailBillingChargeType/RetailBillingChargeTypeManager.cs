﻿using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Retail.Billing.Business
{
    public class RetailBillingChargeTypeManager
    {
        #region Public Methods

        public IEnumerable<RetailBillingChargeTypeInfo> GetRetailBillingChargeTypeInfo(Guid? targetRecordTypeId)
        {
            Func<RetailBillingChargeType, bool> filterExpression = (chargeType) =>
            {
                if (targetRecordTypeId.HasValue)
                {
                    if (chargeType != null && chargeType.Settings != null && chargeType.Settings.ExtendedSettings != null && chargeType.Settings.ExtendedSettings.TargetRecordTypeId.HasValue)
                        if (chargeType.Settings.ExtendedSettings.TargetRecordTypeId.Value != targetRecordTypeId.Value)
                            return false;
                }
                return true;
            };

            return this.GetCachedRetailBillingChargeTypes().MapRecords(RetailBillingChargeTypeInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public RetailBillingChargeType GetRetailBillingChargeType(Guid retailBillingChargeTypeId)
        {
            if (this.GetCachedRetailBillingChargeTypes().TryGetValue(retailBillingChargeTypeId, out RetailBillingChargeType retailBillingChargeType))
                return retailBillingChargeType;
            else
                throw new Exception($"No Retail Billing Charge Type with ID : {retailBillingChargeTypeId}");
        }

        public IEnumerable<RetailBillingChargeTypeExtendedSettingsConfig> GetChargeTypeExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<RetailBillingChargeTypeExtendedSettingsConfig>(RetailBillingChargeTypeExtendedSettingsConfig.EXTENSION_TYPE);
        }

        #endregion

        #region Private Methods

        private Dictionary<Guid, RetailBillingChargeType> GetCachedRetailBillingChargeTypes()
        {
            return new Vanrise.Common.Business.VRComponentTypeManager().GetCachedComponentTypes<RetailBillingChargeTypeSettings, RetailBillingChargeType>();
        }

        #endregion

        #region Mappers

        public RetailBillingChargeTypeInfo RetailBillingChargeTypeInfoMapper(RetailBillingChargeType retailBillingChargeType)
        {
            return new RetailBillingChargeTypeInfo
            {
                Name = retailBillingChargeType.Name,
                RetailBillingChargeTypeId = retailBillingChargeType.VRComponentTypeId,
                RuntimeEditor = retailBillingChargeType.Settings.ExtendedSettings.RuntimeEditor
            };
        }

        #endregion
    }
}