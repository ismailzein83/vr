using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class BusinessCaseStatus
    {
        public string Zone { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthYear { get; set; }

        public decimal? Durations { get; set; }

        public double? Amount { get; set; }
                        
        /// <summary>
        /// DO NOT REMOVE
        /// the purpose of this method is to set the schema in the RDLC file
        /// </summary>
        /// <returns></returns>
        /// 
        public BusinessCaseStatus() { }
        public IEnumerable<BusinessCaseStatus> GetBusinessCaseStatusRDLCSchema()
        {
            return null;
        }
    }
}
