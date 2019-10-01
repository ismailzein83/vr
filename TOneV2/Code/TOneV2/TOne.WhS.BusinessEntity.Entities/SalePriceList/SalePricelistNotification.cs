using System;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePricelistNotification
    {
        public long Id { get; set; }
        public int CustomerID { get; set; }
        public int PricelistId { get; set; }
        public DateTime EmailCreationDate { get; set; }
    }
}
