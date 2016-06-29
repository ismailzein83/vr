using System;
using Vanrise.Entities.EntitySynchronization;
namespace TOne.WhS.DBSync.Entities
{
    public class SourceCustomerSellingProduct : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public int CustomerId { get; set; }
        public int CustomerSellingProductId { get; set; }
        public int SellingProductId { get; set; }
        public DateTime BED { get; set; }
    }
}
