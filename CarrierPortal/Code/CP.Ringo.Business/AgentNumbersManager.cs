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
        public IDataRetrievalResult<AgentNumberDetail> GetFilteredAgentNumbers(DataRetrievalInput<PortalAgentNumberRequestQuery> query)
        {
            BigResult<AgentNumberDetail> result = new BigResult<AgentNumberDetail>();
            VRInterAppRestConnection connectionSettings = GetInterAppConnection();
            var retailAccountSettings = GetRetailAccountSettings();
            DataRetrievalInput<AgentNumberRequestQuery> apiQuery = BuildApiQuery(query);

            var bigResult = connectionSettings.Post<DataRetrievalInput<AgentNumberRequestQuery>, BigResult<AgentNumberRequestDetail>>("api/Retail_Ringo/RingoAgentNumberRequest/GetFilteredAgentNumberRequests", apiQuery);
            List<AgentNumberDetail> agentNumbers = new List<AgentNumberDetail>();
            List<AgentNumber> tempList = new List<AgentNumber>();
            foreach (var agentNumberDetail in bigResult.Data.OrderByDescending(itm => itm.Entity.Id))
            {
                if (agentNumberDetail.Entity.Settings.AgentNumbers == null || agentNumberDetail.Entity.Status == Status.Rejected)
                    continue;
                tempList.AddRange(agentNumberDetail.Entity.Settings.AgentNumbers);
            }
            Func<AgentNumber, bool> filterExpression = BuildFilterExpression(query);
            agentNumbers.AddRange(tempList.MapRecords(AgentNumberDetailMapper, filterExpression));
            result.Data = agentNumbers;

            return result;
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
            settings.ThrowIfNull("settings", RingoRetailSettings.SETTING_TYPE);
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection(settings.RetailVRConnectionId);
            vrConnection.ThrowIfNull("vrConnection", settings.RetailVRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings;
        }

        Func<AgentNumber, bool> BuildFilterExpression(DataRetrievalInput<PortalAgentNumberRequestQuery> query)
        {
            Func<AgentNumber, bool> filterExpression = (agentNumber) =>
            {
                if (agentNumber.Status == NumberStatus.Rejected)
                    return false;
                if (!string.IsNullOrEmpty(query.Query.Number) && !agentNumber.Number.StartsWith(query.Query.Number))
                    return false;
                if (query.Query.Status != null && !query.Query.Status.Contains((int)agentNumber.Status))
                    return false;
                return true;
            };
            return filterExpression;
        }

        DataRetrievalInput<AgentNumberRequestQuery> BuildApiQuery(DataRetrievalInput<PortalAgentNumberRequestQuery> query)
        {
            VRInterAppRestConnection connectionSettings = GetInterAppConnection();
            var retailAccountSettings = GetRetailAccountSettings();
            DataRetrievalInput<AgentNumberRequestQuery> apiQuery = new DataRetrievalInput<AgentNumberRequestQuery>
            {
                Query = new AgentNumberRequestQuery
                {
                    AgentIds = new List<long>() { retailAccountSettings.AccountId },
                    Number = query.Query.Number,
                    Status = query.Query.Status
                },
                DataRetrievalResultType = query.DataRetrievalResultType,
                FromRow = query.FromRow,
                GetSummary = query.GetSummary,
                IsAPICall = query.IsAPICall,
                IsSortDescending = query.IsSortDescending,
                ResultKey = query.ResultKey,
                SortByColumnName = "Entity.Id",
                ToRow = query.ToRow
            };
            return apiQuery;
        }

        AgentNumberDetail AgentNumberDetailMapper(AgentNumber agentNumber)
        {
            return new AgentNumberDetail
            {
                Entity = agentNumber,
                StatusDescription = Utilities.GetEnumDescription<NumberStatus>(agentNumber.Status)
            };
        }

        #endregion
        public class AgentNumberDetail
        {
            public AgentNumber Entity { get; set; }
            public string StatusDescription { get; set; }
        }
    }
}
