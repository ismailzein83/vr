using System;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class UploadSupplierSMSRateChangesLog
    {
        public long? ProcessDraftID { get; set; }
        public long FileID { get; set; }
        public int NumberOfItemsAdded { get; set; }
        public int NumberOfItemsFailed { get; set; }
        public String ErrorMessage { get; set; }
    }
}