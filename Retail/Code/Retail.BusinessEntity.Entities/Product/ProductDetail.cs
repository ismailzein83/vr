﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ProductDetail
    {
        public Product Entity { get; set; }

        public bool AllowEdit { get; set; }
    }
}
