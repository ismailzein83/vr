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
        public IEnumerable<VRDashboardDefinitionInfo> GetDashboardInfo(VRDashboardDefinitionFilter filter)
        {
            var dashboardDefinitions = GetCachedVRDashboardDefinitions();
            Func<VRDashboardDefinition, bool> filterExpression = (itm) =>
                {
                    if (filter != null && filter.DashboardDefinitionIds != null && !filter.DashboardDefinitionIds.Contains((Guid)itm.VRDashboardDefinitionId))
                        return false;

                    return true;
                };
            return dashboardDefinitions.MapRecords(DashboardInfoMapper, filterExpression);
        }
        public VRDashboardDefinition GetDashboardEntity(Guid vrDashboardDefinitionId)
        {
            return GetCachedVRDashboardDefinitions().GetRecord(vrDashboardDefinitionId);
        }

        #endregion

        #region Private Methods
        private Dictionary<Guid, VRDashboardDefinition> GetCachedVRDashboardDefinitions()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedVRDashboardDefinitions", _dashboardDefinitionBEId, () =>
            {
                Dictionary<Guid, VRDashboardDefinition> result = new Dictionary<Guid, VRDashboardDefinition>();
                IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_dashboardDefinitionBEId, null);
                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        VRDashboardDefinition vRDashboardDefinition = new VRDashboardDefinition()
                        {
                            VRDashboardDefinitionId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = genericBusinessEntity.FieldValues.GetRecord("Name") as string,
                            Settings = genericBusinessEntity.FieldValues.GetRecord("Settings") as VRDashboardSettings
                        };
                        result.Add(vRDashboardDefinition.VRDashboardDefinitionId, vRDashboardDefinition);
                    }
                }
                return result;
            });
        }
        #endregion

        #region Mappers
        private VRDashboardDefinitionInfo DashboardInfoMapper(VRDashboardDefinition dashboardDefinition)
        {
            return new VRDashboardDefinitionInfo()
            {
                VRDashboardDefinitionId = dashboardDefinition.VRDashboardDefinitionId,
                Name = dashboardDefinition.Name
            };
        }
        #endregion
    }
}

