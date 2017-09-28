using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SendCustomerPriceListsInput
    {
        public long ProcessInstanceId { get; set; }
        public bool SelectAll { get; set; }
        public IEnumerable<int> SelectedPriceListIds { get; set; }
        public IEnumerable<int> NotSelectedPriceListIds { get; set; }
        public bool CompressAttachement { get; set; }
    }
}
