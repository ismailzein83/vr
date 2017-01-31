using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
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

        public Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionView> GetVisibleViews(VRRetailBEVisibility retailBEVisibility)
        {
            var visibleViews = new List<VRRetailBEVisibilityAccountDefinitionView>();

            if (retailBEVisibility != null && retailBEVisibility.AccountDefinitions != null)
            {
                foreach (var accountDefinition in retailBEVisibility.AccountDefinitions.Values)
                {
                    if (accountDefinition.GridColumns != null && accountDefinition.GridColumns.Count > 0)
                        visibleViews.AddRange(accountDefinition.Views);
                }
            }
            return visibleViews.ToDictionary(itm => itm.ViewId, itm => itm);
        }

        public Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionAction> GetVisibleActions(VRRetailBEVisibility retailBEVisibility)
        {
            var visibleActions = new List<VRRetailBEVisibilityAccountDefinitionAction>();

            if (retailBEVisibility != null && retailBEVisibility.AccountDefinitions != null)
            {
                foreach (var accountDefinition in retailBEVisibility.AccountDefinitions.Values)
                {
                    if (accountDefinition.GridColumns != null && accountDefinition.GridColumns.Count > 0)
                        visibleActions.AddRange(accountDefinition.Actions);
                }
            }
            return visibleActions.ToDictionary(itm => itm.ActionId, itm => itm);
        }
    }
}
