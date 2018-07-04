using Retail.Ringo.ProxyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Voucher.Business;

namespace Retail.Runtime.Tasks
{
    public class ElHaririTask : ITask
    {
        public void Execute()
        {
            //var runtimeServices = new List<Vanrise.Runtime.Entities.RuntimeService>();

            //// BP Services
            //var bpService = new Vanrise.BusinessProcess.BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(bpService);

            //var bpRegulatorRuntimeService = new Vanrise.BusinessProcess.BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(bpRegulatorRuntimeService);

            //// Queue Services
            //var queueRegulatorService = new Vanrise.Queueing.QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(queueRegulatorService);

            //var queueActivationService = new Vanrise.Queueing.QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(queueActivationService);

            //var summaryQueueActivationService = new Vanrise.Queueing.SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(summaryQueueActivationService);

            //// Other Services
            //var schedulerService = new Vanrise.Runtime.SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            //runtimeServices.Add(schedulerService);

            //var runtimeHost = new Vanrise.Runtime.RuntimeHost(runtimeServices);
            //runtimeHost.Start();

            ////InsertIntoQueueTable();

            //Console.WriteLine("\nI'm glad that that's over...\n");
            //Console.ReadKey();
            var decriptedPin = Cryptography.Decrypt("ovpsFZ7BpEgnu/3ykI22fw==", DataEncryptionKeyManager.GetLocalTokenDataDecryptionKey());

            TopupManager topupManager = new TopupManager();
            var topup = topupManager.AddTopup(new AddTopupInput
            {
                PhoneNumber = "7453526",
                PinCode = decriptedPin
            });


            var decriptedPin1 = Cryptography.Decrypt("NOqpvAjWxQ0vSrxvmvUTGQ==", DataEncryptionKeyManager.GetLocalTokenDataDecryptionKey());

            var topup1 = topupManager.AddTopup(new AddTopupInput
            {
                PhoneNumber = "7453526",
                PinCode = decriptedPin1
            });

        }

        //private void InsertIntoQueueTable()
        //{
        //    var accountTypeId = new Guid("20b0c83e-6f53-49c7-b52f-828a19e6dc2a");

        //    new Vanrise.AccountBalance.Business.UsageBalanceManager().UpdateUsageBalance(accountTypeId, new Vanrise.AccountBalance.Entities.UpdateUsageBalancePayload()
        //    {
        //        TransactionTypeId = new Guid("007869D9-6DC2-4F56-88A4-18C8C442E49E"),
        //        UpdateUsageBalanceItems = new List<Vanrise.AccountBalance.Entities.UpdateUsageBalanceItem>()
        //        {
        //            new Vanrise.AccountBalance.Entities.UpdateUsageBalanceItem()
        //            {
        //                AccountId = "422117_1",
        //                Value = 1750,
        //                CurrencyId = 1,
        //                EffectiveOn = new DateTime(2017, 2, 1)
        //            }
        //        }
        //    });
        //}
    }
}
