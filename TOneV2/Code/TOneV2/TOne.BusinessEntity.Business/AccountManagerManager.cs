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

        public List<AssignedCarrier> GetAssignedCarriers(List<int> parameters)
        {
            IAssignedCarrierDataManager dataManager = BEDataManagerFactory.GetDataManager<IAssignedCarrierDataManager>();
            List<int> memberIds = GetMemberIds(parameters[0], parameters[1]);
            return dataManager.GetAssignedCarriers(memberIds);
        }

        public void AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers)
        {
            IAssignedCarrierDataManager dataManager = BEDataManagerFactory.GetDataManager<IAssignedCarrierDataManager>();
            dataManager.AssignCarriers(updatedCarriers);
        }

        public int? GetLinkedOrgChartId()
        {
            OrgChartManager orgChartManager = new OrgChartManager();
            return orgChartManager.GetLinkedOrgChartId(new OrgChartAccountManagerInfo());
        }

        public void UpdateLinkedOrgChart(int orgChartId)
        {
            OrgChartManager orgChartManager = new OrgChartManager();
            orgChartManager.UpdateOrgChartLinkedEntity(orgChartId, new OrgChartAccountManagerInfo());
        }

        private List<int> GetMemberIds(int orgChartId, int managerId)
        {
            OrgChartManager orgChartManager = new OrgChartManager();
            return orgChartManager.GetMemberIds(orgChartId, managerId);
        }
    }
}
