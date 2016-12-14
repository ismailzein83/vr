using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class LiveBalanceToUpdate
    {
        public long LiveBalanceId { get; set; }
        public decimal Value { get; set; }
    }
}
