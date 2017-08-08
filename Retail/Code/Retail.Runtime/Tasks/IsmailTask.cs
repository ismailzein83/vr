using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace Retail.Runtime.Tasks
{
    public class IsmailTask : ITask
    {
      
        public void Execute()
        {
            //TestData testData = new TestData();
            //string originatorNumber = Console.ReadLine();
            //int batchSize = int.Parse(Console.ReadLine());
            //while (true)
            //{
            //    DateTime start = DateTime.Now;
            //    var cdrs = testData.ReadCDRs(true, batchSize, originatorNumber);
            //    Console.WriteLine("{0} CDRs read in {1} with top", cdrs.Count, DateTime.Now - start);
            //    cdrs = null;
            //    start = DateTime.Now;
            //    cdrs = testData.ReadCDRs(false, batchSize, originatorNumber);
            //    Console.WriteLine("{0} CDRs read in {1} without top", cdrs.Count, DateTime.Now - start);
            //    cdrs = null;
            //    originatorNumber = Console.ReadLine();
            //    batchSize = int.Parse(Console.ReadLine());
            //}

            Vanrise.Common.Business.OverriddenConfigurationManager overriddenConfigurationManager = new Vanrise.Common.Business.OverriddenConfigurationManager();
            var zajilScript = overriddenConfigurationManager.GenerateOverriddenConfigurationGroupScript(new Guid("CF1EAF73-93DB-416F-92FC-F8C0E7EE6EA7"));
            var multinetScript = overriddenConfigurationManager.GenerateOverriddenConfigurationGroupScript(new Guid("D79E9957-3EA5-49C1-AEDB-15F251BEDCDC"));
            var devScript = overriddenConfigurationManager.GenerateOverriddenConfigurationDevScript();
            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService);

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(schedulerService);

            //Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(bigDataService);

            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            Console.ReadKey();

            //Vanrise.Security.Entities.ContextFactory.GetContext().SetContextUserId(1);
            //var accountManager = new Retail.BusinessEntity.Business.AccountManager();
            //IAccountPayment dummy;
            //var financialAccounts = accountManager.GetCachedAccounts().Values.Where(a => accountManager.HasAccountPayment(a.AccountId, false, out dummy));
            //foreach(var a in financialAccounts)
            //{
            //    Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            //    try
            //    {
            //        invoiceManager.GenerateInvoice(new Vanrise.Invoice.Entities.GenerateInvoiceInput
            //                {
            //                    InvoiceTypeId = new Guid("384c819d-6e21-4e9a-9f08-11c7b81ad329"),
            //                    PartnerId =  a.AccountId.ToString(),
            //                    IssueDate = DateTime.Today,
            //                    FromDate = DateTime.Parse("2016-11-01"),
            //                    ToDate = DateTime.Parse("2016-12-01")
            //                }
            //                );
            //    }
            //catch(Vanrise.Invoice.Entities.InvoiceGeneratorException ex)
            //    {
            //        Console.WriteLine("Invoice has no data");
            //    }
            //}

            Console.WriteLine("DONE");
            Console.ReadKey();
        }

        private class TestData : Vanrise.Data.SQL.BaseSQLDataManager
        {
            protected override string GetConnectionString()
            {
                return @"Server=192.168.110.185\MSSQL2014;Database=ZajilBI_Online_CDR;Trusted_Connection=True;";
            }

            public List<CDR> ReadCDRs(bool withTop, int nbOfCDRs, string originatorNumber)
            {
                List<CDR> cdrs = new List<CDR>();
                StringBuilder query = new StringBuilder(sql);
                if (withTop)
                {
                    query.Replace("#TOP#", "Top " + nbOfCDRs.ToString());
                    query.Replace("#FILTER#", " AND OriginatorNumber = '" + originatorNumber + "'");
                }
                else
                {
                    query.Replace("#TOP#", "");
                    query.Replace("#FILTER#", " ");
                }
                ExecuteReaderText(query.ToString(), (reader) =>
                {
                    int index = 0;
                    while (reader.Read())
                    {
                        if (!withTop)
                        {
                            if (reader["OriginatorNumber"] as string != originatorNumber)
                                continue;
                        }
                        cdrs.Add(new CDR
                        {
                            ID = reader["ID"],
                            Call_Id = reader["Call_Id"],
                            ConnectDateTime = reader["ConnectDateTime"],
                            DisconnectDateTime = reader["DisconnectDateTime"],
                            DurationInSeconds = reader["DurationInSeconds"],
                            DisconnectReason = reader["DisconnectReason"],
                            CallProgressState = reader["CallProgressState"],
                            Account = reader["Account"],
                            OriginatorId = reader["OriginatorId"],
                            OriginatorNumber = reader["OriginatorNumber"],
                            OriginatorFromNumber = reader["OriginatorFromNumber"],
                            OriginalDialedNumber = reader["OriginalDialedNumber"],
                            TerminatorId = reader["TerminatorId"],
                            TerminatorNumber = reader["TerminatorNumber"],
                            IncomingGwId = reader["IncomingGwId"],
                            OutgoingGwId = reader["OutgoingGwId"],
                            TransferredCall_Id = reader["TransferredCall_Id"],
                            OriginatorIP = reader["OriginatorIP"],
                            TerminatorIP = reader["TerminatorIP"],
                            AttemptDateTime = reader["AttemptDateTime"],
                            FileName = reader["FileName"],
                            QueueItemId = reader["QueueItemId"],
                            InitiationCallType = reader["InitiationCallType"],
                            TerminationCallType = reader["TerminationCallType"]
                        });
                        index++;
                        if (index == nbOfCDRs)
                            break;
                    }
                }, null);
                return cdrs;
            }

            const string sql = @"SELECT #TOP# [ID]
      ,[Call_Id]
      ,[ConnectDateTime]
      ,[DisconnectDateTime]
      ,[DurationInSeconds]
      ,[DisconnectReason]
      ,[CallProgressState]
      ,[Account]
      ,[OriginatorId]
      ,[OriginatorNumber]
      ,[OriginatorFromNumber]
      ,[OriginalDialedNumber]
      ,[TerminatorId]
      ,[TerminatorNumber]
      ,[IncomingGwId]
      ,[OutgoingGwId]
      ,[TransferredCall_Id]
      ,[OriginatorIP]
      ,[TerminatorIP]
      ,[AttemptDateTime]
      ,[FileName]
      ,[QueueItemId]
      ,[InitiationCallType]
      ,[TerminationCallType]
  FROM[Retail_CDR].[CDR]  
where [AttemptDateTime] > '2016-11-15' and [AttemptDateTime] < '2016-11-20'
#FILTER#
order by [AttemptDateTime] desc";
        }

        public class CDR
        {
            public Object ID { get; set; }
            public Object Call_Id { get; set; }
            public Object ConnectDateTime { get; set; }
            public Object DisconnectDateTime { get; set; }
            public Object DurationInSeconds { get; set; }
            public Object DisconnectReason { get; set; }
            public Object CallProgressState { get; set; }
            public Object Account { get; set; }
            public Object OriginatorId { get; set; }
            public Object OriginatorNumber { get; set; }
            public Object OriginatorFromNumber { get; set; }
            public Object OriginalDialedNumber { get; set; }
            public Object TerminatorId { get; set; }
            public Object TerminatorNumber { get; set; }
            public Object IncomingGwId { get; set; }
            public Object OutgoingGwId { get; set; }
            public Object TransferredCall_Id { get; set; }
            public Object OriginatorIP { get; set; }
            public Object TerminatorIP { get; set; }
            public Object AttemptDateTime { get; set; }
            public Object FileName { get; set; }
            public Object QueueItemId { get; set; }
            public Object InitiationCallType { get; set; }
            public Object TerminationCallType { get; set; }
        }
    }
    
}
