﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountLocatorContext : IFinancialAccountLocatorContext
    {
        public Guid AccountDefinitionId { get; set; }

        public long AccountId { get; set; }

        public DateTime EffectiveOn { get; set; }

        public long FinancialAccountId { get; set; }

        public string BalanceAccountId { get; set; }

        public Guid BalanceAccountTypeId { get; set; }
    }
}
