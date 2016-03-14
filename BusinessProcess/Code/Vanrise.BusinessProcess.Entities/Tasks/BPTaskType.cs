namespace Vanrise.BusinessProcess.Entities
{
    public enum TaskType { Test = 1 }
    public class BPTaskType
    {
        public int BPTaskTypeId { get; set; }
        public string Name { get; set; }
        public BPTaskTypeSettings Settings { get; set; }
    }
}
