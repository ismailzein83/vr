﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ProductDefinitionFilter
    {
        public Guid? AccountBEDefinitionId { get; set; }

        public bool IncludeHiddenProductDefinitions { get; set; }
    }
}
