﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class SupplierGroupSettings
    {
        public int ConfigId { get; set; }
    }

    public class SelectiveSuppliersSettings : SupplierGroupSettings
    {
        public List<int> SupplierIds { get; set; }
    }
}
