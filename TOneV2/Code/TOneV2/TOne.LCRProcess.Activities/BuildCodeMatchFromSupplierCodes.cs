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

        public ConcurrentQueue<Tuple<string, List<LCRCode>>> InputQueue { get; set; }

        public ConcurrentQueue<List<CodeMatch>> OutputQueue { get; set; }
    }

    #endregion

    public sealed class BuildCodeMatchFromSupplierCodes : DependentAsyncActivity<BuildCodeMatchFromSupplierCodesInput>
    {
        [RequiredArgument]
        public InArgument<CodeTree> DistinctCodes { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<char> FirstDigit { get; set; }

        [RequiredArgument]
        public InArgument<ConcurrentQueue<Tuple<string, List<LCRCode>>>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<ConcurrentQueue<List<CodeMatch>>> OutputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new ConcurrentQueue<List<CodeMatch>>());
            base.OnBeforeExecute(context, handle);
        }

        protected override BuildCodeMatchFromSupplierCodesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new BuildCodeMatchFromSupplierCodesInput
            {
                 DistinctCodes = this.DistinctCodes.Get(context),
                 IsFuture = this.IsFuture.Get(context),
                 FirstDigit = this.FirstDigit.Get(context),
                 InputQueue = this.InputQueue.Get(context),
                 OutputQueue = this.OutputQueue.Get(context)
            };
        }

        static object s_lockObj = new object();
        protected override void DoWork(BuildCodeMatchFromSupplierCodesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            foreach (var fileInfo in System.IO.Directory.GetFiles(@"C:\CodeMatch"))
            {
                System.IO.File.Delete(fileInfo);
            }
            //ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            TimeSpan totalTime = default(TimeSpan);
            //System.Threading.Tasks.Parallel.For(0, 2, (i) =>
            //    {
            //DataTable dtCodeMatches = dataManager.BuildCodeMatchSchemaTable(inputArgument.IsFuture);
            //string fileName = Guid.NewGuid().ToString();

            List<CodeMatch> codeMatches = new List<CodeMatch>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                Tuple<string, List<LCRCode>> supplierCodes;
                while (!ShouldStop(handle) && inputArgument.InputQueue.TryDequeue(out supplierCodes))
                {
                    //while (inputArgument.QueueReadyCodeMatches.Count > 4)
                    //    Thread.Sleep(1000);
                    DateTime start = DateTime.Now;
                    BuildAndAddCodeMatchesToTable(codeMatches, inputArgument.DistinctCodes.CodesWithPossibleMatches, supplierCodes);
                    lock (s_lockObj)
                        totalTime += (DateTime.Now - start);
                    if (codeMatches.Count > 50000)
                    {
                        inputArgument.OutputQueue.Enqueue(codeMatches);
                        codeMatches = new List<CodeMatch>();
                        //dtCodeMatches = dataManager.BuildCodeMatchSchemaTable(inputArgument.IsFuture);
                    }
                }
            });
            if (codeMatches.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(codeMatches);
            }
            //});
            Console.WriteLine("{0}: BuildCodeMatchFromSupplierCodes is done in {1} ", DateTime.Now, totalTime);
        }

        private void BuildAndAddCodeMatchesToTable(List<CodeMatch> readyCodeMatches, Dictionary<string, List<string>> distinctCodesWithPossibleMatches, Tuple<string, List<LCRCode>> supplierCodes)
        {            
            DateTime start = DateTime.Now;
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();

            //System.Collections.Hashtable tableSupplierCodes = new System.Collections.Hashtable();
            
            Dictionary<string, LCRCode> dicSupplierCodes = new Dictionary<string, LCRCode>();
            //HashSet<string> supplierCodesSet = new HashSet<string>();
            foreach (var supplierCode in supplierCodes.Item2)
            {
                if (!dicSupplierCodes.ContainsKey(supplierCode.Value))
                {
                    dicSupplierCodes.Add(supplierCode.Value, supplierCode);
                    //supplierCodesSet.Add(supplierCode.Value);
                }
            }
            //CodeTree treeSupplierCodes = new CodeTree(dicSupplierCodes.Keys.ToList());
            int count = 0;
            //string filePath = String.Format(@"C:\CodeMatch\{0}.txt", fileName);
            //using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath, true))
            //{
                foreach (var distinctCode in distinctCodesWithPossibleMatches)
                {
                    foreach (var possibleMatch in distinctCode.Value)
                    {
                        LCRCode longestMatchSupplierCode;
                        if (dicSupplierCodes.TryGetValue(possibleMatch, out longestMatchSupplierCode))
                        {
                            //LCRCode longestMatchSupplierCode = dicSupplierCodes[possibleMatch];
                            //wr.WriteLine(String.Format("{0},{1},{2},{3},{4}", distinctCode.Key, supplierCodes.Item1, longestMatchSupplierCode.Value, longestMatchSupplierCode.ID, longestMatchSupplierCode.ZoneId));
                            //dataManager.AddCodeMatchRowToTable(dtCodeMatches, distinctCode.Key, supplierCodes.Item1, longestMatchSupplierCode.Value, longestMatchSupplierCode.ID, longestMatchSupplierCode.ZoneId);
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
                    ////string longestMatchCode = treeSupplierCodes.GetLongestMatch(distinctCode);
                    //int codeIndex = supplierCodes.Item2.BinarySearch(new LCRCode { Value = distinctCode }, comparer);
                    //if (codeIndex > -1)// (!String.IsNullOrEmpty(longestMatchCode))
                    //{
                    //    LCRCode longestMatchSupplierCode = supplierCodes.Item2[codeIndex];
                    //    //dataManager.AddCodeMatchRowToTable(dtCodeMatches, distinctCode, supplierCodes.Item1, longestMatchSupplierCode.Value, longestMatchSupplierCode.ID, longestMatchSupplierCode.ZoneId);
                    //    count++;
                    //}
                }
            //    wr.Close();
            //}

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

            Console.WriteLine("{0}: {1} code Match ready for supplier {2} in {3}", DateTime.Now, count, supplierCodes.Item1, (DateTime.Now - start));
        }

        private class CodeMatchComparer : IComparer<LCRCode>
        {
            public int Compare(LCRCode x, LCRCode y)
            {
                if (y.Value.StartsWith(x.Value))
                    return 0;
                else return -1;
            }
        }

    }
}
