﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Retail.BusinessEntity.Business
{
    public class SubscriberAccountBalanceSetting : AccountTypeExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("FEB17AE0-6CD4-467B-A7FA-4FEE9D8538EC"); }
        }
        public override string AccountSelector
        {
            get { return "retail-be-extendedsettings-account-selector"; }
        }
        public override IAccountManager GetAccountManager()
        {
            return new SubscriberAccountBalanceManager();
        }
    }
}
