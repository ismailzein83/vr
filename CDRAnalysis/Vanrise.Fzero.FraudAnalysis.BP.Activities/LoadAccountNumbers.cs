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
    #region Arguments Classes

    public class LoadAccountNumbersInput
    {
        public BaseQueue<AccountNumberBatch> OutputQueue { get; set; }
    }

    #endregion

    public sealed class LoadAccountNumbers : BaseAsyncActivity<LoadAccountNumbersInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<AccountNumberBatch>> OutputQueue { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<AccountNumberBatch>());

            base.OnBeforeExecute(context, handle);
        }

        static string configuredDirectory = ConfigurationManager.AppSettings["LoadAccountNumbersDirectory"];

        protected override void DoWork(LoadAccountNumbersInput inputArgument, AsyncActivityHandle handle)
        {
            List<String> numbers = new List<String>();
            int totalCount = 0;
            IStrategyExecutionDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            dataManager.LoadAccountNumbersfromStrategyExecutionDetails(0, (number) =>
                {
                    numbers.Add(number);
                    if (numbers.Count >= 10)
                    {
                        totalCount += numbers.Count;
                        inputArgument.OutputQueue.Enqueue(BuildAccountNumberBatch(numbers));
                        handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} Account Numbers loaded", totalCount);
                        numbers = new List<string>();
                    }
                });
            if (numbers.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(BuildAccountNumberBatch(numbers));
            }
        }

        private AccountNumberBatch BuildAccountNumberBatch(List<string> numbers)
        {
            var numbersBytes = Vanrise.Common.Compressor.Compress(Vanrise.Common.ProtoBufSerializer.Serialize(numbers));
            string filePath = !String.IsNullOrEmpty(configuredDirectory) ? System.IO.Path.Combine(configuredDirectory, Guid.NewGuid().ToString()) : System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllBytes(filePath, numbersBytes);
            return new AccountNumberBatch
            {
                AccountNumberBatchFilePath = filePath
            };
        }

        protected override LoadAccountNumbersInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadAccountNumbersInput
            {
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
