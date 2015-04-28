using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NumberProfile
    {
        public string subscriberNumber { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public int?  countOutCalls { get; set; }
        public int?      diffOutputNumb { get; set; }
        public int?      countOutInter { get; set; }
        public int?      countInInter { get; set; }
        public decimal?  callOutDurAvg { get; set; }
        public int ? countOutFail { get; set; }
        public int   countInFail { get; set; }
        public decimal?  totalOutVolume { get; set; }
        public decimal   totalInVolume { get; set; }
        public int       diffInputNumbers { get; set; }
        public int?      countOutSMS { get; set; }
        public int?      totalIMEI { get; set; }
        public int?      totalBTS { get; set; }
        public int?      isOnNet { get; set; }
        public decimal?  totalDataVolume { get; set; }
        public int?  periodId { get; set; }                
        public int   countInCalls { get; set; }
        public decimal   callInDurAvg { get; set; }
        public int ?     countOutOnNet { get; set; }
        public int?      countInOnNet { get; set; }
        public int?      countOutOffNet { get; set; }
        public int?      countInOffNet { get; set; }
        

    }
}
