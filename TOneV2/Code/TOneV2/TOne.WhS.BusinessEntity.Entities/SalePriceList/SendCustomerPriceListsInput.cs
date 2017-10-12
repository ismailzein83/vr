using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SendPricelistsWithCheckPreviousInput
    {
        public long ProcessInstanceId { get; set; }
        public bool SelectAll { get; set; }
        public IEnumerable<int> SelectedPriceListIds { get; set; }
        public IEnumerable<int> NotSelectedPriceListIds { get; set; }
        public bool CompressAttachement { get; set; }
    }

    public class SendPricelistsInput
    {
        public List<int> PricelistIds { get; set; }
        public bool CompressAttachement { get; set; }
    }

    public class SendCustomerPricelistsResponse
    {
        public List<CarrierAccountInfo> Customers { get; set; }
        public List<int> PricelistIds { get; set; }
        public bool AllEmailsHaveBeenSent { get; set; }
    }
}
