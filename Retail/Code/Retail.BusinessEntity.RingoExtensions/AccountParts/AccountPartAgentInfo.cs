using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.RingoExtensions.AccountParts
{
    public class AccountPartAgentInfo : AccountPartSettings
    {
        public long AgentId { get; set; }
        public long PosId { get; set; }

    }
}
