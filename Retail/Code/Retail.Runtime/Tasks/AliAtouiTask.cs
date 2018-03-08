using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Common.Business;
using Vanrise.Integration.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Retail.Runtime.Tasks
{
    public class AliAtouiTask : ITask
    {
        public void Execute()
        {
            #region Runtime
            //ExecuteRuntime.Runtime_Main();
            #endregion

            #region RetailAccountPropertyEvaluatorTask
            //RetailAccountPropertyEvaluatorTask.RetailAccountPropertyEvaluator_Main();
            #endregion
        }

        public static MappingOutput DataSourceMapData(IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var cdrs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("CDRCost");
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("CDRCost");

            var currentItemCount = 21;
            System.IO.StreamReader sr = ImportedData.StreamReader;
            Vanrise.Common.Business.CurrencyManager currencyManager = new Vanrise.Common.Business.CurrencyManager();

            while (!sr.EndOfStream)
            {
                string currentLine = sr.ReadLine();
                if (string.IsNullOrEmpty(currentLine))
                    continue;

                string[] rowData = currentLine.Split(';');
                if (rowData.Length != currentItemCount)
                    continue;

                string customerName = rowData[18];
                string durationAsString;
                string amountAsString;

                if (string.Compare(customerName, "FLL-Incoming (Dom)", true) == 0)
                {
                    durationAsString = rowData[17];
                    amountAsString = rowData[14];
                }
                else if (string.Compare(customerName, "FLL-Incoming (IDD)", true) == 0 || string.Compare(customerName, "FLL_Incoming (IDD)", true) == 0)
                {
                    durationAsString = rowData[16];
                    amountAsString = rowData[13];
                }
                else
                    continue;

                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                cdr.SourceID = rowData[0];
                cdr.AttemptDateTime = DateTime.ParseExact(rowData[1], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                cdr.ReceivedCGPN = rowData[4];
                cdr.CGPN = cdr.ReceivedCGPN;
                cdr.ReceivedCDPN = rowData[5];
                cdr.CDPN = cdr.ReceivedCDPN;
                cdr.DurationInSeconds = !string.IsNullOrEmpty(durationAsString) ? decimal.Parse(durationAsString) : default(decimal?);
                cdr.Amount = !string.IsNullOrEmpty(amountAsString) ? decimal.Parse(amountAsString) : default(decimal?);
                cdr.FileName = ImportedData.Name;

                string rateAsString = rowData[12];
                cdr.Rate = !string.IsNullOrEmpty(rateAsString) ? decimal.Parse(rateAsString) : default(decimal?);

                cdr.SupplierName = rowData[19];
                string currencySymbol = rowData[10];
                if (!string.IsNullOrEmpty(currencySymbol))
                {
                    var currency = currencyManager.GetCurrencyBySymbol(currencySymbol);
                    if (currency != null)
                        cdr.Currency = currency.CurrencyId;
                }
                string statusAsString = rowData[20];
                if (!string.IsNullOrEmpty(statusAsString) && string.Compare(statusAsString, "R", true) == 0)
                    cdr.IsReRate = true;

                cdrs.Add(cdr);
            }

            if (cdrs.Count > 0)
            {
                long startingId;
                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, cdrs.Count, out startingId);
                long currentCDRId = startingId;

                foreach (var cdr in cdrs)
                {
                    cdr.ID = currentCDRId;
                    currentCDRId++;
                }
                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "CDRCost");
                mappedBatches.Add("Distribute CDR Cost Stage", batch);
            }
            else
                ImportedData.IsEmpty = true;

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;

            return result;
        }
    }

    public class ExecuteRuntime
    {
        public static void Runtime_Main()
        {
            var runtimeServices = new List<RuntimeService>();

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(schedulerService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService);

            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bigDataService);

            CachingRuntimeService cachingRuntimeService = new CachingRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingRuntimeService);

            CachingDistributorRuntimeService cachingDistributorRuntimeService = new CachingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingDistributorRuntimeService);

            DataGroupingExecutorRuntimeService dataGroupingExecutorRuntimeService = new Vanrise.Common.Business.DataGroupingExecutorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dataGroupingExecutorRuntimeService);

            DataGroupingDistributorRuntimeService dataGroupingDistributorRuntimeService = new Vanrise.Common.Business.DataGroupingDistributorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dataGroupingDistributorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            Console.ReadKey();
        }
    }

    public class RetailAccountPropertyEvaluatorTask
    {
        public static void RetailAccountPropertyEvaluator_Main()
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


