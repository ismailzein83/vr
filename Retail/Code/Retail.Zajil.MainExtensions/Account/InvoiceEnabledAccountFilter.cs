﻿using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Zajil.MainExtensions
{
    public class InvoiceEnabledAccountFilter : IAccountFilter
    {
        public bool IsExcluded(IAccountFilterContext context)
        {
            if (context.Account != null)
            {
                AccountBEManager accountBEManager = new AccountBEManager();
                IAccountPayment accountPayment;
                return !accountBEManager.HasAccountPayment(context.AccountBEDefinitionId, context.Account.AccountId, false, out accountPayment);
            }
            return false;
        }
    }
}
