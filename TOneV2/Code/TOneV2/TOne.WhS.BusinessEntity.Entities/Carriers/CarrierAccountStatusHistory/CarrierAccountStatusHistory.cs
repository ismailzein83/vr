using System;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CarrierAccountStatusHistory
    {
        public int CarrierAccountStatusHistoryId { get; set; }

        public int CarrierAccountId { get; set; }

        public ActivationStatus Status { get; set; }

        public ActivationStatus? PreviousStatus { get; set; }

        public DateTime StatusChangedDate { get; set; }
    }
}
