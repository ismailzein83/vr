using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class ProcessManager
    {
        public List<BPDefinition> GetDefinitions()
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetDefinitions();
        }
    }
}
