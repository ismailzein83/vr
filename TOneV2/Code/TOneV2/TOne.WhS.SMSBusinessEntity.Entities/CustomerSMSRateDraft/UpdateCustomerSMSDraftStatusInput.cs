namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class UpdateCustomerSMSDraftStatusInput
    {
        public long ProcessDraftID { get; set; }
        public ProcessStatus NewStatus { get; set; }
    }
}
