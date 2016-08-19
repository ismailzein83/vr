using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Web.Models;

namespace Vanrise.BusinessProcess.Web.ModelMappers
{
    public class BPMappers
    {
        public static BPInstanceModel MapInstance(BPInstance ins)
        {
            return new BPInstanceModel
            {
                ProcessInstanceID = ins.ProcessInstanceID,
                Title = ins.Title,
                ParentProcessID = ins.ProcessInstanceID,
                DefinitionID = ins.DefinitionID,
                WorkflowInstanceID = ins.WorkflowInstanceID,
                Status = ins.Status,
                InputArgument = ins.InputArgument,
                LastMessage = ins.LastMessage,
                CreatedTime = ins.CreatedTime,
                StatusDescription = ins.Status.ToString(),
                StatusUpdatedTime = ins.StatusUpdatedTime
            };
        }
    }
}