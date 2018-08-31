using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BaseBPInstance
    {
        public const string REQUIREDPERMISSIONSET_MODULENAME = "BP_BPInstanceReqPerm";
        public string Title { get; set; }
        public long? ParentProcessID { get; set; }
        public int InitiatorUserId { get; set; }
        public string EntityId { get; set; }
        public Guid DefinitionID { get; set; }
        public BPInstanceStatus Status { get; set; }
        public BaseProcessInputArgument InputArgument { get; set; }
        public int? ViewRequiredPermissionSetId { get; set; }
        public ProcessCompletionNotifier CompletionNotifier { get; set; }
        public Guid? TaskId { get; set; }
    }

    public class BPInstance : BaseBPInstance
    {
        public long ProcessInstanceID { get; set; }
        public Guid? WorkflowInstanceID { get; set; }
        public BPInstanceAssignmentStatus AssignmentStatus { get; set; }
        public string LastMessage { get; set; }
        public int? CancellationRequestByUserId { get; set; }
        public DateTime? StatusUpdatedTime { get; set; }
        public Guid? ServiceInstanceId { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public class BPInstanceToAdd : BaseBPInstance
    {
    }
}