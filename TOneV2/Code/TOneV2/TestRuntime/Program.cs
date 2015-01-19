using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using System.Timers;
using System.Configuration;
using TOne.Entities;
using System.Collections.Concurrent;

namespace TestRuntime
{
    class Program
    {
        static void Main(string[] args)
        {
            //BuildCodeMatches();
            //Console.ReadKey();
            //Guid guid = Guid.NewGuid();
            //var cacheManager = Caching.CacheManagerFactory.GetCacheManager<MyCacheManager>(guid);

            //int initialVal = 1;
            //for (int i = 0; i < 5; i++)
            //{
            //    Console.WriteLine(cacheManager.GetOrCreateObject("testCache", 1, () => initialVal++).ToString());
            //}

            //Caching.CacheManagerFactory.RemoveCacheManager(guid);

            //guid = Guid.NewGuid();
            //cacheManager = Caching.CacheManagerFactory.GetCacheManager<MyCacheManager>(guid);


            //for (int i = 0; i < 5; i++)
            //{
            //    Console.WriteLine(cacheManager.GetOrCreateObject("testCache", 1, () => initialVal++).ToString());
            //}

            var config = new BPConfiguration { MaxConcurrentWorkflows = 20 };
            var ser = Vanrise.Common.Serializer.Serialize(config);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            //List<int> lst = new List<int>();
            //for (int i = 0; i < 200; i++)
            //    lst.Add(i);

            //Parallel.ForEach(lst, (i) =>
            //{
            //    //System.Threading.Thread.Sleep(i * 10);
            //    Console.WriteLine(i.ToString());
            //    System.Threading.Thread.Sleep(i * 100);
            //});
            //Console.WriteLine("Parallel done");
            //Console.ReadKey();
            //            CarrierAccountManager manager = new CarrierAccountManager();
            //            string[] carrierIds = new string[] { "C001", "C006", "C019", "C025", "C045", "C059", "C060", 
            //"C061",
            //"C062",
            //"C063",
            //"C064",
            //"C065",
            //"C066",
            //"C067",
            //"C068",
            //"C069",
            //"C070",
            //"C071",
            //"C072",
            //"C073",
            //"C074",
            //"C075",
            //"C076",
            //"C077",
            //"C078",
            //"C079",
            //"C080",
            //"C081" };
            //            List<CarrierAccount> carrierAccounts = new List<CarrierAccount>();
            //            foreach (var id in carrierIds)
            //            {
            //                carrierAccounts.Add(manager.GetCarrierAccount(id, TradeDirection.Buy, DateTime.Today));
            //                carrierAccounts.Add(manager.GetCarrierAccount(id, TradeDirection.Sell, DateTime.Today));
            //            }

            //long bpId;
            //BusinessProcessRuntime.Current.CreateNewProcess(new DailyRepricingProcess { RepricingDay = DateTime.Parse("10/15/2013") }, out bpId);
            //BusinessProcessRuntime.Current.CreateNewProcess(new DailyRepricingProcess { RepricingDay = DateTime.Parse("9/16/2014 10:00") }, out bpId);

            Console.WriteLine("Host Started");
            BusinessProcessRuntime.Current.TerminatePendingProcesses();

            Timer timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();

            System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
                {
                    for (DateTime d = DateTime.Parse(ConfigurationManager.AppSettings["RepricingFrom"]); d <= DateTime.Parse(ConfigurationManager.AppSettings["RepricingTo"]); d = d.AddDays(1))
                    {
                        TriggerProcess(d);
                        System.Threading.Thread.Sleep(30000);
                    }
                });
            t.Start();
            ProcessManager processManager = new ProcessManager();
            processManager.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "UpdateCodeZoneMatchProcess",
                InputArguments = new TOne.LCRProcess.Arguments.UpdateCodeZoneMatchProcessInput
                {
                    IsFuture = false,
                    CodeEffectiveOn = DateTime.Today,
                    GetChangedCodeGroupsOnly = false
                }
            });
            //BusinessProcessRuntime.Current.CreateNewProcess<TOne.LCRProcess.UpdateCodeZoneMatchProcess>(new CreateProcessInput { InputArguments = new TOne.LCRProcess.UpdateCodeZoneMatchProcessInput { IsFuture = true } });

            processManager.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "UpdateZoneRateProcess"
            });

            //TriggerProcess(DateTime.Parse("10/15/2013"));
            //TriggerProcess(DateTime.Parse("09/16/2014"));
            //TriggerProcess(DateTime.Parse("07/07/2014"));
            //System.Threading.Thread.Sleep(5000);
            //TriggerProcess(DateTime.Parse("07/29/2014"));
            //System.Threading.Thread.Sleep(5000);
            //TriggerProcess(DateTime.Parse("07/28/2014"));

            //CDRManager cdrManager = new CDRManager();
            //cdrManager.LoadCDRRange(DateTime.Parse("8/1/2014"), DateTime.Parse("1/1/2015"), 2000, (cdrs) =>
            //    {
            //        RepricingProcess process = new RepricingProcess { CDRs = cdrs };
            //        BusinessProcessRuntime.Current.CreateNewProcess(process);
            //    });



            Console.ReadKey();
        }

        private class MyCode : LCRCode
        {
            public int ValueAsInt { get; set; }
        }

        private static void BuildCodeMatches()
        {
            //////bool isRunning = true;
            //////TOne.Data.SQL.CodeDataManager dataManager = new TOne.Data.SQL.CodeDataManager();
            ////////IEnumerable<MyCode> allCodes = dataManager.GetAllCodesOrdered().Select(itm => new MyCode
            ////////    {
            ////////         ID = itm.ID,
            ////////          CodeGroup = itm.CodeGroup,
            ////////           SupplierId = itm.SupplierId,
            ////////            BeginEffectiveDate = itm.BeginEffectiveDate,
            ////////             EndEffectiveDate =itm.EndEffectiveDate,
            ////////              Timestamp = itm.Timestamp,
            ////////               Value = itm.Value,
            ////////                ZoneId = itm.ZoneId,
            ////////                 ValueAsInt = long.Parse(itm.Value.PadRight)
            ////////    });

            //////IEnumerable<LCRCode> allCodes = dataManager.GetAllCodesOrdered();
            //////ConcurrentQueue<CodeMatch> qCodeMatches = new ConcurrentQueue<CodeMatch>();
            //////System.Threading.Tasks.Task tProcessCodeMatches = new System.Threading.Tasks.Task(() =>
            //////{
            //////    int totalProcessed = 0;
            //////    while (isRunning || qCodeMatches.Count > 0)
            //////    {
            //////        CodeMatch codeMatch;
            //////        while (qCodeMatches.TryDequeue(out codeMatch))
            //////        {
            //////            totalProcessed++;
            //////            if ((totalProcessed % 100000) == 0)
            //////                Console.WriteLine("{0}: {1} code matches processed", DateTime.Now, totalProcessed);
            //////        }
            //////        System.Threading.Thread.Sleep(100);
            //////    }
            //////    Console.WriteLine("{0}: DONE", DateTime.Now);
            //////});
            //////tProcessCodeMatches.Start();
            //////Dictionary<string, List<string>> distinctCodesWithSuppliers = new Dictionary<string, List<string>>();
            //////string lastDistinctCode = null;
            ////////List<string> completedDistinctCodes = new List<string>();
            ////////int totalCodesProcessed = 0;
            //////int processedCodeGroups = 0;
            //////string currentCodeGroup = null;
            //////foreach (var code in allCodes)
            //////{
            //////    if (code.Value != lastDistinctCode)
            //////    {
            //////        lastDistinctCode = code.Value;
            //////        distinctCodesWithSuppliers.Add(lastDistinctCode, new List<string>());
            //////        if (code.CodeGroup != currentCodeGroup)
            //////        {
            //////            processedCodeGroups++;
            //////            Console.WriteLine("{0}: {1} code group done. {2} total processed", DateTime.Now, currentCodeGroup, processedCodeGroups);
            //////            distinctCodesWithSuppliers.Clear();
            //////            currentCodeGroup = code.CodeGroup;
            //////        }
            //////    }
            //////    foreach (string distinctCode in distinctCodesWithSuppliers.Keys)
            //////    {
            //////        if (true)//distinctCode.StartsWith(code.Value))
            //////        {
            //////            List<string> distinctCodeSuppliers = distinctCodesWithSuppliers[distinctCode];

            //////            if (!distinctCodeSuppliers.Contains(code.SupplierId))
            //////            {
            //////                qCodeMatches.Enqueue(new CodeMatch
            //////                    {
            //////                        Code = distinctCode,
            //////                        SupplierCode = code.Value,
            //////                        SupplierId = code.SupplierId,
            //////                        SupplierCodeId = code.ID,
            //////                        SupplierZoneId = code.ZoneId
            //////                    });
            //////                distinctCodeSuppliers.Add(code.SupplierId);
            //////            }
            //////        }
            //////        //else
            //////        //{
            //////        //    completedDistinctCodes.Add(distinctCode);
            //////        //}
            //////    }

            //////    //if (completedDistinctCodes.Count > 0)
            //////    //{
            //////    //    foreach (string completedDistinctCode in completedDistinctCodes)
            //////    //    {
            //////    //        distinctCodesWithSuppliers.Remove(completedDistinctCode);
            //////    //    }
            //////    //    completedDistinctCodes.Clear();
            //////    //}
            //////    //totalCodesProcessed++;
            //////    //if ((totalCodesProcessed % 10000) == 0)
            //////    //    Console.WriteLine("{0}: {1} code processed", DateTime.Now, totalCodesProcessed);
            //////}
            //////lock (typeof(Program))
            //////    isRunning = false;
        }

        private static void TriggerProcess(DateTime date)
        {
            TOne.CDRProcess.Arguments.DailyRepricingProcessInput inputArguments = new TOne.CDRProcess.Arguments.DailyRepricingProcessInput { RepricingDay = date };
            CreateProcessInput input = new CreateProcessInput
            {
                ProcessName = "DailyRepricingProcess",
                InputArguments = inputArguments
            };
            ProcessManager processManager = new ProcessManager();
            processManager.CreateNewProcess(input);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(e.ExceptionObject.ToString());

        }

        static bool _isRunning;
        static object _lockObj = new object();
        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lockObj)
            {
                if (_isRunning)
                    return;
                _isRunning = true;
            }
            try
            {
                //BusinessProcessRuntime.Current.LoadAndExecutePendings();

                BusinessProcessRuntime.Current.ExecutePendings();
                BusinessProcessRuntime.Current.TriggerPendingEvents();
            }
            finally
            {
                lock (_lockObj)
                {
                    _isRunning = false;
                }
            }
        }
    }
}
