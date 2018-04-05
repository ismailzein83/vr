using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;
using Vanrise.Common;

namespace Vanrise.Runtime.Business
{
    public class RuntimeNodeConfigurationManager
    {
        static IRuntimeNodeConfigurationDataManager s_dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeConfigurationDataManager>();
        public Dictionary<Guid, RuntimeNodeConfiguration> GetAllNodeConfigurations()
        {
            //needs caching
            var allNodeConfigs = s_dataManager.GetAllNodeConfigurations();
            return allNodeConfigs != null ? allNodeConfigs.ToDictionary(n => n.RuntimeNodeConfigurationId, n => n) : null;
        }

        public RuntimeNodeConfiguration GetNodeConfiguration(Guid nodeConfigurationId)
        {
            return GetAllNodeConfigurations().GetRecord(nodeConfigurationId);
        }
    }
}
