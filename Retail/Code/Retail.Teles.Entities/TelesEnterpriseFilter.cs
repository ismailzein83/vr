﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public class TelesEnterpriseFilter
    {
        public Guid AccountBEDefinitionId { get; set; }
        public string TelesDomainId { get; set; }
        public List<ITelesEnterpriseFilter> Filters { get; set; }
    }
}
