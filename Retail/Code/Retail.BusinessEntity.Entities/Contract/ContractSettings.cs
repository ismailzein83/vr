﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ContractSettings
    {
        public Guid ContractDefinitionId { get; set; }

        public List<ContractPackageItem> Packages { get; set; }

        public ContractExtendedSettings ExtendedSettings { get; set; }
    }
}
