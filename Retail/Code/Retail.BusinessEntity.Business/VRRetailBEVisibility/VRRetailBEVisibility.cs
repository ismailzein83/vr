using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class VRRetailBEVisibility : VRModuleVisibility
    {
        public static Guid s_configType = new Guid("EB85EE78-78CE-437D-B13E-18DD15EABE54");
        public override Guid ConfigId
        {
            get { return s_configType; }
        }

        public Dictionary<Guid, VRRetailBEVisibilityAccountDefinition> AccountDefinitions { get; set; }

        public override void GenerateScript(IVRModuleVisibilityGenerateScriptContext context)
        {
            if (this.AccountDefinitions != null)
            {
                AddAccountBEDefinitionsScript(context);
                AddAccountTypesScript(context);
                AddServiceTypesScript(context);
                AddComponentTypesScript(context);
            }
        }

        public override VRModuleVisibilityEditorRuntime GetEditorRuntime()
        {
            var accountBEDefinitionNamesById = new Dictionary<Guid, string>();
            var businessEntityDefinitionManager = new Vanrise.GenericData.Business.BusinessEntityDefinitionManager();

            if (AccountDefinitions != null)
            {
                foreach (var accountDefinitionsVisibility in AccountDefinitions)
                {
                    if (!accountBEDefinitionNamesById.ContainsKey(accountDefinitionsVisibility.Key))
                        accountBEDefinitionNamesById.Add(accountDefinitionsVisibility.Key, businessEntityDefinitionManager.GetBusinessEntityDefinitionName(accountDefinitionsVisibility.Key));
                }
            }

            VRRetailBEVisibilityEditorRuntime retailBEVisibilityEditorRuntime = new VRRetailBEVisibilityEditorRuntime();
            retailBEVisibilityEditorRuntime.AccountBEDefinitionNamesById = accountBEDefinitionNamesById;

            return retailBEVisibilityEditorRuntime;
        }

        #region Private Methods

        private void AddAccountBEDefinitionsScript(IVRModuleVisibilityGenerateScriptContext context)
        {
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
            StringBuilder accountBEDefinitionScriptBuilder = new StringBuilder();
            foreach (var accountDefinitionEntry in this.AccountDefinitions)
            {
                var businessEntityDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(accountDefinitionEntry.Key);
                businessEntityDefinition.ThrowIfNull("businessEntityDefinition", accountDefinitionEntry.Key);
                AccountBEDefinitionSettings accountDefSettingsWithVisib = ApplyVisibilityToAccountBEDefinition(businessEntityDefinition, accountDefinitionEntry.Value);
                if (accountBEDefinitionScriptBuilder.Length > 0)
                {
                    accountBEDefinitionScriptBuilder.Append(",");
                    accountBEDefinitionScriptBuilder.AppendLine();
                }
                accountBEDefinitionScriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}')", businessEntityDefinition.BusinessEntityDefinitionId, businessEntityDefinition.Name, businessEntityDefinition.Title, Serializer.Serialize(accountDefSettingsWithVisib));
            }
            string accountBEDefinitionScript = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[Settings]);", accountBEDefinitionScriptBuilder);
            context.AddEntityScript("[genericdata].[BusinessEntityDefinition] - 'Account BE Definitions'", accountBEDefinitionScript);
        }

        private AccountBEDefinitionSettings ApplyVisibilityToAccountBEDefinition(BusinessEntityDefinition accountBEDefinition, VRRetailBEVisibilityAccountDefinition accountDefinitionVisibility)
        {
            AccountBEDefinitionSettings accountDefSettings = accountBEDefinition.Settings.CastWithValidate<AccountBEDefinitionSettings>("accountBEDefinition.Settings", accountBEDefinition.BusinessEntityDefinitionId);
            accountDefSettings = Serializer.Deserialize<AccountBEDefinitionSettings>(Serializer.Serialize(accountDefSettings));

            //GridColumns
            if (accountDefinitionVisibility.GridColumns != null && accountDefSettings.GridDefinition != null && accountDefSettings.GridDefinition.ColumnDefinitions != null)
            {
                for (int i = accountDefSettings.GridDefinition.ColumnDefinitions.Count - 1; i >= 0; i--)
                {
                    var gridColumn = accountDefSettings.GridDefinition.ColumnDefinitions[i];
                    VRRetailBEVisibilityAccountDefinitionGridColumns columnVisibility = accountDefinitionVisibility.GridColumns.FindRecord(itm => itm.FieldName == gridColumn.FieldName);
                    if (columnVisibility != null)
                    {
                        if (!string.IsNullOrEmpty(columnVisibility.Header))
                            gridColumn.Header = columnVisibility.Header;
                        if(columnVisibility.SpecialAvailabilitySettings)
                        {
                            gridColumn.IsAvailableInRoot = columnVisibility.IsAvailableInRoot;
                            gridColumn.IsAvailableInSubAccounts = columnVisibility.IsAvailableInSubAccounts;
                            gridColumn.SubAccountsAvailabilityCondition = columnVisibility.SubAccountsAvailabilityCondition;
                        }
                    }
                    else
                    {
                        accountDefSettings.GridDefinition.ColumnDefinitions.RemoveAt(i);
                    }
                }
            }

            //Views
            if (accountDefinitionVisibility.Views != null && accountDefSettings.AccountViewDefinitions != null)
            {
                for (int i = accountDefSettings.AccountViewDefinitions.Count - 1; i >= 0; i--)
                {
                    var view = accountDefSettings.AccountViewDefinitions[i];
                    VRRetailBEVisibilityAccountDefinitionView viewVisibility = accountDefinitionVisibility.Views.FindRecord(itm => itm.ViewId == view.AccountViewDefinitionId);
                    if (viewVisibility != null)
                    {
                        if (viewVisibility.SpecialCondition)
                            view.AvailabilityCondition = viewVisibility.AvailabilityCondition;
                        if (viewVisibility.SpecialDrillDownSectionName)
                            view.DrillDownSectionName = viewVisibility.DrillDownSectionName;
                        if (viewVisibility.SpecialAccount360DegreeSectionName)
                            view.Account360DegreeSectionName = viewVisibility.Account360DegreeSectionName;
                    }
                    else
                    {
                        accountDefSettings.AccountViewDefinitions.RemoveAt(i);
                    }
                }
            }

            //Actions
            if (accountDefinitionVisibility.Actions != null && accountDefSettings.ActionDefinitions != null)
            {
                for (int i = accountDefSettings.ActionDefinitions.Count - 1; i >= 0; i--)
                {
                    var action = accountDefSettings.ActionDefinitions[i];
                    VRRetailBEVisibilityAccountDefinitionAction actionVisibility = accountDefinitionVisibility.Actions.FindRecord(itm => itm.ActionId == action.AccountActionDefinitionId);
                    if (actionVisibility != null)
                    {
                        if (!String.IsNullOrEmpty(actionVisibility.Title))
                            action.Name = actionVisibility.Title;
                        if (actionVisibility.SpecialCondition)
                            action.AvailabilityCondition = actionVisibility.AvailabilityCondition;
                    }
                    else
                    {
                        accountDefSettings.ActionDefinitions.RemoveAt(i);
                    }
                }
            }

            return accountDefSettings;
        }

        private void AddAccountTypesScript(IVRModuleVisibilityGenerateScriptContext context)
        {
            StringBuilder accountTypesScriptBuilder = new StringBuilder();
            var visibleAccountTypes = GetVisibleAccountTypes();
            foreach (var accountType in visibleAccountTypes)
            {
                if (accountTypesScriptBuilder.Length > 0)
                {
                    accountTypesScriptBuilder.Append(",");
                    accountTypesScriptBuilder.AppendLine();
                }
                accountTypesScriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}','{4}')", accountType.AccountTypeId, accountType.Name, accountType.Title, accountType.AccountBEDefinitionId, Serializer.Serialize(accountType.Settings));
            }
            if (accountTypesScriptBuilder.Length > 0)
            {
                string accountTypesScript = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Title],[AccountBEDefinitionID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[AccountBEDefinitionID],[Settings]))
