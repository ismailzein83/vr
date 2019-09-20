using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Retail.Billing.Entities;

namespace Retail.Runtime.Tasks
{
    public class NaimTask : ITask
    {
        public void Execute()
        {

            string FilterGroup = "", FirstName = "اسماعيل",  MiddleName = "احمد", LastName = "زين", MotherName = "سلوى";
            decimal CustomerId = 6;

            Vanrise.GenericData.Entities.RecordFilterGroup recordFilterGroup = new Vanrise.GenericData.Entities.RecordFilterGroup()
            {
                LogicalOperator = Vanrise.GenericData.Entities.RecordQueryLogicalOperator.And,
                Filters = new System.Collections.Generic.List<Vanrise.GenericData.Entities.RecordFilter>() { }
            };

            Vanrise.GenericData.Entities.RecordFilterGroup subRecordFilterGroup = new Vanrise.GenericData.Entities.RecordFilterGroup()
            {
                LogicalOperator = Vanrise.GenericData.Entities.RecordQueryLogicalOperator.Or,
                Filters = new System.Collections.Generic.List<Vanrise.GenericData.Entities.RecordFilter>() { }
            };


            var parentsFilters = new Vanrise.GenericData.Entities.StringListRecordFilter()
            {
                FieldName = "FirstName",
                CompareOperator = Vanrise.GenericData.Entities.ListRecordFilterOperator.In,
                Values = new System.Collections.Generic.List<string>() { FirstName, MiddleName, MotherName }
            };

            var paternalSiblingsOrChildrenFilters = new Vanrise.GenericData.Entities.StringListRecordFilter()
            {
                FieldName = "MiddleName",
                CompareOperator = Vanrise.GenericData.Entities.ListRecordFilterOperator.In,
                Values = new System.Collections.Generic.List<string>() { FirstName, MiddleName }
            };

                      subRecordFilterGroup.Filters.Add(Vanrise.GenericData.Business.Helper.ConvertToRecordFilter("FirstName", new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType(), parentsFilters));
            subRecordFilterGroup.Filters.Add(Vanrise.GenericData.Business.Helper.ConvertToRecordFilter("MiddleName", new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType(), paternalSiblingsOrChildrenFilters));

            recordFilterGroup.Filters.Add(subRecordFilterGroup);

            recordFilterGroup.Filters.Add(new Vanrise.GenericData.Entities.StringRecordFilter()
            {
                FieldName = "LastName",
                CompareOperator = Vanrise.GenericData.Entities.StringRecordFilterOperator.Equals,
                Value = LastName
            });
            recordFilterGroup.Filters.Add(new Vanrise.GenericData.Entities.NonEmptyRecordFilter()
            {
                FieldName = "DunningStatus"
            });
            recordFilterGroup.Filters.Add(new Vanrise.GenericData.Entities.NumberListRecordFilter()
            {
                FieldName = "ID",
                CompareOperator = Vanrise.GenericData.Entities.ListRecordFilterOperator.NotIn,
                Values = new System.Collections.Generic.List<decimal>() { CustomerId }
            });
            FilterGroup = Vanrise.Common.Serializer.Serialize(recordFilterGroup);



            var entities = new Vanrise.GenericData.Business.GenericBusinessEntityManager().GetAllGenericBusinessEntities(new System.Guid("cd136747-8961-4794-a5c6-a78bb948a8e6"), null, recordFilterGroup);




            //var s = billingContractServiceManager.GetBillingContractServices(recordFilterGroup);
            //List<BillingRatePlanService> billingRatePlanServices = new List<BillingRatePlanService>();

            //foreach (var na in s)
            //{
            //    var billingRatePlanService = billingRatePlanServiceManager.GetBillingRatePlanServiceByRatePlanAndService(na.RatePlanId, na.ServiceID);
            //    if (billingRatePlanService != null)
            //        billingRatePlanServices.Add(billingRatePlanService);
            //}
            var nnsms = 0;

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
            //var decriptedPin = Cryptography.Decrypt("ovpsFZ7BpEgnu/3ykI22fw==", DataEncryptionKeyManager.GetLocalTokenDataDecryptionKey());

            //TopupManager topupManager = new TopupManager();
            //var topup = topupManager.AddTopup(new AddTopupInput
            //{
            //    PhoneNumber = "7453526",
            //    PinCode = decriptedPin
            //});


            //var decriptedPin1 = Cryptography.Decrypt("NOqpvAjWxQ0vSrxvmvUTGQ==", DataEncryptionKeyManager.GetLocalTokenDataDecryptionKey());

            //var topup1 = topupManager.AddTopup(new AddTopupInput
            //{
            //    PhoneNumber = "7453526",
            //    PinCode = decriptedPin1
            //});


            //  var retailInvoiceSettings = new Retail.BusinessEntity.Business.ConfigManager().GetRetailTaxesDefinitions();
            int tta = 0;


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
