using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class LiveBalanceRecreateAlertAfterUpdateEntity
    {
        public Guid AccountTypeId { get; set; }

        public String AccountId { get; set; }

        public TimeSpan? RecreateAlertAfter { get; set; }
    }
}
