using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace TOne.WhS.BusinessEntity.Business
{
    public class GenericRuleCleanupManager
    {
        public void Cleanup(Dictionary<CleanupDataType, object> data)
        {
            GenericRuleDefinitionManager genericRuleDefManager = new GenericRuleDefinitionManager();
            IEnumerable<GenericRuleDefinition> ruleDefinitions = genericRuleDefManager.GetGenericRulesDefinitons();

            foreach (GenericRuleDefinition ruleDef in ruleDefinitions)
            {
                foreach (GenericRuleDefinitionCriteriaField field in ruleDef.CriteriaDefinition.Fields)
                {
                    if (field.FieldType is FieldBusinessEntityType)
                    {
                        if (IsFieldTypeTargetForCleanUp(field.FieldType))
                        {
                            CleanRulesAfterRestore(ruleDef, data);
                            break;
                        }
                    }
                }
            }
        }

        private void CleanRulesAfterRestore(GenericRuleDefinition genericRuleDefinition, Dictionary<CleanupDataType, object> data)
        {
            IGenericRuleManager manager = GetManager(genericRuleDefinition.GenericRuleDefinitionId);
            IEnumerable<GenericRule> genericRules = manager.GetGenericRulesByDefinitionId(genericRuleDefinition.GenericRuleDefinitionId);
            foreach (GenericRuleDefinitionCriteriaField criteriaDefinitionField in genericRuleDefinition.CriteriaDefinition.Fields)
            {
                foreach (GenericRule rule in genericRules)
                {
                    if (IsFieldTypeTargetForCleanUp(criteriaDefinitionField.FieldType))
                    {
                        GenericRuleCriteriaFieldValues fieldValues = null;
                        if (rule.Criteria != null)
                        {
                            rule.Criteria.FieldsValues.TryGetValue(criteriaDefinitionField.FieldName, out fieldValues);
                            if (fieldValues != null)
                            {
                                BusinessEntityValues businessEntitiesValues = fieldValues as BusinessEntityValues;
                                if (businessEntitiesValues.BusinessEntityGroup != null)
                                {
                                    FieldBusinessEntityType businessEntityType = criteriaDefinitionField.FieldType as FieldBusinessEntityType;

                                    if (data.ContainsKey(CleanupDataType.SaleZone) && IsFieldofTypeSaleZone(businessEntityType))
                                    {
                                        IEnumerable<long> deletedSaleZoneIds = data[CleanupDataType.SaleZone] as IEnumerable<long>;
                                        ProcessSaleZoneFields(deletedSaleZoneIds, rule, manager, businessEntitiesValues);
                                    }

                                    else if (data.ContainsKey(CleanupDataType.SupplierZone) && IsFieldofTypeSupplierZone(businessEntityType))
                                    {
                                        IEnumerable<long> deletedSupplierZoneIds = data[CleanupDataType.SupplierZone] as IEnumerable<long>;
                                        ProcessSupplierZoneFields(deletedSupplierZoneIds, rule, manager, businessEntitiesValues);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool IsFieldTypeTargetForCleanUp(DataRecordFieldType fieldType)
        {
            FieldBusinessEntityType businessEntityType = fieldType as FieldBusinessEntityType;
            return IsFieldofTypeSaleZone(businessEntityType) || IsFieldofTypeSupplierZone(businessEntityType);
        }

        private bool IsFieldofTypeSaleZone(FieldBusinessEntityType businessEntityType)
        {
            return businessEntityType.BusinessEntityDefinitionId == SaleZone.BUSINESSENTITY_DEFINITION_ID;
        }

        private bool IsFieldofTypeSupplierZone(FieldBusinessEntityType businessEntityType)
        {
            return businessEntityType.BusinessEntityDefinitionId == SupplierZone.BUSINESSENTITY_DEFINITION_ID;
        }

        private void ProcessSaleZoneFields(IEnumerable<long> deletedSaleZoneIds, GenericRule rule, IGenericRuleManager manager, BusinessEntityValues businessEntitiesValues)
        {
            SaleZoneGroupSettings saleZoneGroup = businessEntitiesValues.BusinessEntityGroup as SaleZoneGroupSettings;
            ISaleZoneGroupCleanupContext cleanupContext = new SaleZoneGroupCleanupContext() { DeletedSaleZoneIds = deletedSaleZoneIds };
            saleZoneGroup.CleanDeletedZoneIds(cleanupContext);
            switch (cleanupContext.Result)
            {
                case SaleZoneGroupCleanupResult.AllZonesRemoved:
                    manager.DeleteGenericRule(rule.RuleId);
                    break;
                case SaleZoneGroupCleanupResult.ZonesUpdated:
                    manager.UpdateGenericRule(rule);
                    break;
            }
        }

        private void ProcessSupplierZoneFields(IEnumerable<long> deletedSupplierZoneIds, GenericRule rule, IGenericRuleManager manager, BusinessEntityValues businessEntitiesValues)
        {
            SupplierZoneGroup saleZoneGroup = businessEntitiesValues.BusinessEntityGroup as SupplierZoneGroup;
            ISupplierZoneGroupCleanupContext cleanupContext = new SupplierZoneGroupCleanupContext() { DeletedSupplierZoneIds = deletedSupplierZoneIds };
            saleZoneGroup.CleanDeletedZoneIds(cleanupContext);
            switch (cleanupContext.Result)
            {
                case SupplierZoneGroupCleanupResult.AllZonesRemoved:
                    manager.DeleteGenericRule(rule.RuleId);
                    break;
                case SupplierZoneGroupCleanupResult.ZonesUpdated:
                    manager.UpdateGenericRule(rule);
                    break;
            }
        }

        private IGenericRuleManager GetManager(Guid ruleDefinitionId)
        {
            GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
            GenericRuleDefinition ruleDefinition = ruleDefinitionManager.GetGenericRuleDefinition(ruleDefinitionId);

            GenericRuleTypeConfigManager ruleTypeManager = new GenericRuleTypeConfigManager();
            GenericRuleTypeConfig ruleTypeConfig = ruleTypeManager.GetGenericRuleTypeById(ruleDefinition.SettingsDefinition.ConfigId);

            Type managerType = Type.GetType(ruleTypeConfig.RuleManagerFQTN);
            return Activator.CreateInstance(managerType) as IGenericRuleManager;
        }
    }
}
