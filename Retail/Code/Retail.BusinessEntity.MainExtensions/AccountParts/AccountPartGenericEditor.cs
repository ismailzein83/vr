using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartGenericEditor : AccountPartSettings
    {
        public override Guid ConfigId
        {
            get
            {
                return Guid.NewGuid();
            }
        };
        public dynamic DataRecord { get; set; }
    }
}
