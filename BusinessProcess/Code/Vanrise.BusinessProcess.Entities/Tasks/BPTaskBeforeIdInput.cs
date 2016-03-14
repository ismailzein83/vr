
namespace Vanrise.BusinessProcess.Entities
{
    public class BPTaskBeforeIdInput
    {
        public long LessThanID { get; set; }
        public int NbOfRows { get; set; }
        public int? ProcessInstanceId { get; set; }
    }
}
