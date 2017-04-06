using System;
using System.Collections.Generic;
using System.Linq;
using Retail.BusinessEntity.Business;
using Retail.Ringo.Data;
using Retail.Ringo.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.Ringo.Business
{
    public class AgentRequestNumberManager
    {
        public IDataRetrievalResult<AgentNumberRequestDetail> GetFilteredAgentNumberRequests(DataRetrievalInput<AgentNumberRequestQuery> input)
        {
            var allAgentNumberRequests = this.GetCachedAgentNumberRequests();

            Func<AgentNumberRequest, bool> filterExpression = (numberRequest) =>
            {
                if (input.Query.AgentIds != null && !input.Query.AgentIds.Contains(numberRequest.AgentId))
                    return false;
                if (input.Query.Status != null && !input.Query.Status.Contains((int)numberRequest.Status))
                    return false;
                if (!string.IsNullOrEmpty(input.Query.Number) && !numberRequest.Settings.AgentNumbers.Any(itm => itm.Number.StartsWith(input.Query.Number)))
                    return false;
                return true;
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allAgentNumberRequests.ToBigResult(input, filterExpression, AgentNumberRequestDetailMapper));
        }

        public InsertOperationOutput<AgentNumberRequestDetail> AddAgentNumberRequest(AgentNumberRequest agentNumberRequest)
        {
            InsertOperationOutput<AgentNumberRequestDetail> insertOperationOutput = new InsertOperationOutput<AgentNumberRequestDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int insertedId = -1;

            IAgentRequestNumberDataManager dataManager = RingoDataManagerFactory.GetDataManager<IAgentRequestNumberDataManager>();
            bool insertActionSucc = dataManager.Insert(agentNumberRequest, out insertedId);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                agentNumberRequest.Id = insertedId;
                insertOperationOutput.InsertedObject = AgentNumberRequestDetailMapper(agentNumberRequest);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<AgentNumberRequestDetail> UpdateAgentNumberRequest(AgentNumberRequest agentNumberRequest)
        {
            IAgentRequestNumberDataManager dataManager = RingoDataManagerFactory.GetDataManager<IAgentRequestNumberDataManager>();

            bool updateActionSucc = dataManager.Update(agentNumberRequest);
            UpdateOperationOutput<AgentNumberRequestDetail> updateOperationOutput = new UpdateOperationOutput<AgentNumberRequestDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AgentNumberRequestDetailMapper(agentNumberRequest);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public AgentNumberRequest GetAgentNumberRequest(int agentNumberRequestId)
        {
            var allAgentNumberRequests = this.GetCachedAgentNumberRequests();
            return allAgentNumberRequests.GetRecord(agentNumberRequestId);
        }

        Dictionary<int, AgentNumberRequest> GetCachedAgentNumberRequests()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAgentNumberRequests", () =>
            {
                IAgentRequestNumberDataManager dataManager = RingoDataManagerFactory.GetDataManager<IAgentRequestNumberDataManager>();
                IEnumerable<AgentNumberRequest> agentNumberRequests = dataManager.GetAgentNumberRequests();
                return agentNumberRequests.ToDictionary(kvp => kvp.Id, kvp => kvp);
            });
        }
        AgentNumberRequestDetail AgentNumberRequestDetailMapper(AgentNumberRequest numberRequest)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            return new AgentNumberRequestDetail
            {
                Entity = numberRequest,
                AgentName = accountBEManager.GetAccountName(new Guid("C7085851-44A6-47A6-8632-0C8E224D4226"), numberRequest.AgentId),
                StatusDescription = Vanrise.Common.Utilities.GetEnumDescription<Status>(numberRequest.Status)
            };
        }

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            object _updateHandle;
            IAgentRequestNumberDataManager _dataManager = RingoDataManagerFactory.GetDataManager<IAgentRequestNumberDataManager>();
            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreAgentNumberRequestsUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
