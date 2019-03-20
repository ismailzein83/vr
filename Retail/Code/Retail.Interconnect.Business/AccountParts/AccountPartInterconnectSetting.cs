using System;
using System.Collections.Generic;
using Retail.BusinessEntity.Entities;
using Retail.Interconnect.Entities;

namespace Retail.Interconnect.Business
{
    public class AccountPartInterconnectSetting : AccountPartSettings
    {
        public static Guid _ConfigId = new Guid("a0b7825c-c936-4b75-b41c-ec8fbe49932e");
        public override Guid ConfigId { get { return _ConfigId; } }
        public bool RepresentASwitch { get; set; }
        public List<SMSServiceTypeEntity> SMSServiceTypes { get; set; }
    }

    public class SMSServiceTypeEntity
    {
        public int SMSServiceTypeId { get; set; }
    }
}
