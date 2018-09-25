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
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class StateBackupBusinessEntitiesCleanupManager
    {
        #region Public Methods

        public void CleanupGenericRules(IStateBackupCleanupContext context)
        {
            GenericRuleDefinitionManager genericRuleDefManager = new GenericRuleDefinitionManager();
            IEnumerable<GenericRuleDefinition> ruleDefinitions = genericRuleDefManager.GetGenericRulesDefinitons();

            foreach (GenericRuleDefinition ruleDef in ruleDefinitions)
            {
                var genericRuleDefinitionCriteria = ruleDef.CriteriaDefinition.CastWithValidate<GenericRuleDefinitionCriteria>("ruleDef.CriteriaDefinition", ruleDef.GenericRuleDefinitionId);

                foreach (GenericRuleDefinitionCriteriaField field in genericRuleDefinitionCriteria.Fields)
                {
                    if (field.FieldType is FieldBusinessEntityType)
                    {
                        if (IsFieldTypeTargetForCleanUp(field.FieldType))
                        {
                            CleanRulesAfterRestore(ruleDef, context);
                            break;
                        }
                    }
                }
            }
        }

        public void CleanupRoutingProducts(IStateBackupCleanupContext context)
        {
            //TODO: to be implemented by MJA
        }

        #endregion

        #region Private Methods

        private void CleanRulesAfterRestore(GenericRuleDefinition genericRuleDefinition, IStateBackupCleanupContext context)
        {
            IGenericRuleManager manager = GetManager(genericRuleDefinition.GenericRuleDefinitionId);
            IEnumerable<GenericRule> genericRules = manager.GetGenericRulesByDefinitionId(genericRuleDefinition.GenericRuleDefinitionId);
            var genericRuleDefinitionCriteria = genericRuleDefinition.CriteriaDefinition.CastWithValidate<GenericRuleDefinitionCriteria>("genericRuleDefinition.CriteriaDefinition");

            foreach (GenericRuleDefinitionCriteriaField criteriaDefinitionField in genericRuleDefinitionCriteria.Fields)
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

                                    if (context.SaleZoneIds != null && IsFieldofTypeSaleZone(businessEntityType))
                                        ProcessSaleZoneFields(context.SaleZoneIds, rule, manager, businessEntitiesValues);
                                    else if (context.SupplierZoneIds != null && IsFieldofTypeSupplierZone(businessEntityType))
                                        ProcessSupplierZoneFields(context.SupplierZoneIds, rule, manager, businessEntitiesValues);
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

        #endregion
    }
}
