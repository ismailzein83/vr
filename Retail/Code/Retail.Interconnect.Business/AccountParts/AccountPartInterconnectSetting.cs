using System;
using Retail.BusinessEntity.Entities;

namespace Retail.Interconnect.Business
{
    public class AccountPartInterconnectSetting : AccountPartSettings
    {
        public override Guid ConfigId
        {
            get { return Guid.Empty; }
        }
        public bool RepresentASwitch { get; set; }
    }
}
