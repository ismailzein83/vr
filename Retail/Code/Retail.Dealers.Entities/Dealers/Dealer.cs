using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Dealers.Entities
{
    public class Dealer
    {
        public long DealerId { get; set; }

        public string Name { get; set; }

        public Guid TypeId { get; set; }

        public long? ParentDealerId { get; set; }

        public DealerSettings Settings { get; set; }

        public string SourceId { get; set; }
    }
}
