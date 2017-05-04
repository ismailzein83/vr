using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartOperatorSetting : AccountPartSettings, IOperatorSetting
    {
        public override Guid ConfigId { get { return new Guid("A2F223CF-3570-4469-BFB6-4A918ACF016B"); } }

        public bool IsMobile { get; set; }
    }
}
