using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class MarginRule : GenericRule
    {
        public MarginRuleSettings Settings { get; set; }

        public override string GetSettingsDescription(IGenericRuleSettingsDescriptionContext context)
        {
            Settings.ThrowIfNull("Settings");
            Settings.MarginSettingItems.ThrowIfNull("Settings.MarginSettingItems");
            if (Settings.MarginSettingItems.Count == 0)
                throw new NullReferenceException("Settings.MarginSettingItems");

            var genericRuleDefinition = new GenericRuleDefinitionManager().GetGenericRuleDefinition(this.DefinitionId);
            genericRuleDefinition.ThrowIfNull("genericRuleDefinition", this.DefinitionId);
            genericRuleDefinition.SettingsDefinition.ThrowIfNull("genericRuleDefinition.SettingsDefinition", this.DefinitionId);

            var marginRuleDefinitionSettings = genericRuleDefinition.SettingsDefinition.CastWithValidate<MarginRuleDefinitionSettings>("genericRuleDefinition.SettingsDefinition"); ;
            var marginCategoryBEDefinitionId = marginRuleDefinitionSettings.MarginCategoryBEDefinitionId;

            StatusDefinitionManager statusDefinitionManager = new StatusDefinitionManager();

            List<string> itemsDescription = new List<string>();
            decimal lastUpToRate=0;
            for (int i = 0; i < Settings.MarginSettingItems.Count - 1; i++)
            {
                var marginSettingItem = Settings.MarginSettingItems[i];
                var statusDefinition = statusDefinitionManager.GetStatusDefinition(marginSettingItem.Category);
                itemsDescription.Add($"Up To {marginSettingItem.UpTo}: {statusDefinition.Name}");
                lastUpToRate = marginSettingItem.UpTo.Value;
            }

            var lastMarginSettingItem = Settings.MarginSettingItems.Last();
            var statusDefinitionForLastItem = statusDefinitionManager.GetStatusDefinition(lastMarginSettingItem.Category);
            //itemsDescription.Add($"Up To +∞ : {statusDefinitionForLastItem.Name}");
            itemsDescription.Add($"Greater Than {lastUpToRate}: {statusDefinitionForLastItem.Name}");

            return string.Join("; ", itemsDescription);
        }

        public override bool AreSettingsMatched(object ruleDefinitionSettings, object settingsFilterValue)
        {
            return true;
        }
    }
}
