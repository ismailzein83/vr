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
            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            //QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(queueRegulatorService);

            //QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(queueActivationService);

            //SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(summaryQueueActivationService);

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(schedulerService);

            //Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(bigDataService);

            //Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(dsRuntimeService);

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
    }
}
