using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

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
    }

    public class VRRetailBEVisibilityAccountDefinition
    {
        public Guid AccountBEDefinitionId { get; set; }

        public List<VRRetailBEVisibilityAccountDefinitionGridColumns> GridColumns { get; set; }

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
    }

    public class VRRetailBEVisibilityAccountDefinitionView
    {
        public Guid ViewId { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionAction
    {
        public Guid ActionId { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionAccountType
    {
        public Guid AccountTypeId { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionServiceType
    {
        public Guid ServiceTypeId { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionProductDefinition
    {
        public Guid ProductDefinitionId { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionPackageDefinition
    {
        public Guid PackageDefinitionId { get; set; }
    }
}
