
namespace Vanrise.BusinessProcess.Entities
{
    public class BPTaskDetail
    {
        public BPTask Entity { get; set; }
        public string StatusDescription { get { if (this.Entity != null) return this.Entity.Status.ToString(); return null; } }
        public string Notes { get { return this.Entity != null && this.Entity.TaskExecutionInformation != null ? this.Entity.TaskExecutionInformation.Notes : null; } }
        public string Decision { get { return this.Entity != null && this.Entity.TaskExecutionInformation != null ? this.Entity.TaskExecutionInformation.Decision : null; } }
    }
}
