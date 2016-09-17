using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.RingoExtensions.AccountParts
{
    public class AccountPartEntitiesInfo : AccountPartSettings
    {
        public const int ExtensionConfigId = 28;
        public long AgentId { get; set; }
        public long PosId { get; set; }
        public long DistributorId { get; set; }

    }
}
