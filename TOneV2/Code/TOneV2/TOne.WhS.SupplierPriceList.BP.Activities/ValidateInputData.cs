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
            IEnumerable<ImportedZone> importedZones = this.ImportedZones.Get(context);
            IEnumerable<ImportedCode> importedCodes = this.ImportedCodes.Get(context);

            List<IRuleTarget> targets = new List<IRuleTarget>();
            targets.AddRange(importedCodes);
            targets.AddRange(importedZones);

            this.ExecuteValidation(context, targets);
        }

        private void ExecuteValidation(CodeActivityContext context, IEnumerable<IRuleTarget> targets)
        {
            IEnumerable<BusinessRule> rules = this.GetRulesConfiguration();

            foreach (BusinessRule rule in rules)
            {
                foreach (string targetFQTN in rule.TargetFQTNList)
                {
                    Type targetType = Type.GetType(targetFQTN);
                    foreach (IRuleTarget target in targets)
                    {
                        if (target.GetType() == targetType)
                        {
                            bool valid = rule.Validate(target);

                            if (!valid)
                            {
                                switch (rule.ActionType)
                                {
                                    case RuleActionType.StopExecution:
                                        this.StopExecution.Set(context, true);
                                        return;
                                    case RuleActionType.ExecludeItem:
                                        target.SetExcluded();
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<BusinessRule> GetRulesConfiguration()
        {
            List<string> codeGroupRuleTargets = new List<string>();
            codeGroupRuleTargets.Add("TOne.WhS.SupplierPriceList.Entities.SPL.ImportedCode, TOne.WhS.SupplierPriceList.Entities");

            BusinessRule codeGroupNotFoundRule = new CodeGroupRule()
            {
                CheckType = "Code Group Not Found",
                ActionType = RuleActionType.StopExecution,
                TargetFQTNList = codeGroupRuleTargets
            };

            List<string> multCountryRuleTargets = new List<string>();
            multCountryRuleTargets.Add("TOne.WhS.SupplierPriceList.Entities.SPL.ImportedZone, TOne.WhS.SupplierPriceList.Entities");

            BusinessRule multipleCountriesInSameZoneRule = new MultipleCountryRule()
            {
                CheckType = "Multiple Countries are found in Same Zone",
                ActionType = RuleActionType.ExecludeItem,
                TargetFQTNList = multCountryRuleTargets
            };

            List<BusinessRule> rules = new List<BusinessRule>();
            rules.Add(codeGroupNotFoundRule);
            rules.Add(multipleCountriesInSameZoneRule);

            return rules;
        }
    }
}
