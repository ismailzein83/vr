namespace Vanrise.BusinessProcess.Entities
{
    public class BPViolatedRule
    {
        public IRuleTarget Target { get; set; }

        public string Message { get; set; }
        public BusinessRule Rule { get; set; }
    }
}
