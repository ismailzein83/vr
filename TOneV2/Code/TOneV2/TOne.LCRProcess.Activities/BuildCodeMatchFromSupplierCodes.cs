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
        public CodeTree DistinctCodes { get; set; }

        public bool IsFuture { get; set; }

        public char FirstDigit { get; set; }

        public ConcurrentQueue<Tuple<string, List<LCRCode>>> QueueReadySuppliersCodes { get; set; }

        public ConcurrentQueue<DataTable> QueueReadyCodeMatchTables { get; set; }

        public AsyncActivityStatus PreviousTaskStatus { get; set; }
    }

    #endregion

    public sealed class BuildCodeMatchFromSupplierCodes : BaseAsyncActivity<BuildCodeMatchFromSupplierCodesInput>
    {
        [RequiredArgument]
        public InArgument<CodeTree> DistinctCodes { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<char> FirstDigit { get; set; }

        [RequiredArgument]
        public InArgument<ConcurrentQueue<Tuple<string, List<LCRCode>>>> QueueReadySuppliersCodes { get; set; }

        [RequiredArgument]
        public InArgument<ConcurrentQueue<DataTable>> QueueReadyCodeMatchTables { get; set; }

        [RequiredArgument]
        public InArgument<AsyncActivityStatus> PreviousTaskStatus { get; set; }

        protected override BuildCodeMatchFromSupplierCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new BuildCodeMatchFromSupplierCodesInput
            {
                 DistinctCodes = this.DistinctCodes.Get(context),
                 IsFuture = this.IsFuture.Get(context),
                 FirstDigit = this.FirstDigit.Get(context),
                 QueueReadySuppliersCodes = this.QueueReadySuppliersCodes.Get(context),
                 QueueReadyCodeMatchTables = this.QueueReadyCodeMatchTables.Get(context),
                 PreviousTaskStatus = this.PreviousTaskStatus.Get(context)
            };
        }

        static object s_lockObj = new object();
        protected override void DoWork(BuildCodeMatchFromSupplierCodesInput inputArgument)
        {
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            TimeSpan totalTime = default(TimeSpan);
            System.Threading.Tasks.Parallel.For(0, 3, (i) =>
                {
                    DataTable dtCodeMatches = dataManager.BuildCodeMatchSchemaTable(inputArgument.IsFuture);
                    while (!inputArgument.PreviousTaskStatus.IsComplete || inputArgument.QueueReadySuppliersCodes.Count > 0)
                    {
                        Tuple<string, List<LCRCode>> supplierCodes;
                        while (inputArgument.QueueReadySuppliersCodes.TryDequeue(out supplierCodes))
                        {
                            while (inputArgument.QueueReadyCodeMatchTables.Count > 2)
                                Thread.Sleep(1000);
                            DateTime start = DateTime.Now;
                            BuildAndAddCodeMatchesToTable(dtCodeMatches, inputArgument.DistinctCodes.MatchCodes, supplierCodes);
                            lock (s_lockObj)
                                totalTime += (DateTime.Now - start);
                            if (dtCodeMatches.Rows.Count > 500000)
                            {
                                inputArgument.QueueReadyCodeMatchTables.Enqueue(dtCodeMatches);
                                dtCodeMatches = dataManager.BuildCodeMatchSchemaTable(inputArgument.IsFuture);
                            }
                        }
                        Thread.Sleep(1000);
                    }
                    if (dtCodeMatches.Rows.Count > 0)
                    {
                        inputArgument.QueueReadyCodeMatchTables.Enqueue(dtCodeMatches);
                    }
                });
            Console.WriteLine("{0}: BuildCodeMatchFromSupplierCodes is done in {1} ", DateTime.Now, totalTime);
        }

        private static void BuildAndAddCodeMatchesToTable(DataTable dtCodeMatches, List<string> distinctCodes, Tuple<string, List<LCRCode>> supplierCodes)
        {            
            DateTime start = DateTime.Now;
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            
            Dictionary<string, LCRCode> dicSupplierCodes = new Dictionary<string, LCRCode>();
            foreach (var supplierCode in supplierCodes.Item2)
            {
                if (!dicSupplierCodes.ContainsKey(supplierCode.Value))
                    dicSupplierCodes.Add(supplierCode.Value, supplierCode);
            }
            CodeTree treeSupplierCodes = new CodeTree(dicSupplierCodes.Keys.ToList());
            int count = 0;
            foreach (var distinctCode in distinctCodes)
            {
                string longestMatchCode = treeSupplierCodes.GetLongestMatch(distinctCode);
                if (!String.IsNullOrEmpty(longestMatchCode))
                {
                    LCRCode longestMatchSupplierCode = dicSupplierCodes[longestMatchCode];
                    dataManager.AddCodeMatchRowToTable(dtCodeMatches, distinctCode, supplierCodes.Item1, longestMatchSupplierCode.Value, longestMatchSupplierCode.ID, longestMatchSupplierCode.ZoneId);
                    count++;
                }
            }


            //foreach (var supplierCode in supplierCodes.Item2)
            //{
            //    List<string> distinctCodesThatMatch = inputArgument.DistinctCodes.GetCodesThatMatch(supplierCode.Value);
            //    if (distinctCodesThatMatch != null && distinctCodesThatMatch.Count > 0)
            //    {
            //        foreach (var distinctCode in distinctCodesThatMatch)
            //        {
            //            CodeMatch codeMatch = new CodeMatch
            //            {
            //                Code = distinctCode,
            //                SupplierId = supplierCodes.Item1,
            //                SupplierCode = supplierCode.Value,
            //                SupplierCodeId = supplierCode.ID,
            //                SupplierZoneId = supplierCode.ZoneId,
            //                IsFuture = inputArgument.IsFuture
            //            };
            //            codeMatches.Add(codeMatch);
            //        }
            //    }
            //}

            //var orderedCodeMatch = codeMatches.OrderByDescending(itm => String.Concat(itm.Code, "||", itm.SupplierCode));
            //var finalCodeMatchList = new List<CodeMatch>();

            //CodeMatch current = null;
            //foreach (var codeMatch in orderedCodeMatch)
            //{
            //    if (current == null || String.Compare(codeMatch.Code, current.Code, true) != 0)
            //    {
            //        current = codeMatch;
            //        finalCodeMatchList.Add(current);
            //    }
            //}
            //inputArgument.QueueSuppliersCodeMatches.Enqueue(finalCodeMatchList);
            
            //Console.WriteLine("{0}: {1} code Match ready for supplier {2} in {3}", DateTime.Now, count, supplierCodes.Item1, (DateTime.Now - start));
        }

        

    }
}
