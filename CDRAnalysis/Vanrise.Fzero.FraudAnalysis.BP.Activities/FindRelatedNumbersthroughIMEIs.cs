using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class FindRelatedNumbersthroughIMEIsInput
    {
        public BaseQueue<CDRBatch> InputQueue { get; set; }

        public AccountNumbersByIMEIDictionary AccountsNumbersByIMEIDictionary { get; set; }

        public AccountRelatedNumberDictionary AccountRelatedNumbersDictionary { get; set; }

    }

    #endregion

    public class FindRelatedNumbersthroughIMEIs : DependentAsyncActivity<FindRelatedNumbersthroughIMEIsInput>
    {

        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }

        public InArgument<AccountNumbersByIMEIDictionary> AccountNumbersByIMEIDictionary { get; set; }

        public InArgument<AccountRelatedNumberDictionary> AccountRelatedNumbersDictionary { get; set; }

        #endregion


        protected override void DoWork(FindRelatedNumbersthroughIMEIsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            AccountNumbersByIMEIDictionary accountNumbersByIMEIDictionary = inputArgument.AccountsNumbersByIMEIDictionary;
            AccountRelatedNumberDictionary accountRelatedNumbersDictionary = inputArgument.AccountRelatedNumbersDictionary;
            List<RelatedNumber> accountRelatedNumbersToBeInserted = new List<RelatedNumber>();
            int cdrsCount = 0;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (cdrBatch) =>
                        {
                            var cdrs = cdrBatch.CDRs;

                            foreach (var cdr in cdrs)
                            {
                                if (cdr.IMEI == null)
                                    continue;
                                HashSet<String> accountNumberswithSameIMEI;
                                if (accountNumbersByIMEIDictionary.TryGetValue(cdr.IMEI, out accountNumberswithSameIMEI))
                                {
                                    foreach (var accountNumberwithSameIMEI in accountNumberswithSameIMEI)
                                    {
                                        if (accountNumberwithSameIMEI != cdr.MSISDN)
                                        {
                                            HashSet<String> relatedNumbers = new HashSet<string>();
                                            if (accountRelatedNumbersDictionary.TryGetValue(accountNumberwithSameIMEI, out relatedNumbers))
                                            {
                                                if (!relatedNumbers.Contains(cdr.MSISDN))
                                                {
                                                    relatedNumbers.Add(cdr.MSISDN);
                                                    accountRelatedNumbersDictionary[accountNumberwithSameIMEI] = relatedNumbers;
                                                    accountRelatedNumbersToBeInserted.Add(new RelatedNumber { AccountNumber = accountNumberwithSameIMEI, RelatedAccountNumber = cdr.MSISDN });
                                                }
                                            }
                                            else
                                            {
                                                relatedNumbers = new HashSet<string>();
                                                relatedNumbers.Add(cdr.MSISDN);
                                                accountRelatedNumbersDictionary.Add(accountNumberwithSameIMEI, relatedNumbers);
                                                accountRelatedNumbersToBeInserted.Add(new RelatedNumber { AccountNumber = accountNumberwithSameIMEI, RelatedAccountNumber = cdr.MSISDN });
                                            }
                                        }
                                    }
                                }
                            }
                            cdrsCount += cdrs.Count;
                            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs Checked", cdrsCount);

                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });

            IRelatedNumberDataManager dataManager = FraudDataManagerFactory.GetDataManager<IRelatedNumberDataManager>();
            dataManager.SavetoDB(accountRelatedNumbersToBeInserted);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Loading CDRs from Database to Memory");

        }



        protected override FindRelatedNumbersthroughIMEIsInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new FindRelatedNumbersthroughIMEIsInput
            {
                InputQueue = this.InputQueue.Get(context),
                AccountsNumbersByIMEIDictionary = this.AccountNumbersByIMEIDictionary.Get(context),
                AccountRelatedNumbersDictionary = this.AccountRelatedNumbersDictionary.Get(context)
            };
        }

    }
}
