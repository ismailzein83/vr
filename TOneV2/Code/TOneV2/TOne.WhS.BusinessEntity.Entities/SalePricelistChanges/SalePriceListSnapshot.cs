using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePriceListSnapShot
    {
        public long PriceListId { get; set; }
        public SnapShotDetail SnapShotDetail { get; set; }
    }
    public class SnapShotDetail
    {
        public List<long> CodeIds { get; set; }
    }
}
