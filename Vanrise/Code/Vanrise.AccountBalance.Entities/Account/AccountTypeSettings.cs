using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountTypeSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("7824DFFA-0EBF-4939-93E8-DEC6E5EDFA10"); }
        }

        public Guid AccountBusinessEntityDefinitionId { get; set; }

        public string AccountSelector { get; set; }

        public Guid UsageTransactionTypeId { get; set; }

        public Guid AlertMailMessageTypeId { get; set; }

        public BalancePeriodSettings BalancePeriodSettings { get; set; }
    }
}
