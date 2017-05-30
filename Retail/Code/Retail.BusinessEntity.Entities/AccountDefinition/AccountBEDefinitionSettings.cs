﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class AccountBEDefinitionSettings : Vanrise.GenericData.Entities.BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("70D4A6AD-10CC-4F0B-8364-7D8EF3C044C4");
        public override Guid ConfigId { get { return s_configId; } }

        public override string ManagerFQTN
        {
            get { return "Retail.BusinessEntity.Business.AccountBEManager, Retail.BusinessEntity.Business"; }
        }

        public override string DefinitionEditor
        {
            get { return "retail-be-accountbedefinitions-editor"; }
        }

        public override string IdType
        {
            get { return "System.Int64"; }
        }

        public override string SelectorFilterEditor
        {
            get { return "retail-be-accountbe-runtimeselectorfilter"; }
        }

        public override string SelectorUIControl
        {
            get { return "retail-be-account-selector"; }
        }

        public bool UseRemoteSelector { get; set; }
        public Guid StatusBEDefinitionId { get; set; }
        public List<AccountExtraFieldDefinition> AccountExtraFieldDefinitions { get; set; }
        public AccountGridDefinition GridDefinition { get; set; }
        public List<AccountViewDefinition> AccountViewDefinitions { get; set; }
        public List<AccountActionDefinition> ActionDefinitions { get; set; }
        public FixedChargingDefinition FixedChargingDefinition { get; set; }

        public FinancialAccountLocator FinancialAccountLocator { get; set; }

        public AccountBEDefinitionSecurity Security { get; set; }

        public override Vanrise.Entities.VRLoggableEntityBase GetLoggableEntity(Vanrise.GenericData.Entities.IBusinessEntityDefinitionSettingsGetLoggableEntityContext context)
        {
            return BEManagerFactory.GetManager<IAccountBEManager>().GetAccountLoggableEntity(context.BEDefinition.BusinessEntityDefinitionId);
        }
    }

    public class AccountExtraFieldDefinition
    {
        public Guid AccountExtraFieldDefinitionId { get; set; }

        public string Name { get; set; }

        public AccountExtraFieldDefinitionSettings Settings { get; set; }
    }
    public abstract class AccountExtraFieldDefinitionSettings
    {
        public abstract Guid ConfigId  { get; }

        public abstract IEnumerable<AccountGenericField> GetFields(IAccountExtraFieldSettingsContext context);
    }
    public interface IAccountExtraFieldSettingsContext
    {
    }
    public abstract class AccountBEDefinitionCondition
    {
        public abstract Guid ConfigId { get; }

        public abstract bool Evaluate(IAccountConditionEvaluationContext context);
    }
    public interface IAccountBEDefinitionConditionContext
    {
        AccountBEDefinitionSettings AccountBEDefinitionSettings { get; }
    }

    public class AccountBEDefinitionSecurity
    {
        public RequiredPermissionSettings ViewRequiredPermission { get; set; }
        public RequiredPermissionSettings AddRequiredPermission { get; set; }
        public RequiredPermissionSettings EditRequiredPermission { get; set; }

        public RequiredPermissionSettings ViewPackageRequiredPermission { get; set; }
        public RequiredPermissionSettings AddPackageRequiredPermission { get; set; }
        public RequiredPermissionSettings EditPackageRequiredPermission { get; set; }

        public RequiredPermissionSettings ViewAccountPackageRequiredPermission { get; set; }
        public RequiredPermissionSettings AddAccountPackageRequiredPermission { get; set; }

        public RequiredPermissionSettings ViewProductRequiredPermission { get; set; }
        public RequiredPermissionSettings AddProductRequiredPermission { get; set; }
        public RequiredPermissionSettings EditProductRequiredPermission { get; set; }
    }
}
