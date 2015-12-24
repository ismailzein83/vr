using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Entities.Tasks;

namespace Vanrise.BusinessProcess.Client
{
    public class BPTaskClient
    {
        public ExecuteBPTaskOutput ExecuteTask(ExecuteBPTaskInput executeBPTaskInput)
        {
            if (executeBPTaskInput == null)
                throw new ArgumentNullException("executeBPTaskInput");
            string bookMark = BPTask.GetTaskWFBookmark(executeBPTaskInput.TaskId);
            long processInstanceId = 0;
            throw new Exception("Get ProcessInstanceID from BPTask table");
            BPClient bpClient = new BPClient();
            var triggerProcessEventInput = new TriggerProcessEventInput
            {
                ProcessInstanceId = processInstanceId,
                BookmarkName = bookMark,
                EventData = executeBPTaskInput
            };
            var triggerProcessEventOutput = bpClient.TriggerProcessEvent(triggerProcessEventInput);
            ExecuteBPTaskOutput output = new ExecuteBPTaskOutput();
            if (triggerProcessEventOutput == null)
                output.Result = ExecuteBPTaskResult.Succeeded;
            else
            {
                switch(triggerProcessEventOutput.Result)
                {
                    case TriggerProcessEventResult.Succeeded: output.Result = ExecuteBPTaskResult.Succeeded; break;
                    case TriggerProcessEventResult.ProcessInstanceNotExists: output.Result = ExecuteBPTaskResult.ProcessInstanceNotExists; break;
                }
            }
            return output;
        }
    }
}
