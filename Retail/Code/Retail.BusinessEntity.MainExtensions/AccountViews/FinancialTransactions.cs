﻿using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountViews
{
    public class FinancialTransactions : AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A9475C63-ECA4-4C01-B9FF-11DF8AA4C157"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-financialtransactions-view";
            }
            set
            {

            }
        }
        public override bool DoesUserHaveAccess(IAccountViewDefinitionCheckAccessContext context)
        {
            Vanrise.AccountBalance.Business.AccountTypeManager _vAccountTypeManger = new Vanrise.AccountBalance.Business.AccountTypeManager();
            var accountTypeId = new AccountBalanceManager().GetAccountBalanceTypeId(context.AccountBEDefinitionId);
            return _vAccountTypeManger.DoesUserHaveViewAccess(context.UserId,accountTypeId);
        }
    }
}