merge	[Retail_BE].[AccountType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[AccountBEDefinitionID] = s.[AccountBEDefinitionID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[AccountBEDefinitionID],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[AccountBEDefinitionID],s.[Settings]);", accountTypesScriptBuilder);
                context.AddEntityScript("[Retail_BE].[AccountType]", accountTypesScript);
            }
            StringBuilder accountPartDefinitionsScriptBuilder = new StringBuilder();
            Dictionary<Guid, AccountPartDefinition> allAccountPartDefinitions = new AccountPartDefinitionManager().GetCachedAccountPartDefinitions();
            HashSet<Guid> addedParts = new HashSet<Guid>();
            foreach(var accountType in visibleAccountTypes)
            {
                if(accountType.Settings != null && accountType.Settings.PartDefinitionSettings != null)
                {
                    foreach(var accountTypePart in accountType.Settings.PartDefinitionSettings)
                    {
                        if(!addedParts.Contains(accountTypePart.PartDefinitionId))
                        {
                            AccountPartDefinition partDefinition;
                            allAccountPartDefinitions.TryGetValue(accountTypePart.PartDefinitionId, out partDefinition);
                            partDefinition.ThrowIfNull("partDefinition", accountTypePart.PartDefinitionId);
                            if (accountPartDefinitionsScriptBuilder.Length > 0)
                            {
                                accountPartDefinitionsScriptBuilder.Append(",");
                                accountPartDefinitionsScriptBuilder.AppendLine();
                            }
                            accountPartDefinitionsScriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}')", partDefinition.AccountPartDefinitionId, partDefinition.Title, partDefinition.Name, Serializer.Serialize(partDefinition.Settings));
                        }
                    }
                }
            }
            if(accountPartDefinitionsScriptBuilder.Length > 0)
            {
                string accountPartDefinitionsScript = String.Format(@"set nocount on;
;with cte_data([ID],[Title],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Title],[Name],[Details]))
merge	[Retail_BE].[AccountPartDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Title] = s.[Title],[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Title],[Name],[Details])
	values(s.[ID],s.[Title],s.[Name],s.[Details]);", accountPartDefinitionsScriptBuilder);
                context.AddEntityScript("[Retail_BE].[AccountPartDefinition]", accountPartDefinitionsScript);
            }
        }

        private List<AccountType> GetVisibleAccountTypes()
        {
            List<AccountType> visibleAccountTypes = new List<AccountType>();
            var allAccountTypes = new AccountTypeManager().GetCachedAccountTypesWithHidden();
            if (allAccountTypes != null)
            {
                foreach (var accountType in allAccountTypes.Values)
                {
                    VRRetailBEVisibilityAccountDefinition accountDefinitionVisibility;
                    if (this.AccountDefinitions.TryGetValue(accountType.AccountBEDefinitionId, out accountDefinitionVisibility))
                    {
                        if (accountDefinitionVisibility.AccountTypes == null)
                        {
                            visibleAccountTypes.Add(Serializer.Deserialize<AccountType>(Serializer.Serialize(accountType)));
                        }
                        else
                        {
                            VRRetailBEVisibilityAccountDefinitionAccountType accountTypeVisibility = accountDefinitionVisibility.AccountTypes.FindRecord(itm => itm.AccountTypeId == accountType.AccountTypeId);
                            if (accountTypeVisibility != null)
                            {
                                AccountType visibleAccountType = Serializer.Deserialize<AccountType>(Serializer.Serialize(accountType));
                                if (!String.IsNullOrEmpty(accountTypeVisibility.Title))
                                    visibleAccountType.Title = accountTypeVisibility.Title;
                                if(accountTypeVisibility.Parts != null)
                                {
                                    for (int i = visibleAccountType.Settings.PartDefinitionSettings.Count - 1; i >= 0; i--)
                                    {
                                        var partDefinition = visibleAccountType.Settings.PartDefinitionSettings[i];
                                        var partDefinitionVisibility = accountTypeVisibility.Parts.FindRecord(itm => itm.PartDefinitionId == partDefinition.PartDefinitionId);
                                        if (partDefinitionVisibility == null)
                                            visibleAccountType.Settings.PartDefinitionSettings.RemoveAt(i);                                        
                                    }
                                }
                                visibleAccountTypes.Add(visibleAccountType);
                            }
                        }
                    }
                }
            }
            return visibleAccountTypes;
        }

        private void AddServiceTypesScript(IVRModuleVisibilityGenerateScriptContext context)
        {
            StringBuilder serviceTypesScriptBuilder = new StringBuilder();
            var visibleServiceTypes = GetVisibleServiceTypes();
            foreach (var serviceType in visibleServiceTypes)
            {
                if (serviceTypesScriptBuilder.Length > 0)
                {
                    serviceTypesScriptBuilder.Append(",");
                    serviceTypesScriptBuilder.AppendLine();
                }
                serviceTypesScriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}','{4}')", serviceType.ServiceTypeId, serviceType.Name, serviceType.Title, serviceType.AccountBEDefinitionId, Serializer.Serialize(serviceType.Settings));
            }
            if (serviceTypesScriptBuilder.Length > 0)
            {
                string serviceTypesScript = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Title],[AccountBEDefinitionId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[AccountBEDefinitionId],[Settings]))
