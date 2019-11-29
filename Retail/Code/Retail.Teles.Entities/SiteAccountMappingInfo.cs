﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public class SiteAccountMappingInfo : BaseAccountExtendedSettings
    {
        public string TelesEnterpriseId { get; set; }
        public string TelesSiteId { get; set; }
        public ProvisionStatus? Status { get; set; }

    }
}
