using System;
using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Security.Business;

namespace Vanrise.GenericData.Business
{
    public class BEParentChildRelationDefinitionManager
    {
        #region Ctor/Properties

        static SecurityManager s_securityManager = new SecurityManager();

        #endregion

        #region Public Methods

        public BEParentChildRelationDefinition GetBEParentChildRelationDefinition(Guid beParentChildRelationDefinitionId)
        {
            var packageDefinitions = this.GetCachedBEParentChildRelationDefinitions();
            return packageDefinitions.FindRecord(x => x.VRComponentTypeId == beParentChildRelationDefinitionId);
        }

        public IEnumerable<BEParentChildRelationDefinitionInfo> GetBEParentChildRelationDefinitionsInfo(BEParentChildRelationDefinitionFilter filter)
        {
            Func<BEParentChildRelationDefinition, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (beParentChildRelationDefinition) =>
                    {
                        if (filter.ParentBEDefinitionId != Guid.Empty && filter.ParentBEDefinitionId != beParentChildRelationDefinition.Settings.ParentBEDefinitionId)
                            return false;

                        if (filter.ChildBEDefinitionId != Guid.Empty && filter.ChildBEDefinitionId != beParentChildRelationDefinition.Settings.ChildBEDefinitionId)
                            return false;

                        return true;
                    };
            }

            var beParentChildRelationDefinitions = this.GetCachedBEParentChildRelationDefinitions();
            return beParentChildRelationDefinitions.MapRecords(BEParentChildRelationDefinitionInfoMapper, filterExpression);
        }

        public IEnumerable<string> GetBEParentChildRelationGridColumnNames(Guid beParentChildRelationDefinitionId)
        {
            List<string> beParentChildRelationGridColumnNames = new List<string>();
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();

            BEParentChildRelationDefinition beParentChildRelationDefinition = this.GetBEParentChildRelationDefinition(beParentChildRelationDefinitionId);
            if (beParentChildRelationDefinition == null)
                throw new NullReferenceException(string.Format("beParentChildRelationDefinition {0}", beParentChildRelationDefinitionId));

            if (beParentChildRelationDefinition.Settings == null)
                throw new NullReferenceException(string.Format("beParentChildRelationDefinition.Settings {0}", beParentChildRelationDefinitionId));

            beParentChildRelationGridColumnNames.Add(businessEntityDefinitionManager.GetBusinessEntityDefinitionName(beParentChildRelationDefinition.Settings.ParentBEDefinitionId));
            beParentChildRelationGridColumnNames.Add(businessEntityDefinitionManager.GetBusinessEntityDefinitionName(beParentChildRelationDefinition.Settings.ChildBEDefinitionId));

            return beParentChildRelationGridColumnNames;
        }

        #endregion

        #region Private Methods

        private Dictionary<Guid, BEParentChildRelationDefinition> GetCachedBEParentChildRelationDefinitions()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetCachedComponentTypes<BEParentChildRelationDefinitionSettings, BEParentChildRelationDefinition>();
        }

        private bool DoesUserHaveAccess(int userId, Guid beParentChildRelationDefinitionId, Func<BEParentChildRelationDefinitionSecurity, Vanrise.Security.Entities.RequiredPermissionSettings> getRequiredPermissionSetting)
        {
            var beParentChildRelationDefinition = GetBEParentChildRelationDefinition(beParentChildRelationDefinitionId);
            if (beParentChildRelationDefinition != null && beParentChildRelationDefinition.Settings != null && beParentChildRelationDefinition.Settings.Security != null && getRequiredPermissionSetting(beParentChildRelationDefinition.Settings.Security) != null)
                return s_securityManager.IsAllowed(getRequiredPermissionSetting(beParentChildRelationDefinition.Settings.Security), userId);
            else
                return true;
        }

        #endregion

        #region Mapper

        public BEParentChildRelationDefinitionInfo BEParentChildRelationDefinitionInfoMapper(BEParentChildRelationDefinition beParentChildRelationDefinition)
        {
            return new BEParentChildRelationDefinitionInfo
            {
                Name = beParentChildRelationDefinition.Name,
                BEParentChildRelationDefinitionId = beParentChildRelationDefinition.VRComponentTypeId,
                ParentBEDefinitionId = beParentChildRelationDefinition.Settings.ParentBEDefinitionId,
                ChildBEDefinitionId = beParentChildRelationDefinition.Settings.ChildBEDefinitionId
            };
        }

        #endregion

        #region Security

        public bool DoesUserHaveViewAccess(Guid beParentChildRelationDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAccess(userId, beParentChildRelationDefinitionId, (sec) => sec.ViewRequiredPermission);
        }
        public bool DoesUserHaveAddAccess(Guid beParentChildRelationDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAccess(userId, beParentChildRelationDefinitionId, (sec) => sec.AddRequiredPermission);
        }

        #endregion
    }
}
