using System.Activities;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Business;

namespace TOne.WhS.Deal.BP.Activities
{
    public sealed class GetDealTechnicalSettingData : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<DealTechnicalSettingData> DealTechnicalSettingData { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ConfigManager configManager = new ConfigManager();
            DealTechnicalSettingData dealTechnicalSettingData = configManager.GetDealTechnicalSettingData();

            this.DealTechnicalSettingData.Set(context, dealTechnicalSettingData);
        }
    }
}
