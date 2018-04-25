using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;
using TOne.WhS.RouteSync.Ericsson.Business;
using System.Text;
using Vanrise.Entities;
using Vanrise.Rules;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace TOne.WhS.RouteSync.Ericsson
{
    public partial class EricssonSWSync : SwitchRouteSynchronizer
    {
        private class OriginalTrunkGroupRule
        {
            public int SupplierId { get; set; }

            public List<int> CustomerIds { get; set; }

            public List<string> Codes { get; set; }

            public bool IsBackUp { get; set; }
        }

        private class TrunkGroupRuleAsGeneric : IVRRule, IGenericRule
        {
            public int SupplierId { get; set; }

            public TrunkGroup TrunkGroup { get; set; }

            public DateTime BeginEffectiveTime { get { return DateTime.MinValue; } set { } }

            public DateTime? EndEffectiveTime { get { return null; } set { } }

            public TimeSpan RefreshTimeSpan { get { return TimeSpan.MaxValue; } }

            public long GetPriorityIfSameCriteria(IRuleGetPriorityContext context)
            {
                return 0;
            }

            public bool IsAnyCriteriaExcluded(object target)
            {
                return false;
            }

            public DateTime? LastRefreshedTime { get; set; }

            public void RefreshRuleState(IRefreshRuleStateContext context)
            {
            }

            public GenericRuleCriteria Criteria
            {
                get
                {
                    GenericRuleCriteria tempGenericRuleCriteria = new GenericRuleCriteria();
                    tempGenericRuleCriteria.FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>();
                    tempGenericRuleCriteria.FieldsValues.Add("Supplier", new StaticValues() { Values = new List<object>() { SupplierId } });
                    tempGenericRuleCriteria.FieldsValues.Add("Code", new StaticValues() { Values = new List<object>() { TrunkGroup.CodeGroupTrunkGroups.Select(itm => itm.CodeGroup) } });
                    tempGenericRuleCriteria.FieldsValues.Add("Customer", new StaticValues() { Values = new List<object>() { TrunkGroup.CustomerTrunkGroups.Select(itm => itm.CustomerId) } });
                    tempGenericRuleCriteria.FieldsValues.Add("IsBackUp", new StaticValues() { Values = new List<object>() { TrunkGroup.IsBackup } });

                    return tempGenericRuleCriteria;
                }
            }
        }

        RuleTree BuildSupplierTrunkGroupTree()
        {
            GenericRuleDefinitionCriteria criteriaDefinition = null;
            List<TrunkGroupRuleAsGeneric> rules = new List<TrunkGroupRuleAsGeneric>();
            if (CarrierMappings != null)
            {
                foreach (var kvp in CarrierMappings)
                {
                    SupplierMapping supplierMapping = kvp.Value.SupplierMapping;
                    if (supplierMapping != null && supplierMapping.TrunkGroups != null)
                    {
                        foreach (TrunkGroup trunkGroup in supplierMapping.TrunkGroups)
                        {
                            TrunkGroupRuleAsGeneric supplierRule = new TrunkGroupRuleAsGeneric()
                            {
                                SupplierId = kvp.Value.CarrierId,
                                TrunkGroup = trunkGroup
                            };
                            rules.Add(supplierRule);
                        }
                    }
                }
            }

            if (rules.Count > 0)
            {
                var ruleTree = GenericRuleManager<GenericRule>.BuildRuleTree<TrunkGroupRuleAsGeneric>(criteriaDefinition, rules);
                var criteriaEvaluationInfos = GenericRuleManager<GenericRule>.BuildCriteriaEvaluationInfos(criteriaDefinition, null);
                return ruleTree;
            }
            else
            {
                return null;
            }
        }
    }
}