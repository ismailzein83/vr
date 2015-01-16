//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Activities;
//using TOne.Entities;
//using Vanrise.BusinessProcess;
//using TOne.Data;
//using System.Collections.Concurrent;
//using System.Threading;
//using System.Data;

//namespace TOne.LCRProcess.Activities
//{
//    #region Argument Classes

//    public class UpdateSupplierCodeMatchInDBInput
//    {
//        public bool IsFuture { get; set; }

//        public ConcurrentQueue<Tuple<string, List<CodeMatch>>> QueueSuppliersCodeMatches { get; set; }

//        public AsyncActivityStatus PreviousTaskStatus { get; set; }
//    }

//    #endregion

//    public sealed class UpdateSupplierCodeMatchInDB : BaseAsyncActivity<UpdateSupplierCodeMatchInDBInput>
//    {
//        [RequiredArgument]
//        public InArgument<bool> IsFuture { get; set; }

//        [RequiredArgument]
//        public InArgument<ConcurrentQueue<Tuple<string, List<CodeMatch>>>> QueueSuppliersCodeMatches { get; set; }

//        [RequiredArgument]
//        public InArgument<AsyncActivityStatus> PreviousTaskStatus { get; set; }

//        protected override UpdateSupplierCodeMatchInDBInput GetInputArgument(AsyncCodeActivityContext context)
//        {
//            return new UpdateSupplierCodeMatchInDBInput
//            {
//                IsFuture = this.IsFuture.Get(context),
//                QueueSuppliersCodeMatches = this.QueueSuppliersCodeMatches.Get(context),
//                PreviousTaskStatus = this.PreviousTaskStatus.Get(context)
//            };
//        }

//        protected override void DoWork(UpdateSupplierCodeMatchInDBInput inputArgument)
//        {
//            ICodeMatchDataManager dataManager = DataManagerFactory.GetDataManager<ICodeMatchDataManager>();
//            DataTable dtCodeMatches = dataManager.BuildCodeMatchSchemaTable(inputArgument.IsFuture);
//            //System.Threading.Tasks.Parallel.For(0, 10, (i) =>
//            //    {

//            while (!inputArgument.PreviousTaskStatus.IsComplete || inputArgument.QueueSuppliersCodeMatches.Count > 0)
//            {
//                Tuple<string, List<CodeMatch>> supplierCodeMatch;
//                while (inputArgument.QueueSuppliersCodeMatches.TryDequeue(out supplierCodeMatch))
//                {
//                    dataManager.AddCodeMatchesToTable(supplierCodeMatch.Item2, dtCodeMatches);
//                }

//                Thread.Sleep(1000);
//            }

//            Console.WriteLine("{0}: start writting {1} records to database", DateTime.Now, dtCodeMatches.Rows.Count);
//            DateTime start = DateTime.Now;
//            dataManager.WriteCodeMatchTableToDB(dtCodeMatches);
//            Console.WriteLine("{0}: writting {1} records to database is done in {2}", DateTime.Now, dtCodeMatches.Rows.Count, (DateTime.Now - start));

//            //});
//            //while (!inputArgument.PreviousTaskStatus.IsComplete || inputArgument.QueueSuppliersCodeMatches.Count > 0)
//            //{
//            //    if (inputArgument.QueueSuppliersCodeMatches.Count > 0)
//            //    {
//            //        ParallelIfNeeded(inputArgument.QueueSuppliersCodeMatches.Count, () =>
//            //        {
//            //            Tuple<string, List<SupplierCodeMatch>> supplierCodeMatch;
//            //            while (inputArgument.QueueSuppliersCodeMatches.TryDequeue(out supplierCodeMatch))
//            //            {
//            //                DateTime start = DateTime.Now;
//            //                dataManager.UpdateSupplierCodeMatches(supplierCodeMatch.Item1, inputArgument.IsFuture, supplierCodeMatch.Item2);
//            //                Console.WriteLine("{0}: writting {1} records to database for supplier {2} is done in {3}", DateTime.Now, supplierCodeMatch.Item2.Count, supplierCodeMatch.Item1, (DateTime.Now - start));
//            //            }
//            //        });
//            //    }

//            //    Thread.Sleep(1000);
//            //}     
//        }

//        void ParallelIfNeeded(int nbOfAction, Action action)
//        {
//            //if (nbOfAction == 1)
//            //    action();
//            //else
//            System.Threading.Tasks.Parallel.For(0, Math.Min(10, 10), (i) =>
//            {
//                action();
//            });
//        }
//    }
//}
