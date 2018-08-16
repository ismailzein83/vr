using System;
using Retail.BusinessEntity.Entities;

namespace Retail.Interconnect.Business
{
    public class AccountPartInterconnectSetting : AccountPartSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
        public bool RepresentASwitch { get; set; }
    }
}
