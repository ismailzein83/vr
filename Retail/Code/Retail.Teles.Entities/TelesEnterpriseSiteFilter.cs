﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public class TelesEnterpriseSiteFilter
    {
        public Guid AccountBEDefinitionId { get; set; }
        public List<ITelesEnterpriseSiteFilter> Filters { get; set; }
    }
}
