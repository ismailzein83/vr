using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
    public class VRDashboardManager
    {
        static Guid businessEntityDefinitionId = new Guid("6243CA7F-A14C-41BE-BE48-86322D835CA6");

        #region Public Methods

        public IEnumerable<VRDashboardInfo> GetDashboardInfo(VRDashboardFilter dashboardDefinitionFilter)
        {
            List<VRDashboardInfo> vRDashboardInfos = new List<VRDashboardInfo>();
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            var vrDashboardsDefinitionInfo = genericBusinessEntityManager.GetGenericBusinessEntityInfo(businessEntityDefinitionId, null);
           
            if (vrDashboardsDefinitionInfo != null) 
            {
                Func<GenericBusinessEntityInfo, bool> filterExpression = (itm) =>
                {
                    if (dashboardDefinitionFilter != null && dashboardDefinitionFilter.DashboardDefinitionIds != null && !dashboardDefinitionFilter.DashboardDefinitionIds.Contains((Guid)itm.GenericBusinessEntityId))
                        return false;

                    return true;
                };

                IEnumerable<GenericBusinessEntityInfo> vrFilteredDashboardsDefinitionInfo = vrDashboardsDefinitionInfo.FindAllRecords(filterExpression);
                if (vrFilteredDashboardsDefinitionInfo != null)
                {
                    foreach(GenericBusinessEntityInfo dashboardDefinitionInfo in vrFilteredDashboardsDefinitionInfo) 
                    {
                        VRDashboardInfo vRDashboardInfo = new VRDashboardInfo()
                        {
                            VRDashboardId = (Guid)dashboardDefinitionInfo.GenericBusinessEntityId,
                            Name = dashboardDefinitionInfo.Name
                        };
                        vRDashboardInfos.Add(vRDashboardInfo);
                    }
                    return vRDashboardInfos;
                }
                return null;
            }
            return null;
        }

        public VRDashboard GetDashboardEntity(Guid vrDashboardId)
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            var entity = genericBusinessEntityManager.GetGenericBusinessEntity(vrDashboardId, businessEntityDefinitionId);

            if (entity != null && entity.FieldValues != null && entity.FieldValues.Count > 0)
            {
                var vrDashboardsDefinition = (VRTileReportSettings)entity.FieldValues.GetRecord("Settings");
                var vrTiles = vrDashboardsDefinition.VRTiles;

                VRDashboard vRDashboard = new VRDashboard()
                {
                    VRDashboardId = vrDashboardId,
                    Name = (string)entity.FieldValues.GetRecord("Name"),
                    Settings = (VRTileReportSettings)entity.FieldValues.GetRecord("Settings")
                };
               
                return vRDashboard;
            }
            return null;
        }

        #endregion
    }
}
