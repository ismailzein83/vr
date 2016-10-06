using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartActivation : AccountPartSettings
    {
        public static Guid _ConfigId = new Guid("5BB10B38-0167-43C1-A849-369652108E1B");
        public override Guid ConfigId { get { return _ConfigId; }}
      //  public const int ExtensionConfigId = 20;
        public AccountStatus Status { get; set; }
        public DateTime ActivationDate { get; set; }


    }
}
