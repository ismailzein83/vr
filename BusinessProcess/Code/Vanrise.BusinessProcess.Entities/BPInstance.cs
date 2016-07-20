﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstance
    {
        public long ProcessInstanceID { get; set; }
        public string Title { get; set; }
        public long? ParentProcessID { get; set; }

        public int InitiatorUserId { get; set; }
        public string EntityId { get; set; }
        public int DefinitionID { get; set; }
        public Guid? WorkflowInstanceID { get; set; }
        public BPInstanceStatus Status { get; set; }
        public int RetryCount { get; set; }
        public BaseProcessInputArgument InputArgument { get; set; }
        public string LastMessage { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? StatusUpdatedTime { get; set; }
    }
}
