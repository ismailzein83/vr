using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class RevenueComparisonDetail
    {
        public RevenueComparison Entity { get; set; }
        public string ServiceTypeDescription { get; set; }
        public string EventDirectionDescription { get; set; }
    }
}
