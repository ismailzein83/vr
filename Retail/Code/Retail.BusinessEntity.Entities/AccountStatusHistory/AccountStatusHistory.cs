using System;

namespace Retail.BusinessEntity.Entities
{
    public class AccountStatusHistory
    {
        public long AccountStatusHistoryId { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
        public long AccountId { get; set; }
        public Guid StatusId { get; set; }
        public Guid? PreviousStatusId { get; set; }
        public DateTime StatusChangedDate { get; set; }
    }
}