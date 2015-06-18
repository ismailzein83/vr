using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class DailyForcastingFormatted
    {
        public string Day { get; set; }       
        public double? SaleNet { get; set; }
        public string SaleNetFormatted { get; set; }
        public double? CostNet { get; set; }
        public string CostNetFormatted { get; set; }
        public string ProfitFormatted { get; set; }
        public string ProfitPercentageFormatted { get; set; }
    }
}
