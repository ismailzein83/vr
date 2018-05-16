using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.RouteSync.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.Rules;

namespace TOne.WhS.RouteSync.Ericsson
{
    public partial class EricssonSWSync : SwitchRouteSynchronizer
    {
        public RuleTree BuildSupplierTrunkGroupTree(Dictionary<string, CarrierMapping> carrierMappings)
        {
            GenericRuleDefinitionCriteria criteriaDefinition = this.GetGenericRuleDefinitionCriteria();
            List<TrunkGroupRuleAsGeneric> rules = new List<TrunkGroupRuleAsGeneric>();

            if (carrierMappings != null)
            {
                foreach (var kvp in carrierMappings)
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
                return ruleTree;
            }
            else
            {
                return null;
            }
        }

        private GenericRuleDefinitionCriteria GetGenericRuleDefinitionCriteria()
        {
            GenericRuleDefinitionCriteriaField supplierCriteriaField = new GenericRuleDefinitionCriteriaField()
            {
                FieldName = "Supplier",
                Title = "Supplier",
                FieldType = new FieldNumberType() { DataType = FieldNumberDataType.Int, DataPrecision = FieldNumberPrecision.Normal },
                //FieldType = new FieldBusinessEntityType() { BusinessEntityDefinitionId = new Guid("8C286BCD-5766-487A-8B32-5D167EC342C0"), IsNullable = false },
                RuleStructureBehaviorType = MappingRuleStructureBehaviorType.ByKey,
                Priority = 1
            };

            GenericRuleDefinitionCriteriaField codeGroupCriteriaField = new GenericRuleDefinitionCriteriaField()
            {
                FieldName = "CodeGroup",
                Title = "CodeGroup",
                FieldType = new FieldNumberType() { DataType = FieldNumberDataType.Int, DataPrecision = FieldNumberPrecision.Normal },
                RuleStructureBehaviorType = MappingRuleStructureBehaviorType.ByKey,
                Priority = 2
            };

            GenericRuleDefinitionCriteriaField customerCriteriaField = new GenericRuleDefinitionCriteriaField()
            {
                FieldName = "Customer",
                Title = "Customer",
                FieldType = new FieldNumberType() { DataType = FieldNumberDataType.Int, DataPrecision = FieldNumberPrecision.Normal },
                //FieldType = new FieldBusinessEntityType() { BusinessEntityDefinitionId = new Guid("BA5A57BD-1F03-440F-A469-463A48762B8F"), IsNullable = true },
                RuleStructureBehaviorType = MappingRuleStructureBehaviorType.ByKey,
                Priority = 3
            };

            GenericRuleDefinitionCriteriaField isBackupCriteriaField = new GenericRuleDefinitionCriteriaField()
            {
                FieldName = "IsBackUp",
                Title = "IsBackUp",
                FieldType = new FieldBooleanType(),
                RuleStructureBehaviorType = MappingRuleStructureBehaviorType.ByKey,
                Priority = 4
            };

            GenericRuleDefinitionCriteria criteriaDefinition = new GenericRuleDefinitionCriteria();
            criteriaDefinition.Fields = new List<GenericRuleDefinitionCriteriaField>() { supplierCriteriaField, codeGroupCriteriaField, customerCriteriaField, isBackupCriteriaField };
            return criteriaDefinition;
        }
    }

    public class TrunkGroupRuleAsGeneric : IGenericRule, IVRRule
    {
        public int SupplierId { get; set; }

        public TrunkGroup TrunkGroup { get; set; }

        public DateTime BeginEffectiveTime { get { return DateTime.MinValue; } set { } }

        public DateTime? EndEffectiveTime { get { return null; } set { } }

        public TimeSpan RefreshTimeSpan { get { return TimeSpan.MaxValue; } }

        public DateTime? LastRefreshedTime { get; set; }

        public GenericRuleCriteria Criteria
        {
            get
            {
                GenericRuleCriteria tempGenericRuleCriteria = new GenericRuleCriteria();
                tempGenericRuleCriteria.FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>();
                tempGenericRuleCriteria.FieldsValues.Add("Supplier", new StaticValues() { Values = new List<object>() { SupplierId } });
                tempGenericRuleCriteria.FieldsValues.Add("IsBackUp", new StaticValues() { Values = new List<object>() { TrunkGroup.IsBackup } });

                if (TrunkGroup.CodeGroupTrunkGroups != null && TrunkGroup.CodeGroupTrunkGroups.Count > 0)
                {
                    List<object> codeGroupValues = new List<object>();
                    foreach (var CodeGroupTrunkGroup in TrunkGroup.CodeGroupTrunkGroups)
                    {
                        codeGroupValues.Add(CodeGroupTrunkGroup.CodeGroupId);
                    }
                    tempGenericRuleCriteria.FieldsValues.Add("CodeGroup", new StaticValues() { Values = codeGroupValues });
                }

                if (TrunkGroup.CustomerTrunkGroups != null && TrunkGroup.CustomerTrunkGroups.Count > 0)
                {
                    tempGenericRuleCriteria.FieldsValues.Add("Customer", new StaticValues() { Values = TrunkGroup.CustomerTrunkGroups.Select(itm => itm.CustomerId as object).ToList() });
                }

                return tempGenericRuleCriteria;
            }
        }

        public long GetPriorityIfSameCriteria(IRuleGetPriorityContext context)
        {
            return 0;
        }

        public bool IsAnyCriteriaExcluded(object target)
        {
            return false;
        }

        public new void RefreshRuleState(IRefreshRuleStateContext context)
        {
        }
    }
}