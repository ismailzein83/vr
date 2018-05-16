﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.Interconnect.Business
{
    class InvoiceSettings : SettingData
    {
        public const string SETTING_TYPE = "Retail_Invoice_InvoiceSettings";
        public Dictionary<Guid, InvoiceTypeSetting> InvoiceTypeSettings { get; set; }
    }
    public class InvoiceTypeSetting
    {
        public bool NeedApproval { get; set; }
    }
}
