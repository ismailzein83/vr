using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;

namespace Vanrise.Queueing
{
    public class ExecutionControlManager
    {
        static IExecutionControlDataManager s_dataManager = QDataManagerFactory.GetDataManager<IExecutionControlDataManager>();

        public bool IsExecutionPaused()
        {
            return s_dataManager.IsExecutionPaused();
        }

        public bool UpdateExecutionPaused(bool isPaused)
        {            
            s_dataManager.UpdateExecutionPaused(isPaused);
            return IsExecutionPaused();
        }
    }
}
