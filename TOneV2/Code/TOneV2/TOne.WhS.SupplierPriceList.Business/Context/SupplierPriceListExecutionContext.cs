﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierPriceListExecutionContext : ISupplierPriceListExecutionContext
    {
        public int InputFileId { get; set; }
    }
}
