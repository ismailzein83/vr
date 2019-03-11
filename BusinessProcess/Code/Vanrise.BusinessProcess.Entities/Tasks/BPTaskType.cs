using System;
using System.Collections.Generic;
namespace Vanrise.BusinessProcess.Entities
{
    public class BPTaskType
    {
        public Guid BPTaskTypeId { get; set; }
        public string Name { get; set; }
        public BaseBPTaskTypeSettings Settings { get; set; }
    }
}