//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Activities;
//using Vanrise.BusinessProcess;
//using TOne.Entities;
//using System.Collections.Concurrent;
//using System.Threading;
//using System.Data;
//using TOne.Data;

//namespace TOne.LCRProcess.Activities
//{
//     #region Argument Classes

//    public class BuildCodeMatchFromOrderedSupplierCodesInput
//    {
//        public CodeTree DistinctCodes { get; set; }

//        public bool IsFuture { get; set; }

//        public Dictionary<string, Dictionary<string, LCRCode>> OrderedSuppliersCodes { get; set; }

//        public ConcurrentQueue<DataTable> QueueReadyCodeMatchTables { get; set; }
//    }

//    #endregion

//    public sealed class BuildCodeMatchFromOrderedSupplierCodes : BaseAsyncActivity<BuildCodeMatchFromOrderedSupplierCodesInput>
//    {
//        [RequiredArgument]
//        public InArgument<CodeTree> DistinctCodes { get; set; }

//        [RequiredArgument]
//        public InArgument<bool> IsFuture { get; set; }

//        [RequiredArgument]
//        public InArgument<Dictionary<string, Dictionary<string, LCRCode>>> OrderedSuppliersCodes { get; set; }

//        [RequiredArgument]
//        public InArgument<ConcurrentQueue<DataTable>> QueueReadyCodeMatchTables { get; set; }

//        protected override BuildCodeMatchFromOrderedSupplierCodesInput GetInputArgument(AsyncCodeActivityContext context)
//        {
//            return new BuildCodeMatchFromOrderedSupplierCodesInput
//            {
//                 DistinctCodes = this.DistinctCodes.Get(context),
//                 IsFuture = this.IsFuture.Get(context),
//                 OrderedSuppliersCodes = this.OrderedSuppliersCodes.Get(context),
//                 QueueReadyCodeMatchTables = this.QueueReadyCodeMatchTables.Get(context)
//            };
//        }

//        protected override void DoWork(BuildCodeMatchFromOrderedSupplierCodesInput inputArgument)
//        {
//            ICodeMatchDataManager dataManager = DataManagerFactory.GetDataManager<ICodeMatchDataManager>();
//            ConcurrentQueue<string> queueDistinctCodes = new ConcurrentQueue<string>(inputArgument.DistinctCodes.MatchCodes);
//            System.Threading.Tasks.Parallel.For(0, 25, (i) =>
//                {
//                    DataTable dtCodeMatches = dataManager.BuildCodeMatchSchemaTable(inputArgument.IsFuture);
//                    string distinctCode;
//                    while (queueDistinctCodes.TryDequeue(out distinctCode))
//                    {
//                        BuildAndAddCodeMatchesToTable(dtCodeMatches, distinctCode, inputArgument.IsFuture, inputArgument.OrderedSuppliersCodes);
//                        if (dtCodeMatches.Rows.Count > 10000)
//                        {
//                            inputArgument.QueueReadyCodeMatchTables.Enqueue(dtCodeMatches);
//                            dtCodeMatches = dataManager.BuildCodeMatchSchemaTable(inputArgument.IsFuture);
//                        }
//                    }
//                    if (dtCodeMatches.Rows.Count > 0)
//                    {
//                        inputArgument.QueueReadyCodeMatchTables.Enqueue(dtCodeMatches);
//                    }
//                });      
//        }

//        private static void BuildAndAddCodeMatchesToTable(DataTable dtCodeMatches, string distinctCode, bool isFuture, Dictionary<string, Dictionary<string, LCRCode>> orderedSuppliersCodes)
//        {
//            //Thread.Sleep(10);
//            DateTime start = DateTime.Now;
//            ICodeMatchDataManager dataManager = DataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            
//            int count = 0;
//            foreach (KeyValuePair<string, Dictionary<string, LCRCode>> supplierCodes in orderedSuppliersCodes)
//            {
//                foreach (var supplierCode in supplierCodes.Value.Keys)
//                {
//                    if (distinctCode.StartsWith(supplierCode))
//                    {
//                        LCRCode longestMatchSupplierCode = supplierCodes.Value[supplierCode];
//                        dataManager.AddCodeMatchRowToTable(dtCodeMatches, distinctCode, longestMatchSupplierCode.SupplierId, longestMatchSupplierCode.Value, longestMatchSupplierCode.ID, longestMatchSupplierCode.ZoneId);
//                        count++;
//                        break;
//                    }
//                }
//            }
//            Console.WriteLine("{0}: {1} code Match ready in {2}", DateTime.Now, count, (DateTime.Now - start));
//        }

        

//    }
//}
