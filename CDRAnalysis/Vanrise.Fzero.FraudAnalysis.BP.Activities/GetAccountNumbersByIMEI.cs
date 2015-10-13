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
            IAccountStatusDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountStatusDataManager>();
            AccountNumbersByIMEI accountNumbersByIMEI = new AccountNumbersByIMEI();
            List<CaseStatus> caseStatuses = new List<CaseStatus>();
            caseStatuses.Add(CaseStatus.Open);
            caseStatuses.Add(CaseStatus.Pending);
            caseStatuses.Add(CaseStatus.ClosedFraud);

            dataManager.LoadAccountStatus(((accountStatus) =>
                {
                    foreach ( var imei in accountStatus.AccountInfo.IMEIs)
                    {
	                        // This code does two hash lookups.
	                        HashSet<String> accountNumbers;
	                        if (accountNumbersByIMEI.TryGetValue(imei, out accountNumbers))
	                        {
                                accountNumbers.Add(accountStatus.AccountNumber);
	                            accountNumbersByIMEI[imei] = accountNumbers;
	                        }
                            else
                            {
                                accountNumbers = new HashSet<string>();
                                accountNumbers.Add(accountStatus.AccountNumber);
                                accountNumbersByIMEI.Add(imei, accountNumbers);
                            }
                    }

                }), caseStatuses);


            context.SetValue(AccountNumbersByIMEI, accountNumbersByIMEI);

        }

    }
}
