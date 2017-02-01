using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class VRRetailBEVisibilityManager
    {
        public VRRetailBEVisibility GetRetailBEVisibility()
        {
            return new VRApplicationVisibilityManager().GetModuleVisibility<VRRetailBEVisibility>();
        }

        public bool ShouldApplyGridColumnsVisibility(out Dictionary<string, VRRetailBEVisibilityAccountDefinitionGridColumns> gridColumnsByFieldName)
        {
            gridColumnsByFieldName = new Dictionary<string, VRRetailBEVisibilityAccountDefinitionGridColumns>();

            var retailBEVisibility = this.GetRetailBEVisibility();
            if (retailBEVisibility == null)
                return false;

            if (retailBEVisibility != null && retailBEVisibility.AccountDefinitions != null)
            {
                foreach (var accountDefinition in retailBEVisibility.AccountDefinitions.Values)
                {
                    if (accountDefinition.GridColumns != null)
                    {
                        foreach (var gridColumn in accountDefinition.GridColumns)
                        {
                            if (!gridColumnsByFieldName.ContainsKey(gridColumn.FieldName))
                                gridColumnsByFieldName.Add(gridColumn.FieldName, gridColumn);
                        }
                    }
                }
            }
            return true;
        }

        public bool ShouldApplyViewsVisibility(out Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionView> viewsById)
        {
            viewsById = new Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionView>();

            var retailBEVisibility = this.GetRetailBEVisibility();
            if (retailBEVisibility == null)
                return false;

            if (retailBEVisibility != null && retailBEVisibility.AccountDefinitions != null)
            {
                foreach (var accountDefinition in retailBEVisibility.AccountDefinitions.Values)
                {
                    if (accountDefinition.Views != null)
                    {
                        foreach (var view in accountDefinition.Views)
                        {
                            if (!viewsById.ContainsKey(view.ViewId))
                                viewsById.Add(view.ViewId, view);
                        }
                    }
                }
            }
            return true;
        }

        public bool ShouldApplyActionsVisibility(out Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionAction> actionsById)
        {
            actionsById = new Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionAction>();

            var retailBEVisibility = this.GetRetailBEVisibility();
            if (retailBEVisibility == null)
                return false;

            if (retailBEVisibility != null && retailBEVisibility.AccountDefinitions != null)
            {
                foreach (var accountDefinition in retailBEVisibility.AccountDefinitions.Values)
                {
                    if (accountDefinition.Actions != null)
                    {
                        foreach (var action in accountDefinition.Actions)
                        {
                            if (!actionsById.ContainsKey(action.ActionId))
                                actionsById.Add(action.ActionId, action);
                        }
                    }
                }
            }
            return true;
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
        public bool ShouldApplyAccountTypesVisibility(out Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionAccountType> accountTypesById)
        {
            accountTypesById = new Dictionary<Guid,VRRetailBEVisibilityAccountDefinitionAccountType>();
            
            var retailBEVisibility = this.GetRetailBEVisibility();
            if (retailBEVisibility == null)
                return false;

            if (retailBEVisibility != null && retailBEVisibility.AccountDefinitions != null)
            {
                foreach (var accountDefinition in retailBEVisibility.AccountDefinitions.Values)
                {
                    if (accountDefinition.AccountTypes != null)
                    {
                        foreach (var accountType in accountDefinition.AccountTypes)
                        {
                            if (!accountTypesById.ContainsKey(accountType.AccountTypeId))
                                accountTypesById.Add(accountType.AccountTypeId, accountType);
                        }
                    }
                }
            }
            return true;
        }
    }
}
