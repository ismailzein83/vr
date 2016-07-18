﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountService
    {
        public long AccountServiceId { get; set; }
        public long AccountId { get; set; }
        public Guid StatusId { get; set; }
        public int ServiceTypeId { get; set; }

        public int ServiceChargingPolicyId { get; set; }

        public AccountServiceSettings Settings { get; set; }

        public ExecutedActionsData ExecutedActionsData { get; set; }
    }
}
