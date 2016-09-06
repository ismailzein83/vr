using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AgentManager
    {
        public IDataRetrievalResult<AgentDetail> GetFilteredAgents(DataRetrievalInput<AgentQuery> input)
        {
            Dictionary<long, Agent> cachedAgents = this.GetCachedAgents();

            Func<Agent, bool> filterExpression = (agent) =>
                (input.Query.Name == null || agent.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, cachedAgents.ToBigResult(input, filterExpression, AgentDetailMapper));
        }

        public InsertOperationOutput<AgentDetail> AddAgent(Agent agent)
        {
            var insertOperationOutput = new InsertOperationOutput<AgentDetail>
            {
                Result = InsertOperationResult.Failed,
                InsertedObject = null
            };

            long agentId;
            IAgentDataManager dataManager = BEDataManagerFactory.GetDataManager<IAgentDataManager>();
            if (dataManager.Insert(agent, out agentId))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                agent.Id = agentId;
                insertOperationOutput.InsertedObject = AgentDetailMapper(agent);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<AgentDetail> UpdateAgent(Agent agent)
        {

            var updateOperationOutput = new UpdateOperationOutput<AgentDetail>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };

            IAgentDataManager dataManager = BEDataManagerFactory.GetDataManager<IAgentDataManager>();

            if (dataManager.Update(agent))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AgentDetailMapper(this.GetAgent(agent.Id));
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<AgentInfo> GetAgentsInfo(string nameFilter)
        {
            string nameFilterLower = nameFilter != null ? nameFilter.ToLower() : null;
            IEnumerable<Agent> agents = GetCachedAgents().Values;

            Func<Agent, bool> agentFilter = (agent) =>
            {
                if (nameFilterLower != null && !agent.Name.ToLower().Contains(nameFilterLower))
                    return false;
                return true;
            };
            return agents.MapRecords(AgentInfoMapper, agentFilter).OrderBy(x => x.Name);
        }

        Agent GetAgent(long agentId)
        {
            var allAgents = GetCachedAgents();
            return allAgents.GetRecord(agentId);
        }

        Dictionary<long, Agent> GetCachedAgents()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAgents", () =>
            {
                IAgentDataManager dataManager = BEDataManagerFactory.GetDataManager<IAgentDataManager>();
                IEnumerable<Agent> agents = dataManager.GetAgents();
                return agents.ToDictionary(kvp => kvp.Id, kvp => kvp);
            });
        }

        #region Mapperds

        AgentInfo AgentInfoMapper(Agent agent)
        {
            return new AgentInfo
            {
                AgentId = agent.Id,
                Name = agent.Name
            };
        }
        AgentDetail AgentDetailMapper(Agent agent)
        {
            return new AgentDetail
            {
                Entity = agent
            };
        }

        #endregion

        #region Classes

        private class CacheManager : BaseCacheManager
        {
            IAgentDataManager _dataManager = BEDataManagerFactory.GetDataManager<IAgentDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAgentsUpdated(ref _updateHandle);
            }


        }

        #endregion

    }
}
