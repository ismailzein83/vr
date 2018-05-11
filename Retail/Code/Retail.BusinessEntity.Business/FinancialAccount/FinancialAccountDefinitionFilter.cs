using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountDefinitionFilter : IFinancialAccountDefinitionFilter
    {
        public long AccountId { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
        public int? SequenceNumber { get; set; }
        public bool IsMatched(IFinancialAccountDefinitionFilterContext context)
        {

            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            FinancialAccountDefinitionManager financialAccountDefinitionManager = new FinancialAccountDefinitionManager();


            string financialAccountId = null;
            if (this.SequenceNumber.HasValue)
            {
                financialAccountId = financialAccountManager.GetFinancialAccountId(AccountId, SequenceNumber.Value);
            }

            var accountClassifications = new AccountBEManager().GetAccountClassifications(this.AccountBEDefinitionId, this.AccountId);
            if (accountClassifications == null)
                return false;

            var financialAccountDefinitionSettings = financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(context.FinancialAccountDefinitionId);
            financialAccountDefinitionSettings.ThrowIfNull("financialAccountDefinitionSettings", context.FinancialAccountDefinitionId);
            if (financialAccountDefinitionSettings.ApplicableClassifications == null || financialAccountDefinitionSettings.ApplicableClassifications.Count == 0)
                return false;

            var financialAccountsByClassifications = financialAccountManager.GetFinancialAccountsWithInheritedAndChildsByAccount(this.AccountBEDefinitionId, this.AccountId);
            if (financialAccountsByClassifications != null && financialAccountsByClassifications.Count > 0)
            {
                bool checkIfClassificationDoesNotHaveEED = false;
                foreach (var classification in financialAccountDefinitionSettings.ApplicableClassifications)
                {
                    if(accountClassifications.Contains(classification))
                    {
                        var financialAccounts = financialAccountsByClassifications.GetRecord(classification);

                        if (financialAccounts != null && financialAccounts.Count() > 0)
                        {
                            if (financialAccountId != null && financialAccounts.Any(x => x.FinancialAccountId == financialAccountId && x.FinancialAccount.FinancialAccountDefinitionId == context.FinancialAccountDefinitionId))
                                return true;

                            if (financialAccounts.All(x => x.FinancialAccount.EED.HasValue && x.FinancialAccount.FinancialAccountDefinitionId == context.FinancialAccountDefinitionId))
                            {
                                return true;
                            }
                            if (financialAccounts.Any(x => !x.FinancialAccount.EED.HasValue))
                            {
                                checkIfClassificationDoesNotHaveEED = true;
                            }
                        }
                    }
                }
                if (!checkIfClassificationDoesNotHaveEED)
                    return true;
            }else
            {
                return true;
            }
            return false;
        }
    }
   
}
