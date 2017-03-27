using System.ComponentModel;

namespace Retail.Ringo.Entities
{
    public enum Status
    {
        [Description("Pending")]
        Pending,
        [Description("Accepted")]
        Accepted,
        [Description("Rejected")]
        Rejected
    }
    public class AgentNumberRequest
    {
        public int Id { get; set; }
        public long AgentId { get; set; }
        public Status Status { get; set; }
        public AgentNumberSetting Settings { get; set; }
    }
}