merge	[Retail].[ServiceType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[AccountBEDefinitionId] = s.[AccountBEDefinitionId],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[AccountBEDefinitionId],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[AccountBEDefinitionId],s.[Settings]);", serviceTypesScriptBuilder);
                context.AddEntityScript("[Retail].[ServiceType]", serviceTypesScript);
            }
        }

        private List<ServiceType> GetVisibleServiceTypes()
        {
            List<ServiceType> visibleServiceTypes = new List<ServiceType>();
            var allServiceTypes = new ServiceTypeManager().GetCachedServiceTypesWithHidden();
            if (allServiceTypes != null)
            {
                foreach (var serviceType in allServiceTypes.Values)
                {
                    VRRetailBEVisibilityAccountDefinition accountDefinitionVisibility;
                    if (this.AccountDefinitions.TryGetValue(serviceType.AccountBEDefinitionId, out accountDefinitionVisibility))
                    {
                        if (accountDefinitionVisibility.ServiceTypes == null)
                        {
                            visibleServiceTypes.Add(Serializer.Deserialize<ServiceType>(Serializer.Serialize(serviceType)));
                        }
                        else
                        {
                            VRRetailBEVisibilityAccountDefinitionServiceType serviceTypeVisibility = accountDefinitionVisibility.ServiceTypes.FindRecord(itm => itm.ServiceTypeId == serviceType.ServiceTypeId);
                            if (serviceTypeVisibility != null)
                            {
                                ServiceType visibileServiceType = Serializer.Deserialize<ServiceType>(Serializer.Serialize(serviceType));
                                if (!String.IsNullOrEmpty(serviceTypeVisibility.Title))
                                    visibileServiceType.Title = serviceTypeVisibility.Title;
                                visibleServiceTypes.Add(visibileServiceType);
                            }
                        }
                    }
                }
            }
            return visibleServiceTypes;
        }

        private void AddComponentTypesScript(IVRModuleVisibilityGenerateScriptContext context)
        {
            List<VRComponentType> componentTypes = new List<VRComponentType>();
            componentTypes.AddRange(GetVisibleProductDefinitions());
            componentTypes.AddRange(GetVisiblePackageDefinitions());
            StringBuilder componentTypesScriptBuilder = new StringBuilder();
            foreach (var componentType in componentTypes)
            {
                if (componentTypesScriptBuilder.Length > 0)
                {
                    componentTypesScriptBuilder.Append(",");
                    componentTypesScriptBuilder.AppendLine();
                }
                componentTypesScriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}')", componentType.VRComponentTypeId, componentType.Name, componentType.Settings.VRComponentTypeConfigId, Serializer.Serialize(componentType.Settings));
            }
            if (componentTypesScriptBuilder.Length > 0)
            {
                string componentTypesScript = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[ConfigID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ConfigID],[Settings]))
