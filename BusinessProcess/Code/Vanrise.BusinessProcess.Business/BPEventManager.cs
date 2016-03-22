using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPEventManager
    {
        public TriggerProcessEventOutput TriggerProcessEvent(TriggerProcessEventInput triggerProcessEventInput)
        {
            IBPEventDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPEventDataManager>();
            TriggerProcessEventOutput output = new TriggerProcessEventOutput();
            if (dataManager.InsertEvent(triggerProcessEventInput.ProcessInstanceId, triggerProcessEventInput.BookmarkName, triggerProcessEventInput.EventData) > 0)
                output.Result = TriggerProcessEventResult.Succeeded;
            else
                output.Result = TriggerProcessEventResult.ProcessInstanceNotExists;
            return output;
        }
    }
}
