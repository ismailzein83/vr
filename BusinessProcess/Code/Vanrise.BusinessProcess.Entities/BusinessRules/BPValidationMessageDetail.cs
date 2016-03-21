
namespace Vanrise.BusinessProcess.Entities
{
    public class BPValidationMessageDetail
    {
        public BPValidationMessage Entity { get; set; }
        public string SeverityDescription { get { return this.Entity != null ? this.Entity.Severity.ToString() : null; } }
    }
}
