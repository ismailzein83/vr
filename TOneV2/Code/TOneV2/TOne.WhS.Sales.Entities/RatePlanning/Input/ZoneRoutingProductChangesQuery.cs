﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class ZoneRoutingProductChangesQuery
    {
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
    }
}
