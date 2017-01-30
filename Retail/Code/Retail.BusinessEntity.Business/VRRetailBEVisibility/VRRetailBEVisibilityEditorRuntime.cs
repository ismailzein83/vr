using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class VRRetailBEVisibilityEditorRuntime : Vanrise.Entities.VRModuleVisibilityEditorRuntime
    {
        public Dictionary<Guid, string> AccountBEDefinitionNamesById { get; set; }

        public Dictionary<Guid, string> AccountTypeTitlesById { get; set; }
    }

    public class VRRetailBEVisibilityManager
    {
        public VRRetailBEVisibility GetRetailBEVisibility()
        {
            return new VRApplicationVisibilityManager().GetModuleVisibility<VRRetailBEVisibility>();
        }

        public Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionAccountType> GetVisibleAccountTypes(VRRetailBEVisibility retailBEVisibility)
        {
            var visibleAccountTypes = new List<VRRetailBEVisibilityAccountDefinitionAccountType>();

            if (retailBEVisibility != null && retailBEVisibility.AccountDefinitions != null)
            {
                foreach (var accountDefinition in retailBEVisibility.AccountDefinitions.Values)
                {
                    if (accountDefinition.AccountTypes != null && accountDefinition.AccountTypes.Count > 0)
                        visibleAccountTypes.AddRange(accountDefinition.AccountTypes);
                }
            }

            return visibleAccountTypes.ToDictionary(itm => itm.AccountTypeId, itm => itm);
        }

        public Dictionary<string, VRRetailBEVisibilityAccountDefinitionGridColumns> GetVisibleGridColumns(VRRetailBEVisibility retailBEVisibility)
        {
            var visibleGridColumns = new List<VRRetailBEVisibilityAccountDefinitionGridColumns>();

            if (retailBEVisibility != null && retailBEVisibility.AccountDefinitions != null)
            {
                foreach (var accountDefinition in retailBEVisibility.AccountDefinitions.Values)
                {
                    if (accountDefinition.GridColumns != null && accountDefinition.GridColumns.Count > 0)
                        visibleGridColumns.AddRange(accountDefinition.GridColumns);
                }
            }
            return visibleGridColumns.ToDictionary(itm => itm.FieldName, itm => itm);
        }
    }
}
