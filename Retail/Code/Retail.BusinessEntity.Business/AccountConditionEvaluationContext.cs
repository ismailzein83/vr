﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountConditionEvaluationContext : IAccountConditionEvaluationContext
    {
        public Account Account
        {
            get;
            set;
        }
    }
}
