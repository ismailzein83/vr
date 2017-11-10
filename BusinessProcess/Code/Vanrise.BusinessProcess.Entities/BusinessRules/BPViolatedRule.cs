using System;
namespace Vanrise.BusinessProcess.Entities
{
    public class BPViolatedRule : IComparable<BPViolatedRule>
    {
        public IRuleTarget Target { get; set; }

        public string Message { get; set; }
        public BusinessRule Rule { get; set; }

        public int CompareTo(BPViolatedRule other)
        {
            Enum severity = this.Rule.Action.GetSeverity();
            Enum otherServerity = other.Rule.Action.GetSeverity();

            return severity.CompareTo(otherServerity);
        }
    }
}
