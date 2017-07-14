using System;
using System.Activities;
using System.Collections.Generic;
using Retail.Cost.Entities;
using Retail.Cost.Business;

namespace Retail.Cost.BP.Activities
{
    public sealed class GetCDRCostTechnicalSettingsData : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<CDRCostTechnicalSettingData> CDRCostTechnicalSettingData { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ConfigManager configManager = new ConfigManager();
            CDRCostTechnicalSettingData cdrCostTechnicalSettingData = configManager.GetCDRCostTechnicalSettingData();

            this.CDRCostTechnicalSettingData.Set(context, cdrCostTechnicalSettingData);
        }
    }
}