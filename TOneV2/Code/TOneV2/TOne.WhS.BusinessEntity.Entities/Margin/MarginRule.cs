using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class MarginRule : GenericRule
    {
        public override string GetSettingsDescription(IGenericRuleSettingsDescriptionContext context)
        {
            throw new NotImplementedException();
        }

        public override bool AreSettingsMatched(object ruleDefinitionSettings, object settingsFilterValue)
        {
            throw new NotImplementedException();
        }
    }
}
