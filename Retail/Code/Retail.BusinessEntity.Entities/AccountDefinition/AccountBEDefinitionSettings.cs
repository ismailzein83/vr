﻿using System;
using System.Collections.Generic;
using Vanrise.Security.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class AccountBEDefinitionSettings : Vanrise.GenericData.Entities.BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("70D4A6AD-10CC-4F0B-8364-7D8EF3C044C4");
        public override Guid ConfigId { get { return s_configId; } }

        public override string ManagerFQTN { get { return "Retail.BusinessEntity.Business.AccountBEManager, Retail.BusinessEntity.Business"; } }

        public override string DefinitionEditor { get { return "retail-be-accountbedefinitions-editor"; } }

        public override string IdType { get { return "System.Int64"; } }

        public override string SelectorFilterEditor { get { return "retail-be-accountbe-runtimeselectorfilter"; } }

        public override string SelectorUIControl { get { return "retail-be-account-selector"; } }

        public Guid StatusBEDefinitionId { get; set; }

        public bool UseRemoteSelector { get; set; }

        public Guid? LocalServiceAccountTypeId { get; set; }

        public List<AccountBulkAction> AccountBulkActions { get; set; }

        public FinancialAccountLocator FinancialAccountLocator { get; set; }

        public List<AccountExtraFieldDefinition> AccountExtraFieldDefinitions { get; set; }

        public AccountGridDefinition GridDefinition { get; set; }

        public List<AccountViewDefinition> AccountViewDefinitions { get; set; }

        public List<AccountActionDefinition> ActionDefinitions { get; set; }

        public List<AccountActionGroupDefinition> ActionGroupDefinitions { get; set; }

        public FixedChargingDefinition FixedChargingDefinition { get; set; }

        public AccountCondition PackageAssignmentCondition { get; set; }

        public bool UseFinancialAccountModule { get; set; }

        public bool UseRecurringChargeModule { get; set; }

        public AccountBEDefinitionSecurity Security { get; set; }

        public List<AccountBEClassification> Classifications { get; set; }

        public override Dictionary<string, object> GetAdditionalSettings(IBEDefinitionSettingsGetAdditionalSettingsContext context)
        {
            var accountPackageProvider = AccountPackageProviderFactory.GetManager<IAccountPackageProvider>();
            accountPackageProvider.CastWithValidate<AccountPackageProvider>("accountPackageProvider");
            return new Dictionary<string, object>() { { "AccountPackageProvider", accountPackageProvider } };
        }

        public override Vanrise.Entities.VRLoggableEntityBase GetLoggableEntity(Vanrise.GenericData.Entities.IBusinessEntityDefinitionSettingsGetLoggableEntityContext context)
        {
            return BEManagerFactory.GetManager<IAccountBEManager>().GetAccountLoggableEntity(context.BEDefinition.BusinessEntityDefinitionId);
        }
    }

    public class AccountActionGroupDefinition
    {
        public Guid AccountActionGroupId { get; set; }
        public string Title { get; set; }
    }

    public class AccountBEClassification
    {
        public string Title { get; set; }
        public string Name { get; set; }
    }

    public class AccountBulkAction
    {
        public Guid AccountBulkActionId { get; set; }
        public string Title { get; set; }
        public AccountBulkActionSettings Settings { get; set; }
        public AccountCondition AccountCondition { get; set; }
        public RequiredPermissionSettings RequiredPermission { get; set; }
    }

    public abstract class AccountBulkActionSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string RuntimeEditor { get; }
        public virtual bool DoesUserHaveActionsAccess(IAccountBulkActionSettingsCheckAccessContext context)
        {
            return ContextFactory.GetContext().IsAllowed(context.AccountBulkAction.RequiredPermission, context.UserId);
        }
        public virtual bool DoesUserHaveStartInstanceAccess(IAccountBulkActionSettingsCheckAccessContext context)
        {
            return ContextFactory.GetContext().IsAllowed(context.AccountBulkAction.RequiredPermission, context.UserId);
        }
        public virtual bool DoesUserHaveRunInstanceAccess(IAccountBulkActionSettingsCheckAccessContext context)
        {
            return ContextFactory.GetContext().IsAllowed(context.AccountBulkAction.RequiredPermission, context.UserId);
        }
    }

    public interface IAccountBulkActionSettingsCheckAccessContext
    {
        int UserId { get; }
        AccountBulkAction AccountBulkAction { get; set; }
    }

    public class AccountBulkActionSettingsCheckAccessContext : IAccountBulkActionSettingsCheckAccessContext
    {
        public int UserId { get; set; }
        public AccountBulkAction AccountBulkAction { get; set; }
    }

    public interface IAccountBulkActionSettingsContext
    {
        Account Account { get; set; }
        Guid AccountBEDefinitionId { get; set; }
        string ErrorMessage { get; set; }
        bool IsErrorOccured { get; set; }
        AccountBulkActionSettings DefinitionSettings { get; set; }
    }

    public abstract class AccountBulkActionRuntimeSettings
    {
        public abstract void Execute(IAccountBulkActionSettingsContext context);
    }

    public class AccountExtraFieldDefinition
    {
        public Guid AccountExtraFieldDefinitionId { get; set; }

        public string Name { get; set; }

        public AccountExtraFieldDefinitionSettings Settings { get; set; }
    }

    public abstract class AccountExtraFieldDefinitionSettings
    {
        public abstract Guid ConfigId { get; }

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

        public RequiredPermissionSettings ViewFinancialAccountRequiredPermission { get; set; }
        public RequiredPermissionSettings AddFinancialAccountRequiredPermission { get; set; }
        public RequiredPermissionSettings EditFinancialAccountRequiredPermission { get; set; }
    }
}