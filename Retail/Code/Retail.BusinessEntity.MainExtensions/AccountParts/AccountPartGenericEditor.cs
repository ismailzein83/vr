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
        public override Guid ConfigId { get { return _ConfigId; } }
        public static Guid _ConfigId = new Guid("C0A04624-8B85-488D-8810-36E367AAF3D4");

        public dynamic DataRecord { get; set; }
    }
}
