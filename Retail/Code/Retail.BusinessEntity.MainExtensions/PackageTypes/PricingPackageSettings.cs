﻿using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{
    public class PricingPackageSettings : PackageExtendedSettings, IPackageFixedChargingPolicy, IPackageUsageChargingPolicy
    {
        public int? FixedChargingPolicyId { get; set; }

        public Dictionary<Guid, ServiceTypeUsageChargingPolicySettings> ServiceTypeUsageChargingPolicies { get; set; }


        public override PackageExtendedSettingsEditorRuntime GetEditorRuntime()
        {
            PricingPackageSettingsEditorRuntime pricingPackageSettingsEditorRuntime = new PricingPackageSettingsEditorRuntime();
            pricingPackageSettingsEditorRuntime.ServiceTypes = new Dictionary<Guid, string>();
            pricingPackageSettingsEditorRuntime.ChargingPolicies = new Dictionary<int, string>();

            ServiceTypeManager serviceTypeManager = new ServiceTypeManager();
            ChargingPolicyManager chargingPolicyManager = new ChargingPolicyManager();

            foreach (var serviceTypeId in ServiceTypeUsageChargingPolicies.Keys)
            {
                if (!pricingPackageSettingsEditorRuntime.ServiceTypes.ContainsKey(serviceTypeId))
                    pricingPackageSettingsEditorRuntime.ServiceTypes.Add(serviceTypeId, serviceTypeManager.GetServiceTypeName(serviceTypeId));
            }

            foreach (var itm in ServiceTypeUsageChargingPolicies.Values)
            {
                if (!pricingPackageSettingsEditorRuntime.ChargingPolicies.ContainsKey(itm.UsageChargingPolicyId))
                    pricingPackageSettingsEditorRuntime.ChargingPolicies.Add(itm.UsageChargingPolicyId, chargingPolicyManager.GetChargingPolicyName(itm.UsageChargingPolicyId));
            }

            return pricingPackageSettingsEditorRuntime;
        }

        bool IPackageFixedChargingPolicy.TryGetFixedChargingPolicyId(IPackageFixedChargingPolicyContext context)
        {
            if (this.FixedChargingPolicyId.HasValue)
            {
                context.ChargingPolicyId = this.FixedChargingPolicyId.Value;
                return true;
            }
            else
                return false;
        }

        bool IPackageUsageChargingPolicy.TryGetServiceUsageChargingPolicyId(IPackageServiceUsageChargingPolicyContext context)
        {
            ServiceTypeUsageChargingPolicySettings serviceUsageChargingPolicySettings;
            if (this.ServiceTypeUsageChargingPolicies != null && this.ServiceTypeUsageChargingPolicies.TryGetValue(context.ServiceTypeId, out serviceUsageChargingPolicySettings))
            {
                context.ChargingPolicyId = serviceUsageChargingPolicySettings.UsageChargingPolicyId;
                return true;
            }
            else
                return false;
        }

        public override void ValidatePackageAssignment(IPackageSettingAssignementValidateContext context)
        {
            context.IsValid = true;
        }
    }

    public class ServiceTypeUsageChargingPolicySettings
    {
        public int UsageChargingPolicyId { get; set; }
    }

    public class PricingPackageSettingsEditorRuntime : PackageExtendedSettingsEditorRuntime
    {
        public Dictionary<Guid, string> ServiceTypes { get; set; }

        public Dictionary<int, string> ChargingPolicies { get; set; }
    }
}
