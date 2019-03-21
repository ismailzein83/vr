using System;
using System.Collections.Generic;
using Retail.BusinessEntity.Entities;
using Retail.Interconnect.Entities;

namespace Retail.Interconnect.Business
{
    public class AccountPartInterconnectSetting : AccountPartSettings
    {
        public override Guid ConfigId
        {
            get { return Guid.Empty; }
        }
        public bool RepresentASwitch { get; set; }
        public List<SMSServiceTypeEntity> SMSServiceTypes { get; set; }
    }

    public class SMSServiceTypeEntity
    {
        public int SMSServiceTypeId { get; set; }
    }
}
