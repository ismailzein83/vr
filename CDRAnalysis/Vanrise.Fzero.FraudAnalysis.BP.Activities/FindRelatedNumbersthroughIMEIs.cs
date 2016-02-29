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

        public AccountNumbersByIMEIDictionary AccountsNumbersByIMEI { get; set; }

    }

    #endregion

    public class FindRelatedNumbersthroughIMEIs : DependentAsyncActivity<FindRelatedNumbersthroughIMEIsInput>
    {

        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }

        public InArgument<AccountNumbersByIMEIDictionary> AccountNumbersByIMEI { get; set; }



        #endregion


        protected override void DoWork(FindRelatedNumbersthroughIMEIsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            AccountNumbersByIMEIDictionary accountNumbersByIMEI = inputArgument.AccountsNumbersByIMEI;
            AccountRelatedNumbersDictionary accountRelatedNumbers = new AccountRelatedNumbersDictionary();
            int cdrsCount = 0;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (cdrBatch) =>
                        {
                            //var serializedCDRs = Vanrise.Common.Compressor.Decompress(System.IO.File.ReadAllBytes(cdrBatch.CDRBatchFilePath));
                            //System.IO.File.Delete(cdrBatch.CDRBatchFilePath);
                            var cdrs = cdrBatch.CDRs;
                            
                            foreach (var cdr in cdrs)
                            {
                                if (cdr.IMEI == null)
                                    continue;
                                HashSet<String> accountNumbers ;
                                if (accountNumbersByIMEI.TryGetValue(cdr.IMEI, out accountNumbers))
                                {
                                    foreach (var accountNumber in accountNumbers)
                                    {
                                        if (accountNumber != cdr.MSISDN)
                                        {
                                            HashSet<String> relatedNumbers = accountRelatedNumbers.GetOrCreateItem(accountNumber);
                                            relatedNumbers.Add(cdr.MSISDN);
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
            dataManager.CreateTempTable();
            dataManager.SavetoDB(accountRelatedNumbers);
            dataManager.SwapTableWithTemp();

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Loading CDRs from Database to Memory");

        }



        protected override FindRelatedNumbersthroughIMEIsInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new FindRelatedNumbersthroughIMEIsInput
            {
                InputQueue = this.InputQueue.Get(context),
                AccountsNumbersByIMEI = this.AccountNumbersByIMEI.Get(context)
            };
        }

    }
}
