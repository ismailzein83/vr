using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class GetAccountNumbersByIMEI : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<AccountNumbersByIMEI> AccountNumbersByIMEI { get; set; }

        #endregion


        protected override void Execute(CodeActivityContext context)
        {
            IAccountInfoDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountInfoDataManager>();
            AccountNumbersByIMEI accountNumbersByIMEI = new AccountNumbersByIMEI();

            dataManager.LoadAccountInfo(((accountInfo) =>
                {
                    foreach ( var imei in accountInfo.InfoDetail.IMEIs)
                    {
	                        // This code does two hash lookups.
	                        HashSet<String> accountNumbers;
	                        if (accountNumbersByIMEI.TryGetValue(imei, out accountNumbers))
	                        {
                                accountNumbers.Add(accountInfo.AccountNumber);
	                            accountNumbersByIMEI[imei] = accountNumbers;
	                        }
                            else
                            {
                                accountNumbers = new HashSet<string>();
                                accountNumbers.Add(accountInfo.AccountNumber);
                                accountNumbersByIMEI.Add(imei, accountNumbers);
                            }
                    }

                }));


            context.SetValue(AccountNumbersByIMEI, accountNumbersByIMEI);

        }

    }
}
