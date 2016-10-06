using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartGeneric : AccountPartSettings
    {
        public override Guid ConfigId { get { return _ConfigId; } }
        public static Guid _ConfigId = Guid.NewGuid();
        public dynamic DataRecord { get; set; }
    }
}
