using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ValidateInputData : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public OutArgument<bool> StopExecution { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<BaseBusinessRule> rules = this.GetRulesConfiguration(context);

            rules.Where(x => x.ActionType == RuleActionType.StopExecution).ToList().ForEach((rule) =>
            {
                if (!rule.isValid())
                    this.StopExecution.Set(context, true);
            });

            rules.Where(x => x.ActionType == RuleActionType.ExecludeItem).ToList().ForEach((rule) =>
            {
                rule.SetExecluded();
            });
        }

        private IEnumerable<BaseBusinessRule> GetRulesConfiguration(CodeActivityContext context)
        {
            IEnumerable<ImportedZone> importedZones = this.ImportedZones.Get(context);
            IEnumerable<ImportedCode> importedCodes = this.ImportedCodes.Get(context);

            CodeRuleTarget codeGroupNotFoundRule = new CodeRuleTarget()
            {
                CheckType = "Code Group Not Found",
                ActionType = RuleActionType.StopExecution,
                data = importedCodes
            };

            ZoneRuleTarget multipleCountriesInSameZoneRule = new ZoneRuleTarget()
            {
                CheckType = "Multiple Countries are found in Same Zone",
                ActionType = RuleActionType.StopExecution,
                data = importedZones
            };

            List<BaseBusinessRule> rules = new List<BaseBusinessRule>();
            rules.Add(codeGroupNotFoundRule);
            rules.Add(multipleCountriesInSameZoneRule);

            return rules;
        }
    }
}
