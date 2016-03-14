
namespace Vanrise.BusinessProcess.Entities
{
    public class BPTaskDetail
    {
        public BPTask Entity { get; set; }
        public string StatusDescription { get { if (this.Entity != null) return this.Entity.Status.ToString(); return null; } }
    }
}
