using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountBEFilter : IAccountFilter
    {
        static AccountBEManager s_accountManager = new AccountBEManager();
        public VRAccountStatus? Status { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsEffectiveInFuture { get; set; }
        public bool IsExcluded(IAccountFilterContext context)
        {
            if (context.Account != null)
            {
                AccountBEFinancialAccountsSettings accountFinancialAccountsSettings = s_accountManager.GetExtendedSettings<AccountBEFinancialAccountsSettings>(context.Account);
                if (accountFinancialAccountsSettings == null)
                    return true;
                if (this.Status.HasValue)
                {
                    switch (this.Status.Value)
                    {
                        case  VRAccountStatus.Active:
                            if (IsEffectiveInFuture.HasValue)
                            {
                                if(IsEffectiveInFuture.Value)
                                {
                                    if (!accountFinancialAccountsSettings.FinancialAccounts.Any(x => !x.EED.HasValue || x.EED > DateTime.Now))
                                        return true;
                                }
                            }
                            if(EffectiveDate.HasValue)
                            {
                                if (!accountFinancialAccountsSettings.FinancialAccounts.Any(x => x.BED < EffectiveDate.Value && (!x.EED.HasValue || x.EED.Value > EffectiveDate.Value)))
                                    return true;
                            }
                            break;
                        case VRAccountStatus.InActive:
                            break;
                    }
                }
            }
            return false;
        }
    }
}
