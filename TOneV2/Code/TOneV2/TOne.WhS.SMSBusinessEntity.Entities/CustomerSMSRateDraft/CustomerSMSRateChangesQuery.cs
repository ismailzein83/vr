﻿namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class CustomerSMSRateChangesQuery
    {
        public long ProcessDraftID { get; set; }

        public int CustomerID { get; set; }

        public CustomerSMSRateChangesFilter Filter { get; set; }
    }

    public class CustomerSMSRateChangesFilter
    {
        public char? CountryChar { get; set; }
    }
}
