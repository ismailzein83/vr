namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SupplierSMSRateChangesQuery
    {
        public long ProcessDraftID { get; set; }

        public int SupplierID { get; set; }

        public SupplierSMSRateChangesFilter Filter { get; set; }
    }

    public class SupplierSMSRateChangesFilter
    {
        public char? CountryChar { get; set; }
    }
}
