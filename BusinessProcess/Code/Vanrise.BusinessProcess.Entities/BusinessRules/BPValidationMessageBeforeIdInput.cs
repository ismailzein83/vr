namespace Vanrise.BusinessProcess.Entities
{
    public class BPValidationMessageBeforeIdInput
    {
        public long LessThanID { get; set; }
        public int NbOfRows { get; set; }
        public int BPInstanceID { get; set; }
    }
}
