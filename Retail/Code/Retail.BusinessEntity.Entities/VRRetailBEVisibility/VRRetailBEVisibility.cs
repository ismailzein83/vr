using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class VRRetailBEVisibility : VRModuleVisibility
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public List<VRRetailBEVisibilityAccountDefinition> AccountDefinitions { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinition
    {
        public Guid AccountDefinitionId { get; set; }

        public string Title { get; set; }

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

        public string Title { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionAction
    {
        public Guid ActionId { get; set; }

        public string Title { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionView
    {
        public Guid ViewId { get; set; }

        public string Title { get; set; }
    }
    public class VRRetailBEVisibilityAccountDefinitionAccountType
    {
        public Guid AccountTypeId { get; set; }

        public string Title { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionServiceType
    {
        public Guid ServiceTypeId { get; set; }

        public string Title { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionProductDefinition
    {
        public Guid ServiceTypeId { get; set; }

        public string Title { get; set; }
    }

    public class VRRetailBEVisibilityAccountDefinitionPackageDefinition
    {
        public Guid PackageDefinitionId { get; set; }

        public string Title { get; set; }
    }
}
