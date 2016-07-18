using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountServiceDetail
    {
        public AccountService Entity { get; set; }
        public string AccountName { get; set; }
        public string ServiceTypeTitle { get; set; }
        public string ServiceChargingPolicyName { get; set; }
        public string StatusDesciption { get; set; }
        public IEnumerable<ActionDefinitionInfo> ActionDefinitions { get; set; }
        public string StatusColor { get; set; }
    }
}
