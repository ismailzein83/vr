namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class UpdateSupplierSMSDraftStatusInput
    {
        public long ProcessDraftID { get; set; }
        public ProcessStatus NewStatus { get; set; }
    }
}
