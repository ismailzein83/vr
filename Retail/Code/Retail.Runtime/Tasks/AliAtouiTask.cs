using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Common.Business;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace Retail.Runtime.Tasks
{
    class AliAtouiTask : ITask
    {
        public void Execute()
        {
            #region Runtime
            //ExecuteRuntime executeRuntime = new ExecuteRuntime();
            //executeRuntime.Runtime_Main();
            #endregion

            #region RetailAccountPropertyEvaluatorTask
            //RetailAccountPropertyEvaluatorTask retailAccountPropertyEvaluatorTask = new RetailAccountPropertyEvaluatorTask();
            //retailAccountPropertyEvaluatorTask.RetailAccountPropertyEvaluator_Main();
            #endregion

            //accountManager.GetAccountGridColumnAttributes();
            //accountManager.GetAccountViewRuntimeEditors();
        }
    }

    public class ExecuteRuntime
    {
        public void Runtime_Main()
        {
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

            Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bigDataService);

            TransactionLockRuntimeService transactionLockRuntimeService = new TransactionLockRuntimeService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(transactionLockRuntimeService);

            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            Console.ReadKey();
        }
    }

    public class RetailAccountPropertyEvaluatorTask
    {
        public void RetailAccountPropertyEvaluator_Main()
        {
            Console.WriteLine("RetailAccountPropertyEvaluator has started!!");

            Guid guid = new Guid("77e86d30-9bca-4b2b-bc78-44d89da772ba");

            Account account = new Account();
            account.Name = "Ali Atoui";
            account.StatusId = guid;
            account.TypeId = new Guid("77e86d30-9bca-4b2b-bc78-44d89da772bb");

            //AccountPartFinancial
            AccountPartFinancial accountPartFinancial = new AccountPartFinancial();
            accountPartFinancial.CurrencyId = 1025;

            //AccountPartResidentialProfile
            AccountPartResidentialProfile accountPartResidentialProfile = new AccountPartResidentialProfile();
            accountPartResidentialProfile.Email = "aliatoui@hotmail.com";

            account.Settings = new AccountSettings();
            account.Settings.Parts = new AccountPartCollection();
            account.Settings.Parts.Add(new Guid("82228BE2-E633-4EF8-B383-9894F28C8CB0"), new AccountPart { Settings = accountPartFinancial });
            account.Settings.Parts.Add(new Guid("05FECF19-6413-402F-BD65-64B0EEF1FB52"), new AccountPart { Settings = accountPartResidentialProfile });

            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
            objects.Add("Subscriber", account);

            VRMailManager vrMailManager = new VRMailManager();
            vrMailManager.SendMail(guid, objects);

            Console.WriteLine("RetailAccountPropertyEvaluator has ended!!");
            Console.ReadLine();
        }
    }

}


