using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;
using System.ServiceModel;
using TOne.LCR.Data;
using TOne.Entities;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes

    public class WriteCodeMatchDataTablesToDBInput
    {
        public ConcurrentQueue<DataTable> QueueCodeMatchTables { get; set; }

        public AsyncActivityStatus PreviousTaskStatus { get; set; }
    }

    #endregion

    public sealed class WriteCodeMatchDataTablesToDB : BaseAsyncActivity<WriteCodeMatchDataTablesToDBInput>
    {
        [RequiredArgument]
        public InArgument<ConcurrentQueue<DataTable>> QueueCodeMatchTables { get; set; }

        [RequiredArgument]
        public InArgument<AsyncActivityStatus> PreviousTaskStatus { get; set; }

        protected override WriteCodeMatchDataTablesToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new WriteCodeMatchDataTablesToDBInput
            {
                QueueCodeMatchTables = this.QueueCodeMatchTables.Get(context),
                PreviousTaskStatus = this.PreviousTaskStatus.Get(context)
            };
        }

        protected override void DoWork(WriteCodeMatchDataTablesToDBInput inputArgument, AsyncActivityHandle handle)
        {
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            TimeSpan totalTime = default(TimeSpan);
            while (!inputArgument.PreviousTaskStatus.IsComplete || inputArgument.QueueCodeMatchTables.Count > 0)
            {
                DataTable dtCodeMatches;
                while (inputArgument.QueueCodeMatchTables.TryDequeue(out dtCodeMatches))
                {
                    //Console.WriteLine("{0}: start writting {1} records to database", DateTime.Now, dtCodeMatches.Rows.Count);
                    DateTime start = DateTime.Now;
                    //NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                    //binding.OpenTimeout = TimeSpan.FromMinutes(5);
                    //binding.CloseTimeout = TimeSpan.FromMinutes(5);
                    //binding.SendTimeout = TimeSpan.FromMinutes(5);
                    //binding.ReceiveTimeout = TimeSpan.FromMinutes(5);
                    //binding.MaxBufferPoolSize = int.MaxValue;
                    //binding.MaxBufferSize = int.MaxValue;
                    //binding.MaxReceivedMessageSize = int.MaxValue;
                    
                    //ChannelFactory<IBulkTableService> channelFactory = new ChannelFactory<IBulkTableService>(binding, new EndpointAddress("net.pipe://localhost/BulkTableService"));
                    //IBulkTableService client = channelFactory.CreateChannel();
                    //client.WriteCodeMatchTable(dtCodeMatches);
                    //(client as IDisposable).Dispose();
                    dataManager.WriteCodeMatchTableToDB(dtCodeMatches);
                    totalTime += (DateTime.Now - start);
                    Console.WriteLine("{0}: writting {1} records to database is done in {2}", DateTime.Now, dtCodeMatches.Rows.Count, (DateTime.Now - start));

                }

                Thread.Sleep(1000);
            }
            Console.WriteLine("{0}: WriteCodeMatchDataTablesToDB is done in {1}", DateTime.Now, totalTime);
        }
    }
}
