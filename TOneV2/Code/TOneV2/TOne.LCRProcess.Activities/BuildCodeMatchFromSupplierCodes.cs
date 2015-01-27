using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.Entities;
using System.Collections.Concurrent;
using System.Threading;
using System.Data;
using TOne.LCR.Data;
using TOne.LCR.Entities;

namespace TOne.LCRProcess.Activities
{
     #region Argument Classes

    public class BuildCodeMatchFromSupplierCodesInput
    {
        public CodeList DistinctCodes { get; set; }

        public TOneQueue<Tuple<string, List<LCRCode>>> InputQueue { get; set; }

        public TOneQueue<List<CodeMatch>> OutputQueue { get; set; }
    }

    #endregion

    public sealed class BuildCodeMatchFromSupplierCodes : DependentAsyncActivity<BuildCodeMatchFromSupplierCodesInput>
    {
        [RequiredArgument]
        public InArgument<CodeList> DistinctCodes { get; set; }

        [RequiredArgument]
        public InArgument<TOneQueue<Tuple<string, List<LCRCode>>>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<TOneQueue<List<CodeMatch>>> OutputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new TOneQueue<List<CodeMatch>>());
            base.OnBeforeExecute(context, handle);
        }

        protected override BuildCodeMatchFromSupplierCodesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new BuildCodeMatchFromSupplierCodesInput
            {
                 DistinctCodes = this.DistinctCodes.Get(context),
                 InputQueue = this.InputQueue.Get(context),
                 OutputQueue = this.OutputQueue.Get(context)
            };
        }
        protected override void DoWork(BuildCodeMatchFromSupplierCodesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            TimeSpan totalTime = default(TimeSpan);            
            List<CodeMatch> codeMatches = new List<CodeMatch>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (supplierCodes) =>
                        {
                            DateTime start = DateTime.Now;
                            BuildAndAddCodeMatchesToTable(codeMatches, inputArgument.DistinctCodes.CodesWithPossibleMatches, supplierCodes);
                            totalTime += (DateTime.Now - start);
                            if (codeMatches.Count > 50000)
                            {
                                inputArgument.OutputQueue.Enqueue(codeMatches);
                                codeMatches = new List<CodeMatch>();
                            }
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
            if (codeMatches.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(codeMatches);
            }
            Console.WriteLine("{0}: BuildCodeMatchFromSupplierCodes is done in {1} ", DateTime.Now, totalTime);
        }

        private void BuildAndAddCodeMatchesToTable(List<CodeMatch> readyCodeMatches, Dictionary<string, List<string>> distinctCodesWithPossibleMatches, Tuple<string, List<LCRCode>> supplierCodes)
        {
            DateTime start = DateTime.Now;
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();

            Dictionary<string, LCRCode> dicSupplierCodes = new Dictionary<string, LCRCode>();

            foreach (var supplierCode in supplierCodes.Item2)
            {
                if (!dicSupplierCodes.ContainsKey(supplierCode.Value))
                {
                    dicSupplierCodes.Add(supplierCode.Value, supplierCode);
                }
            }
            int count = 0;

            foreach (var distinctCode in distinctCodesWithPossibleMatches)
            {
                foreach (var possibleMatch in distinctCode.Value)
                {
                    LCRCode longestMatchSupplierCode;
                    if (dicSupplierCodes.TryGetValue(possibleMatch, out longestMatchSupplierCode))
                    {
                        readyCodeMatches.Add(new CodeMatch
                            {
                                Code = distinctCode.Key,
                                SupplierId = longestMatchSupplierCode.SupplierId,
                                SupplierCode = longestMatchSupplierCode.Value,
                                SupplierCodeId = longestMatchSupplierCode.ID,
                                SupplierZoneId = longestMatchSupplierCode.ZoneId
                            });
                        count++;
                        break;
                    }
                }
            }

            Console.WriteLine("{0}: {1} code Match ready for supplier {2} in {3}", DateTime.Now, count, supplierCodes.Item1, (DateTime.Now - start));
        }

    }
}
