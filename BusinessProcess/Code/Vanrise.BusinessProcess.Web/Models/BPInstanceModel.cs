using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Web.Models
{
    public class BPInstanceModel
    {
        public long ProcessInstanceID { get; set; }
        public string Title { get; set; }
        public long? ParentProcessID { get; set; }
        public Guid DefinitionID { get; set; }
        public Guid? WorkflowInstanceID { get; set; }
        public BPInstanceStatus Status { get; set; }
        public string StatusDescription { get; set; }
        public int RetryCount { get; set; }
        public object InputArgument { get; set; }
        public string LastMessage { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? StatusUpdatedTime { get; set; }
        
    }
}