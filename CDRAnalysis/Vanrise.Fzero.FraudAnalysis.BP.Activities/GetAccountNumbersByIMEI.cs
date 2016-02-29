using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class GetAccountNumbersByIMEI : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<AccountNumbersByIMEIDictionary> AccountNumbersByIMEI { get; set; }

        #endregion


        protected override void Execute(CodeActivityContext context)
        {
            IAccountInfoDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountInfoDataManager>();
            AccountNumbersByIMEIDictionary accountNumbersByIMEI = new AccountNumbersByIMEIDictionary();

            dataManager.LoadAccountInfo(new CaseStatus[] { CaseStatus.ClosedFraud, CaseStatus.Open, CaseStatus.Pending }, ((accountInfo) =>
                {
                    if (accountInfo.InfoDetail.IMEIs != null)
                    {
                        foreach (var imei in accountInfo.InfoDetail.IMEIs)
                        {
                            // This code does two hash lookups.
                            HashSet<String> accountNumbers = accountNumbersByIMEI.GetOrCreateItem(imei);
                            accountNumbers.Add(accountInfo.AccountNumber);
                        }
                    }
                }));


            context.SetValue(AccountNumbersByIMEI, accountNumbersByIMEI);

        }

    }
}
