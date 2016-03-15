using System.Collections.Generic;
namespace Vanrise.BusinessProcess.Entities
{
    public class BPTaskType
    {
        public int BPTaskTypeId { get; set; }
        public string Name { get; set; }
        public BPTaskTypeSettings Settings { get; set; }
        public List<BPTaskAction> Actions { get; set; }
    }
}