using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
namespace Vanrise.AccountBalance.Business
{
    public class GenericFinancialAccountBalanceManager: IAccountManager
    {
        GenericFinancialAccountConfiguration _configuration;
        public GenericFinancialAccountBalanceManager(GenericFinancialAccountConfiguration configuration)
        {
            _configuration = configuration;
        }
        public dynamic GetAccount(IAccountContext context)
        {
            GenericFinancialAccountManager manager = new GenericFinancialAccountManager(_configuration);
            return manager.GetFinancialAccount(context.AccountId);
        }
        public Vanrise.AccountBalance.Entities.AccountInfo GetAccountInfo(IAccountInfoContext context)
        {
            GenericFinancialAccountManager manager = new GenericFinancialAccountManager(_configuration);
            var financialAccount = manager.GetFinancialAccount(context.AccountId);
            financialAccount.ThrowIfNull("financialAccount", context.AccountId);

            Vanrise.AccountBalance.Entities.AccountInfo accountInfo = new Vanrise.AccountBalance.Entities.AccountInfo
            {
                Name = financialAccount.Name,
                StatusDescription = new Vanrise.Common.Business.StatusDefinitionManager().GetStatusDefinitionName(financialAccount.Status),
                CurrencyId = financialAccount.CurrencyId,
                BED = financialAccount.BED,
                EED = financialAccount.EED,
                Status = financialAccount.Status,
            };
            return accountInfo;
        }
    }
}