merge	[common].[VRComponentType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ConfigID] = s.[ConfigID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ConfigID],[Settings])
	values(s.[ID],s.[Name],s.[ConfigID],s.[Settings]);", componentTypesScriptBuilder);
                context.AddEntityScript("[common].[VRComponentType]", componentTypesScript);
            }
        }

        private List<VRComponentType> GetVisibleProductDefinitions()
        {
            List<ProductDefinition> visibleProductDefinitions = new List<ProductDefinition>();
            var allProductDefinitions = new ProductDefinitionManager().GetCachedProductDefinitionsWithHidden();
            if (allProductDefinitions != null)
            {
                foreach (var productDefinition in allProductDefinitions.Values)
                {
                    productDefinition.Settings.ThrowIfNull("productDefinition.Settings", productDefinition.VRComponentTypeId);
                    VRRetailBEVisibilityAccountDefinition accountDefinitionVisibility;
                    if (this.AccountDefinitions.TryGetValue(productDefinition.Settings.AccountBEDefinitionId, out accountDefinitionVisibility))
                    {
                        if (accountDefinitionVisibility.ProductDefinitions == null)
                        {
                            visibleProductDefinitions.Add(Serializer.Deserialize<ProductDefinition>(Serializer.Serialize(productDefinition)));
                        }
                        else
                        {
                            VRRetailBEVisibilityAccountDefinitionProductDefinition productDefinitionVisibility = accountDefinitionVisibility.ProductDefinitions.FindRecord(itm => itm.ProductDefinitionId == productDefinition.VRComponentTypeId);
                            if (productDefinitionVisibility != null)
                            {
                                ProductDefinition visibileProductDefinition = Serializer.Deserialize<ProductDefinition>(Serializer.Serialize(productDefinition));
                                if (!String.IsNullOrEmpty(productDefinitionVisibility.Title))
                                    visibileProductDefinition.Name = productDefinitionVisibility.Title;
                                visibleProductDefinitions.Add(visibileProductDefinition);
                            }
                        }
                    }
                }
            }
            return visibleProductDefinitions.Select(itm => new VRComponentType { VRComponentTypeId = itm.VRComponentTypeId, Name = itm.Name, Settings = itm.Settings }).ToList();
        }

        private List<VRComponentType> GetVisiblePackageDefinitions()
        {
            List<PackageDefinition> visiblePackageDefinitions = new List<PackageDefinition>();
            var allPackageDefinitions = new PackageDefinitionManager().GetCachedPackageDefinitionswithHidden();
            if (allPackageDefinitions != null)
            {
                foreach (var packageDefinition in allPackageDefinitions.Values)
                {
                    packageDefinition.Settings.ThrowIfNull("packageDefinition.Settings", packageDefinition.VRComponentTypeId);
                    VRRetailBEVisibilityAccountDefinition accountDefinitionVisibility;
                    if (this.AccountDefinitions.TryGetValue(packageDefinition.Settings.AccountBEDefinitionId, out accountDefinitionVisibility))
                    {
                        if (accountDefinitionVisibility.PackageDefinitions == null)
                        {
                            visiblePackageDefinitions.Add(Serializer.Deserialize<PackageDefinition>(Serializer.Serialize(packageDefinition)));
                        }
                        else
                        {
                            VRRetailBEVisibilityAccountDefinitionPackageDefinition packageDefinitionVisibility = accountDefinitionVisibility.PackageDefinitions.FindRecord(itm => itm.PackageDefinitionId == packageDefinition.VRComponentTypeId);
                            if (packageDefinitionVisibility != null)
                            {
                                PackageDefinition visibilePackageDefinition = Serializer.Deserialize<PackageDefinition>(Serializer.Serialize(packageDefinition));
                                if (!String.IsNullOrEmpty(packageDefinitionVisibility.Title))
                                    visibilePackageDefinition.Name = packageDefinitionVisibility.Title;
                                visiblePackageDefinitions.Add(visibilePackageDefinition);
                            }
                        }
                    }
                }
            }
            return visiblePackageDefinitions.Select(itm => new VRComponentType { VRComponentTypeId = itm.VRComponentTypeId, Name = itm.Name, Settings = itm.Settings }).ToList();
        }

        #endregion
    }

    public class VRRetailBEVisibilityAccountDefinition
    {
        public Guid AccountBEDefinitionId { get; set; }

        public List<VRRetailBEVisibilityAccountDefinitionGridColumns> GridColumns { get; set; }

        public List<VRRetailBEVisibilityAccountDefinitionGridExportColumn> GridExportColumns { get; set; }

        public List<VRRetailBEVisibilityAccountDefinitionExtraField> ExtraFields { get; set; }

        public List<VRRetailBEVisibilityAccountDefinitionView> Views { get; set; }

        public List<VRRetailBEVisibilityAccountDefinitionAction> Actions { get; set; }

        public List<VRRetailBEVisibilityAccountDefinitionAccountType> AccountTypes { get; set; }

        public List<VRRetailBEVisibilityAccountDefinitionServiceType> ServiceTypes { get; set; }

        public List<VRRetailBEVisibilityAccountDefinitionProductDefinition> ProductDefinitions { get; set; }

        public List<VRRetailBEVisibilityAccountDefinitionPackageDefinition> PackageDefinitions { get; set; }
    }


    public class VRRetailBEVisibilityAccountDefinitionGridColumns
    {
        public string FieldName { get; set; }

        public string Header { get; set; }

        public bool SpecialAvailabilitySettings { get; set; }

        public bool IsAvailableInRoot { get; set; }

        public bool IsAvailableInSubAccounts { get; set; }

        public AccountCondition SubAccountsAvailabilityCondition { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionGridExportColumn
    {
        public string FieldName { get; set; }

        public string Header { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionExtraField
    {
        public string ExtraFieldId { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionView
    {
        public Guid ViewId { get; set; }

        public bool SpecialCondition { get; set; }

        public AccountCondition AvailabilityCondition { get; set; }

        public bool SpecialDrillDownSectionName { get; set; }
                
        public string DrillDownSectionName { get; set; }

        public bool SpecialAccount360DegreeSectionName { get; set; }

        public string Account360DegreeSectionName { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionAction
    {
        public Guid ActionId { get; set; }

        public string Title { get; set; }

        public bool SpecialCondition { get; set; }

        public AccountCondition AvailabilityCondition { get; set; }

    }

    public class VRRetailBEVisibilityAccountDefinitionAccountType
    {
        public Guid AccountTypeId { get; set; }

        public string Title { get; set; }

        public List<VRRetailBEVisibilityAccountDefinitionAccountTypePart> Parts { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionAccountTypePart
    {
        public Guid PartDefinitionId { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionServiceType
    {
        public Guid ServiceTypeId { get; set; }

        public string Title { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionProductDefinition
    {
        public Guid ProductDefinitionId { get; set; }

        public string Title { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionPackageDefinition
    {
        public Guid PackageDefinitionId { get; set; }

        public string Title { get; set; }
    }
}
