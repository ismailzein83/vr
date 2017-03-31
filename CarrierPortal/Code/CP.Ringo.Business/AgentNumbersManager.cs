using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP.Ringo.Entities;
using Retail.Ringo.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Common;
using PartnerPortal.CustomerAccess.Entities;

namespace CP.Ringo.Business
{
    public class AgentNumbersManager
    {
        public InsertOperationOutput<IEnumerable<AgentNumberDetail>> AddAgentNumbersRequest(AgentNumberRequest agentNumbersRequest)
        {
            VRInterAppRestConnection connectionSettings = GetInterAppConnection();
            var retailAccountSettings = GetRetailAccountSettings();

            agentNumbersRequest.AgentId = retailAccountSettings.AccountId;
            List<AgentNumberDetail> agentNumberDetails = new List<AgentNumberDetail>();
            var apiResult = connectionSettings.Post<AgentNumberRequest, InsertOperationOutput<AgentNumberRequestDetail>>("api/Retail_Ringo/RingoAgentNumberRequest/AddAgentNumberRequest", agentNumbersRequest);
            InsertOperationOutput<IEnumerable<AgentNumberDetail>> result = new InsertOperationOutput<IEnumerable<AgentNumberDetail>>
            {
                InsertedObject = apiResult.InsertedObject.Entity.Settings.AgentNumbers.MapRecords(AgentNumberDetailMapper),
                Message = apiResult.Message,
                Result = apiResult.Result,
                ShowExactMessage = apiResult.ShowExactMessage
            };

            return result;
        }
        public IDataRetrievalResult<AgentNumberDetail> GetFilteredAgentNumbers(DataRetrievalInput<AgentNumberRequestQuery> query)
        {
            BigResult<AgentNumberDetail> result = new BigResult<AgentNumberDetail>();
            VRInterAppRestConnection connectionSettings = GetInterAppConnection();
            var retailAccountSettings = GetRetailAccountSettings();

            query.Query = new AgentNumberRequestQuery
            {
                AgentIds = new List<long> { retailAccountSettings.AccountId }
            };
            query.SortByColumnName = "Entity.Id";
            var bigResult = connectionSettings.Post<DataRetrievalInput<AgentNumberRequestQuery>, BigResult<AgentNumberRequestDetail>>("api/Retail_Ringo/RingoAgentNumberRequest/GetFilteredAgentNumberRequests", query);
            List<AgentNumberDetail> agentNumbers = new List<AgentNumberDetail>();
            foreach (var agentNumberDetail in bigResult.Data)
            {
                if (agentNumberDetail.Entity.Settings.AgentNumbers == null || agentNumberDetail.Entity.Status == Status.Rejected)
                    continue;
                agentNumbers.AddRange(agentNumberDetail.Entity.Settings.AgentNumbers.Where(itm => itm.Status != NumberStatus.Rejected).MapRecords(AgentNumberDetailMapper));
            }
            result.Data = agentNumbers;

            return result;
        }

        private AgentNumberDetail AgentNumberDetailMapper(AgentNumber agentNumber)
        {
            return new AgentNumberDetail
            {
                Entity = agentNumber,
                StatusDescription = Utilities.GetEnumDescription<NumberStatus>(agentNumber.Status)
            };
        }

        #region Private Methods

        RetailAccountSettings GetRetailAccountSettings()
        {
            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();

            UserManager userManager = new UserManager();
            var retailAccountSettings = userManager.GetUserExtendedSettings<RetailAccountSettings>(loggedInUserId);

            retailAccountSettings.ThrowIfNull("retailAccountSettings", loggedInUserId);
            return retailAccountSettings;
        }
        VRInterAppRestConnection GetInterAppConnection()
        {
            SettingManager settingManager = new SettingManager();
            RingoRetailSettings settings = settingManager.GetSetting<RingoRetailSettings>(RingoRetailSettings.SETTING_TYPE);
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection(settings.RetailVRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings;
        }

        #endregion
        public class AgentNumberDetail
        {
            public AgentNumber Entity { get; set; }
            public string StatusDescription { get; set; }
        }
    }
}
