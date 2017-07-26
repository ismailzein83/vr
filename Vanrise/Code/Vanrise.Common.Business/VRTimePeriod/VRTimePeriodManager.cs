using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRTimePeriodManager
    {
        public IEnumerable<VRTimePeriodConfig> GetVRTimePeriodConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<VRTimePeriodConfig>(VRTimePeriodConfig.EXTENSION_TYPE);
        }

        public DateTimeRange GetTimePeriod(VRTimePeriod timePeriod, DateTime effectiveDate)
        {
            VRTimePeriodContext context = new VRTimePeriodContext() { EffectiveDate = effectiveDate };
            timePeriod.GetTimePeriod(context);
            return new DateTimeRange() { From = context.FromTime, To = context.ToTime };
        }

        public DateTimeRange GetTimePeriod(VRTimePeriod timePeriod)
        {
            return GetTimePeriod(timePeriod, DateTime.Now);
        }
    }
}