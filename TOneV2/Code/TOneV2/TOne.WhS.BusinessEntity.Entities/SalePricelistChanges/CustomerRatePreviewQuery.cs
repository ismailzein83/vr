using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities.SalePricelistChanges
{
   public class CustomerRatePreviewQuery
    {
        public long ProcessInstanceId { get; set; }
        public List<int> CustomerIds { get; set; }
    }
}
