﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountRecurringChargeRule
    {
        public Guid RecurringChargeRuleId { get; set; }

        public string Name { get; set; }

        public AccountCondition Condition { get; set; }

        public Guid RecurringChargeDefinitionId { get; set; }

        public AccountRecurringChargeEvaluator RecurringChargeEvaluator { get; set; }
    }
}
