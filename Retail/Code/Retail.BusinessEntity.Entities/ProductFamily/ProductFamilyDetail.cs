﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ProductFamilyDetail
    {
        public ProductFamily Entity { get; set; }

        public Guid AccountBEDefinitionId { get; set; }
        
        public bool AllowEdit { get; set; }
    }
}
