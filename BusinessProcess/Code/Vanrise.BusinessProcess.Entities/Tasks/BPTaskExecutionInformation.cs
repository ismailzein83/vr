using System.Runtime.Serialization;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPTaskExecutionInformation
    {
        [IgnoreDataMember]
        public string Decision { get; set; }

        [IgnoreDataMember]
        public string Notes { get; set; }

        public BPTaskAction TakenAction { get; set; }
    }
}
