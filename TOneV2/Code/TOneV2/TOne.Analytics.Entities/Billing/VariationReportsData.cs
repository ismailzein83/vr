using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class VariationReportsData
    {
        public string Name { get; set; }

        public decimal PeriodTypeValueAverage { get; set; }

        public decimal PeriodTypeValuePercentage { get; set; }

        public decimal PreviousPeriodTypeValuePercentage { get; set; }

        //public List<decimal> TotalDurationPerDate { get; set; }

        public decimal[][] TotalDurationPerDate ;
       
        public string CarrierAccountID { get; set; }

        public VariationReportsData(int periodCount) { 
       
            TotalDurationPerDate = new decimal[periodCount][];
        }


    }
}
