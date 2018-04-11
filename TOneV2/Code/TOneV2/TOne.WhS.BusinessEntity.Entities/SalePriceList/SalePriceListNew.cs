using System;
using System.ComponentModel;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePriceListNew
    {
        public int PriceListId { get; set; }
        public SalePriceListOwnerType OwnerType { get; set; }
        public SalePriceListType? PriceListType { get; set; }
        public int OwnerId { get; set; }
        public int CurrencyId { get; set; }
        public string SourceId { get; set; }
        public DateTime EffectiveOn { get; set; }
        public long ProcessInstanceId { get; set; }
        public long FileId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Description { get; set; }
        public long? PricelistStateBackupId { get; set; }
        public SalePricelistSource? PricelistSource { get; set; }
    }
}
