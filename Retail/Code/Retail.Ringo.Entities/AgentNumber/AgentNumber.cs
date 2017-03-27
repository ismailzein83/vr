using System;
using System.ComponentModel;
namespace Retail.Ringo.Entities
{
    public enum NumberStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Accepted")]
        Accepted,
        [Description("Rejected")]
        Rejected
    }
    public class AgentNumber
    {
        public string Number { get; set; }
        public NumberStatus Status { get; set; }

    }
}
