using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountBEFilter : IAccountFilter
    {
        static AccountBEManager s_accountManager = new AccountBEManager();
        public FinancialAccountEffective? FinancialAccountEffective { get; set; }
        public bool IsExcluded(IAccountFilterContext context)
        {
            if (context.Account != null)
            {
                AccountBEFinancialAccountsSettings accountFinancialAccountsSettings = s_accountManager.GetExtendedSettings<AccountBEFinancialAccountsSettings>(context.Account);
                if (accountFinancialAccountsSettings == null)
                    return true;
                if(FinancialAccountEffective.HasValue)
                {
                    switch(FinancialAccountEffective)
                    {
                        case Entities.FinancialAccountEffective.All:break;
                        case Entities.FinancialAccountEffective.EffectiveOnly :
                            if (!accountFinancialAccountsSettings.FinancialAccounts.Any(x => x.BED < DateTime.Now && (!x.EED.HasValue || x.EED.Value > DateTime.Now)))
                                  return true;
                            break;
                    }
                }
            }
            return false;
        }
    }
}
