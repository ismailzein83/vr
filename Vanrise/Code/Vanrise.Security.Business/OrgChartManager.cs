using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Caching;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
using Vanrise.Common.Business;

namespace Vanrise.Security.Business
{
    public class OrgChartManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<OrgChart> GetFilteredOrgCharts(Vanrise.Entities.DataRetrievalInput<OrgChartQuery> input)
        {
            var cachedOrgCharts = GetCachedOrgCharts();
            Func<OrgChart, bool> filterExpression = (orgChart) => (input.Query.Name == null || orgChart.Name.ToUpper().Contains(input.Query.Name.ToUpper()));
            VRActionLogger.Current.LogGetFilteredAction(OrgChartLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedOrgCharts.Values.ToBigResult(input, filterExpression));
        }

        public IEnumerable<OrgChartInfo> GetOrgChartInfo()
        {
            var cachedOrgCharts = GetCachedOrgCharts();
            return cachedOrgCharts.Values.MapRecords(OrgChartInfoMapper);
        }

        public OrgChart GetOrgChartById(int orgChartId, bool isViewedFromUI)
        {
            var cachedOrgCharts = GetCachedOrgCharts();
            var orgChartItem= cachedOrgCharts.Values.FindRecord(orgChart => orgChart.OrgChartId == orgChartId);
            if (orgChartItem != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(OrgChartLoggableEntity.Instance, orgChartItem);
            return orgChartItem;
        }

        public OrgChart GetOrgChartById(int orgChartId)
        {
           return GetOrgChartById(orgChartId,false);
        }
        public string GetOrgChartName(OrgChart orgChart)
        {
            return (orgChart != null) ? orgChart.Name : null;
        }

        public Vanrise.Entities.InsertOperationOutput<OrgChart> AddOrgChart(OrgChart orgChartObject)
        {
            InsertOperationOutput<OrgChart> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<OrgChart>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int orgChartId = -1;

            IOrgChartDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
            bool insertActionSucc = dataManager.AddOrgChart(orgChartObject, out orgChartId);

            if (insertActionSucc)
            {
                orgChartObject.OrgChartId = orgChartId;
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                VRActionLogger.Current.TrackAndLogObjectAdded(OrgChartLoggableEntity.Instance, orgChartObject);
                insertOperationOutput.InsertedObject = orgChartObject;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<OrgChart> UpdateOrgChart(OrgChart orgChartObject)
        {
            IOrgChartDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
            bool updateActionSucc = dataManager.UpdateOrgChart(orgChartObject);

            UpdateOperationOutput<OrgChart> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<OrgChart>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                VRActionLogger.Current.TrackAndLogObjectUpdated(OrgChartLoggableEntity.Instance, orgChartObject);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = orgChartObject;
            }
            else
            {
                
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<object> DeleteOrgChart(int orgChartId)
        {
            var orgChart = GetOrgChartById(orgChartId);
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();

            deleteOperationOutput.Result = DeleteOperationResult.Failed;

            int? linkdOrgChartId = GetLinkedOrgChartId(new OrgChartLinkedEntityInstance());
            // make sure that the org chart isn't linked to an entity
            if (linkdOrgChartId == null || linkdOrgChartId != orgChartId)
            {
                IOrgChartDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
                bool updateActionSucc = dataManager.DeleteOrgChart(orgChartId);

                if (updateActionSucc)
                {
                    VRActionLogger.Current.TrackAndLogObjectDeleted(OrgChartLoggableEntity.Instance, orgChart);
                    deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
                }
            }
            else
            {
                deleteOperationOutput.Result = DeleteOperationResult.InUse;
            }

            return deleteOperationOutput;
        }

        public int? GetLinkedOrgChartId(OrgChartLinkedEntityInfo entityInfo)
        {
            IOrgChartLinkedEntityDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartLinkedEntityDataManager>();
            return dataManager.GetLinkedOrgChartId(entityInfo.GetIdentifier());
        }

        public Vanrise.Entities.UpdateOperationOutput<object> UpdateOrgChartLinkedEntity(int orgChartId, OrgChartLinkedEntityInfo entityInfo)
        {
            IOrgChartLinkedEntityDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartLinkedEntityDataManager>();

            bool updateActionSucc = dataManager.InsertOrUpdate(orgChartId, entityInfo.GetIdentifier());
            Vanrise.Entities.UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();

            updateOperationOutput.Result = updateActionSucc ? Vanrise.Entities.UpdateOperationResult.Succeeded : Vanrise.Entities.UpdateOperationResult.Failed;

            updateOperationOutput.UpdatedObject = null;

            return updateOperationOutput;
        }

        public List<int> GetMemberIds(int orgChartId, int managerId)
        {
            OrgChart orgChart = GetOrgChartById(orgChartId);
            Member manager = GetMember(managerId, orgChart.Hierarchy);

            // manager would be null if he/she were added after the org chart's hierarchy was created
            // one way to get around this problem is to update all org charts to include the new users in the hierarchy
            List<int> memberIds = new List<int>();

            if (manager != null)
                memberIds = GetMemberIdsRecursively(manager);

            memberIds.Add(managerId);

            return memberIds;
        }
        
        #endregion
        
        #region Private Methods

        Dictionary<int, OrgChart> GetCachedOrgCharts()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetOrgCharts",
               () =>
               {
                   IOrgChartDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
                   IEnumerable<OrgChart> orgCharts = dataManager.GetOrgCharts();
                   return orgCharts.ToDictionary(orgChart => orgChart.OrgChartId, orgChart => orgChart);
               });
        }

        Member GetMember(int memberId, List<Member> members)
        {
            for (int i = 0; i < members.Count; i++)
            {
                Member target = GetMemberRecursively(memberId, members[i]);

                if (target != null)
                    return target;
            }

            return null;
        }

        Member GetMemberRecursively(int memberId, Member member)
        {
            if (member.Id == memberId)
                return member;

            if (member.Members.Count > 0)
            {
                for (int i = 0; i < member.Members.Count; i++)
                {
                    Member result = GetMemberRecursively(memberId, member.Members[i]);

                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        List<int> GetMemberIdsRecursively(Member manager)
        {
            List<int> ids = new List<int>();

            foreach (Member member in manager.Members)
            {
                ids.Add(member.Id);

                if (member.Members.Count > 0)
                    ids.InsertRange(ids.Count, GetMemberIdsRecursively(member));
            }

            return ids;
        }

        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IOrgChartDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreOrgChartsUpdated(ref _updateHandle);
            }
        }

        private class OrgChartLoggableEntity : VRLoggableEntityBase
        {
            public static OrgChartLoggableEntity Instance = new OrgChartLoggableEntity();

            private OrgChartLoggableEntity()
            {

            }

            static OrgChartManager s_orgChartManager = new OrgChartManager();

            public override string EntityUniqueName
            {
                get { return "VR_Security_OrgChart"; }
            }

            public override string ModuleName
            {
                get { return "Security"; }
            }

            public override string EntityDisplayName
            {
                get { return "Org Chart"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Security_OrgChart_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                OrgChart orgChart = context.Object.CastWithValidate<OrgChart>("context.Object");
                return orgChart.OrgChartId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                OrgChart orgChart = context.Object.CastWithValidate<OrgChart>("context.Object");
                return s_orgChartManager.GetOrgChartName(orgChart);
            }
        }

        #endregion

        #region Mappers

        OrgChartInfo OrgChartInfoMapper(OrgChart orgChart)
        {
            return new OrgChartInfo()
            {
                OrgChartId = orgChart.OrgChartId,
                Name = orgChart.Name
            };
        }
        
        #endregion
    }
}
