using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Caching;
using Vanrise.Security.Business;

namespace TOne.BusinessEntity.Business
{
    public class AccountManagerManager
    {
        public List<AccountManagerCarrier> GetCarriers(int userId, int from, int to)
        {
            IAccountManagerCarrierDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountManagerCarrierDataManager>();
            return dataManager.GetCarriers(userId, from, to);
        }

        public List<AssignedCarrier> GetAssignedCarriers(int managerId, bool withDescendants, CarrierTypeFilter carrierType)
        {
            IAssignedCarrierDataManager dataManager = BEDataManagerFactory.GetDataManager<IAssignedCarrierDataManager>();
            
            List<int> memberIds = new List<int>();
            
            if (withDescendants)
            {
                int? orgChartId = GetLinkedOrgChartId();
                
                if (orgChartId != null)
                    memberIds = GetMemberIds((int)orgChartId, managerId); // GetMemberIds will add managerId to the list
            }
            else
                memberIds.Add(managerId);
            
            return dataManager.GetAssignedCarriers(memberIds, carrierType);
        }

        public Vanrise.Entities.UpdateOperationOutput<object> AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers)
        {
            IAssignedCarrierDataManager dataManager = BEDataManagerFactory.GetDataManager<IAssignedCarrierDataManager>();
            bool updateActionSucc = dataManager.AssignCarriers(updatedCarriers);

            Vanrise.Entities.UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();

            updateOperationOutput.Result = updateActionSucc ? Vanrise.Entities.UpdateOperationResult.Succeeded : Vanrise.Entities.UpdateOperationResult.Failed;
            
            updateOperationOutput.UpdatedObject = null;
           
            return updateOperationOutput;
        }

        public int? GetLinkedOrgChartId()
        {
            OrgChartManager orgChartManager = new OrgChartManager();
            return orgChartManager.GetLinkedOrgChartId(new OrgChartAccountManagerInfo());
        }

        public Vanrise.Entities.UpdateOperationOutput<object> UpdateLinkedOrgChart(int orgChartId)
        {
            OrgChartManager orgChartManager = new OrgChartManager();
            return orgChartManager.UpdateOrgChartLinkedEntity(orgChartId, new OrgChartAccountManagerInfo());
        }

        private List<int> GetMemberIds(int orgChartId, int managerId)
        {
            OrgChartManager orgChartManager = new OrgChartManager();
            return orgChartManager.GetMemberIds(orgChartId, managerId);
        }
    }
}
