﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SalePriceListTemplateSettingsContext : ISalePriceListTemplateSettingsContext
    {
        public IEnumerable<SalePLZoneNotification> Zones { get; set; }
        public PriceListExtensionFormat PriceListExtensionFormat { get; set; }
    }
}
