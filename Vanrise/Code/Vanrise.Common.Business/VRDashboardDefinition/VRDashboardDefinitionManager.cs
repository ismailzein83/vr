using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
    public class VRDashboardDefinitionManager
    {
        static Guid _dashboardDefinitionBEId = new Guid("6243CA7F-A14C-41BE-BE48-86322D835CA6");

        #region Public Methods
        public IEnumerable<VRDashboardInfo> GetDashboardInfo(VRDashboardFilter filter)
        {
            var dashboardDefinitions = GetCachedVRDashboardDefinitions();
            Func<VRDashboard, bool> filterExpression = (itm) =>
                {
                    if (filter != null && filter.DashboardDefinitionIds != null && !filter.DashboardDefinitionIds.Contains((Guid)itm.VRDashboardId))
                        return false;

                    return true;
                };
            return dashboardDefinitions.MapRecords(DashboardInfoMapper, filterExpression);
        }
        public VRDashboard GetDashboardEntity(Guid vrDashboardId)
        {
            return GetCachedVRDashboardDefinitions().GetRecord(vrDashboardId);
        }

        #endregion

        #region Private Methods
        private Dictionary<Guid, VRDashboard> GetCachedVRDashboardDefinitions()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedVRDashboardDefinitions", _dashboardDefinitionBEId, () =>
            {
                Dictionary<Guid, VRDashboard> result = new Dictionary<Guid, VRDashboard>();
                IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_dashboardDefinitionBEId, null);
                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        VRDashboard vRDashboardDefinition = new VRDashboard()
                        {
                            VRDashboardId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = genericBusinessEntity.FieldValues.GetRecord("Name") as string,
                            Settings = genericBusinessEntity.FieldValues.GetRecord("Settings") as VRDashboardSettings
                        };
                        result.Add(vRDashboardDefinition.VRDashboardId, vRDashboardDefinition);
                    }
                }
                return result;
            });
        }
        #endregion

        #region Mappers
        private VRDashboardInfo DashboardInfoMapper(VRDashboard dashboardDefinition)
        {
            return new VRDashboardInfo()
            {
                VRDashboardId = dashboardDefinition.VRDashboardId,
                Name = dashboardDefinition.Name
            };
        }
        #endregion
    }
}

