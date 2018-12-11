
namespace Vanrise.BusinessProcess.Entities
{
    public class BPTaskUpdateInput
    {
        public object LastUpdateHandle { get; set; }
        public int NbOfRows { get; set; }
        public int ProcessInstanceId { get; set; }
    }
}
