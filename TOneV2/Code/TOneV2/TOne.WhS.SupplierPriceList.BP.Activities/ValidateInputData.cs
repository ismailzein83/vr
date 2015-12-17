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
        public OutArgument<bool> StopExecution { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<CodeRuleTarget> codeTargets = new List<CodeRuleTarget>();
            List<ZoneRuleTarget> zoneTargets = new List<ZoneRuleTarget>();

            this.Validate(context, codeTargets, zoneTargets);
            this.ApplyValidationRules(context, codeTargets, zoneTargets);
        }

        private void Validate(CodeActivityContext context, List<CodeRuleTarget> codeTargets, List<ZoneRuleTarget> zoneTargets)
        {
            IEnumerable<ImportedZone> importedZonesList = this.ImportedZones.Get(context);

            CodeGroupManager codeGroupManager = new CodeGroupManager();

            foreach (ImportedZone zone in importedZonesList)
            {
                if (zone.ImportedCodes != null)
                {
                    var firstCode = zone.ImportedCodes.FirstOrDefault();
                    if (firstCode != null && !firstCode.IsExecluded)
                    {
                        int countryIdOfFirstCode = firstCode.CodeGroupId != null ? codeGroupManager.GetCodeGroup(firstCode.CodeGroupId.Value).CountryId : -1;
                        Func<ImportedCode, bool> pred = new Func<ImportedCode, bool>((code) =>
                        !code.IsExecluded && codeGroupManager.GetCodeGroup(code.CodeGroupId.Value) != null 
                            && codeGroupManager.GetCodeGroup(code.CodeGroupId.Value).CountryId != countryIdOfFirstCode);

                        if (zone.ImportedCodes.Any(pred))
                        {
                            ZoneRuleTarget zoneTarget = new ZoneRuleTarget(zone);
                            zoneTargets.Add(zoneTarget);
                        }
                    }

                    foreach (ImportedCode code in zone.ImportedCodes)
                    {
                        if (code.CodeGroupId == null)
                        {
                            codeTargets.Add(new CodeRuleTarget()
                            {
                                ImportedCode = code
                            });
                        }
                    }
                }
            }
        }

        private void ApplyValidationRules(CodeActivityContext context, List<CodeRuleTarget> codeTargets, List<ZoneRuleTarget> zoneTargets)
        {
            IEnumerable<BusinessRule> rules = this.GetRulesConfiguration();
            rules.Where(x => x.ActionType == RuleActionType.StopExecution).ToList().ForEach((rule) =>
            {
                if (rule.RuleTargetType == typeof(CodeRuleTarget))
                {
                    if (codeTargets.Count > 0)
                        this.StopExecution.Set(context, true);
                }
            });

            rules.Where(x => x.ActionType == RuleActionType.ExecludeItem).ToList().ForEach((rule) =>
            {
                if (rule.RuleTargetType == typeof(ZoneRuleTarget))
                {
                    foreach (ZoneRuleTarget target in zoneTargets)
                    {
                        target.SetExecluded();
                    }
                }
            });
        }

        private IEnumerable<BusinessRule> GetRulesConfiguration()
        {
            BusinessRule codeGroupNotFoundRule = new BusinessRule()
            {
                CheckType = "Code Group Not Found",
                ActionType = RuleActionType.StopExecution,
                RuleTargetType = typeof(CodeRuleTarget)
            };

            List<BusinessRule> rules = new List<BusinessRule>();
            rules.Add(codeGroupNotFoundRule);

            return rules;
        }
    }
}
