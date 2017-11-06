﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CarrierProfileDetail
    {
        public CarrierProfile Entity { get; set; }

        public String CountryName { get; set; }
        public string InvoiceTypeDescription { get; set; }
        public string InvoiceSettingName { get; set; }
    }
}
