using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Data
{
    public interface IAlertRuleActionExecutionDataManager:IDataManager
    {
        bool Insert(AlertRuleActionExecution alertRuleActionExecution, out long insertedId);
        void GetAletRuleAciontExecutionsToDelete(Action<AlertRuleActionExecution> onAlertRuleActionExecutionReady);
        bool Delete(long alertRuleActionExecutionId);
    }
}
