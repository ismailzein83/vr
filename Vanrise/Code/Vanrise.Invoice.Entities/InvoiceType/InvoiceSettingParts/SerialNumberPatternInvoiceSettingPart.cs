﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Entities
{
    public class SerialNumberPatternInvoiceSettingPart : InvoiceSettingPart
    {
        public string SerialNumberPattern { get; set; }
        public static Guid s_configId = new Guid("BB32D601-A1F7-478E-A5D9-F554DE35C85C");
        
        public override Guid ConfigId
        {
            get { return s_configId; }
        }
    }
}
