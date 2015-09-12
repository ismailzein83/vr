using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class FindRelatedNumbersthroughIMEIsInput
    {
        public BaseQueue<CDRBatch> InputQueue { get; set; }

        public BaseQueue<AccountsRelatedNumbersBatch> OutputQueue { get; set; }

        public AccountNumbersByIMEI AccountsNumbersByIMEI { get; set; }

    }

    #endregion

    public class FindRelatedNumbersthroughIMEIs : DependentAsyncActivity<FindRelatedNumbersthroughIMEIsInput>
    {

        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<AccountsRelatedNumbersBatch>> OutputQueue { get; set; }

        public InArgument<AccountNumbersByIMEI> AccountNumbersByIMEI { get; set; }

      

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<AccountsRelatedNumbersBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(FindRelatedNumbersthroughIMEIsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

                 AccountNumbersByIMEI accountNumbersByIMEI = new AccountNumbersByIMEI();

                INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();

                AccountRelatedNumbers accountRelatedNumbers= new AccountRelatedNumbers();
                int cdrsCount = 0;
                DoWhilePreviousRunning(previousActivityStatus, handle, () =>
                {
                    bool hasItem = false;
                    do
                    {

                        hasItem = inputArgument.InputQueue.TryDequeue(
                            (cdrBatch) =>
                            {
                                var serializedCDRs = Vanrise.Common.Compressor.Decompress(System.IO.File.ReadAllBytes(cdrBatch.CDRBatchFilePath));
                                System.IO.File.Delete(cdrBatch.CDRBatchFilePath);
                                var cdrs = Vanrise.Common.ProtoBufSerializer.Deserialize<List<CDR>>(serializedCDRs);
                                foreach (var cdr in cdrs)
                                {

                                    HashSet<String> accountNumbers;
                                        if (accountNumbersByIMEI.TryGetValue(cdr.IMEI, out accountNumbers))
                                        {
                                            HashSet<String> relatedNumbers;
                                            if (accountRelatedNumbers.TryGetValue(cdr.MSISDN, out relatedNumbers))
                                            {
                                                foreach (var accountNumber in accountNumbers)
                                                    relatedNumbers.Add(accountNumber);
                                                accountRelatedNumbers[cdr.MSISDN] = accountNumbers;
                                            }
                                            else
                                            {
                                                relatedNumbers = new HashSet<string>();
                                                foreach (var accountNumber in accountNumbers)
                                                    relatedNumbers.Add(accountNumber);
                                                accountRelatedNumbers.Add(cdr.MSISDN, accountNumbers);
                                            }
                                        }
                                }
                                cdrsCount += cdrs.Count;
                                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs Checked", cdrsCount);

                            });
                    }
                    while (!ShouldStop(handle) && hasItem);
                });
               

                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Loading CDRs from Database to Memory");

           
        }


        protected override FindRelatedNumbersthroughIMEIsInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new FindRelatedNumbersthroughIMEIsInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                AccountsNumbersByIMEI = this.AccountNumbersByIMEI.Get(context)
            };
        }

    }
}
