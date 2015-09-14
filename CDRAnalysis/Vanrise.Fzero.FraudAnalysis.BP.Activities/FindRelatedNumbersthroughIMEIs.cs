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
using System.Linq;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class FindRelatedNumbersthroughIMEIsInput
    {
        public BaseQueue<CDRBatch> InputQueue { get; set; }

        public AccountNumbersByIMEI AccountsNumbersByIMEI { get; set; }

    }

    #endregion

    public class FindRelatedNumbersthroughIMEIs : DependentAsyncActivity<FindRelatedNumbersthroughIMEIsInput>
    {

        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }

        public InArgument<AccountNumbersByIMEI> AccountNumbersByIMEI { get; set; }



        #endregion


        protected override void DoWork(FindRelatedNumbersthroughIMEIsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {


            AccountNumbersByIMEI accountNumbersByIMEI = inputArgument.AccountsNumbersByIMEI;

            IRelatedNumberDataManager dataManager = FraudDataManagerFactory.GetDataManager<IRelatedNumberDataManager>();

            AccountRelatedNumbers accountRelatedNumbers = new AccountRelatedNumbers();
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


                            HashSet<String> accountNumbers ;
                            HashSet<String> relatedNumbers ;


                            foreach (var cdr in cdrs.Where(x => x.IMEI != null && x.IMEI != "000000000000000"))
                            {
                                accountNumbers = new HashSet<string>();
                                relatedNumbers = new HashSet<string>();

                                if (accountNumbersByIMEI.TryGetValue(cdr.IMEI, out accountNumbers))
                                {
                                    foreach (var accountNumber in accountNumbers)
                                        if (accountNumber != cdr.MSISDN)
                                        {
                                            if (accountRelatedNumbers.TryGetValue(accountNumber, out relatedNumbers))
                                            {
                                                if (accountNumber != cdr.MSISDN)
                                                    relatedNumbers.Add(accountNumber);
                                                accountRelatedNumbers[cdr.MSISDN] = accountNumbers;
                                            }
                                            else
                                            {
                                                relatedNumbers = new HashSet<string>();
                                                if (accountNumber != cdr.MSISDN)
                                                    relatedNumbers.Add(cdr.MSISDN);
                                                accountRelatedNumbers.Add(accountNumber, relatedNumbers);
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

            foreach (KeyValuePair<string, HashSet<string>> dicItem in accountRelatedNumbers)
            {
                foreach (string relatedNumber in dicItem.Value.ToList())
                {
                    if (relatedNumber == dicItem.Key)
                        dicItem.Value.Remove(dicItem.Key);
                }
            }

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
