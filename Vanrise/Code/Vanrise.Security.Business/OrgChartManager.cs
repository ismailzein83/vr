using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class OrgChartManager
    {
        public List<OrgChart> GetOrgCharts()
        {
            IOrgChartDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
            return dataManager.GetOrgCharts();
        }

        public List<OrgChart> GetFilteredOrgCharts(int fromRow, int toRow, string name)
        {
            IOrgChartDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
            return dataManager.GetFilteredOrgCharts(fromRow, toRow, name);
        }

        public OrgChart GetOrgChartById(int orgChartId)
        {
            IOrgChartDataManager datamanager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
            return datamanager.GetOrgChartById(orgChartId);
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
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                orgChartObject.Id = orgChartId;
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
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = orgChartObject;
            }
            return updateOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<object> DeleteOrgChart(int orgChartId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();

            deleteOperationOutput.Result = DeleteOperationResult.Failed;

            // make sure that the org chart isn't linked to an entity
            if (GetLinkedOrgChartId(new OrgChartLinkedEntityInstance()) == null)
            {
                IOrgChartDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
                bool updateActionSucc = dataManager.DeleteOrgChart(orgChartId);

                if (updateActionSucc)
                {
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
            IOrgChartDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IOrgChartDataManager>();
            OrgChart orgChart = dataManager.GetOrgChartById(orgChartId);

            Member manager = GetMember(managerId, orgChart.Hierarchy);

            List<int> memberIds = new List<int>();
            memberIds = GetMemberIdsRecursively(manager);
            memberIds.Add(manager.Id);

            return memberIds;
        }

        public Member GetMember(int memberId, List<Member> members) {
            for (int i = 0; i < members.Count; i++ )
            {
                Member target = GetMemberRecursively(memberId, members[i]);

                if (target != null)
                    return target;
            }

            return null;
        }

        public Member GetMemberRecursively(int memberId, Member member)
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

        public List<int> GetMemberIdsRecursively(Member manager)
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
    }
}
