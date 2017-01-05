using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.Ringo.MainExtensions.AccountParts
{
    public class AccountPartEntitiesInfo : AccountPartSettings
    {
        public static Guid _ConfigId = Guid.Parse("A153EA40-E2B8-4D50-A569-B117F64BB2EC");
        public override Guid ConfigId { get { return _ConfigId; } }


      //  public const int ExtensionConfigId = 28;
        public long AgentId { get; set; }
        public long PosId { get; set; }
        public long DistributorId { get; set; }

    }
}
