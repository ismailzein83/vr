using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class LiveBalanceLastThresholdUpdateEntity
    {
        public Guid AccountTypeId { get; set; }
        public long AccountId { get; set; }
        public decimal? LastExecutedActionThreshold { get; set; }
        public List<decimal> ActiveAlertThresholds { get; set; }
    }
}
