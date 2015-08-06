using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace Vanrise.Fzero.DevRuntime
{
    public partial class WF : Form
    {
        public WF()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            IStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            ddlStrategy.DataSource = dataManager.GetStrategies(0);
            ddlStrategy.ValueMember = "Id";
            ddlStrategy.DisplayMember = "Name";
        }


        public static class DateHelper
        {
            public static DateTime Min(DateTime date1, DateTime date2)
            {
                return (date1 < date2 ? date1 : date2);
            }
        }


        private void btnStart_Click(object sender, EventArgs e)
        {






            Console.WriteLine("Strategy started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 20) };



            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            ////////List<int> StrategyIds = new List<int>();
            ////////StrategyIds.Add(int.Parse(ddlStrategy.SelectedValue.ToString()));

            ////////List<Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput> inputArgs = new List<Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput>();





            //////////var Start = DateTime.Parse(dateTimePicker1.Text);
            //////////var End = DateTime.Parse(dateTimePicker2.Text);

            ////////var Start = DateTime.Parse("2015-03-13 23:01:12.000");
            ////////var End = DateTime.Parse("2015-03-14 06:11:42.000");


            ////////if (ddlPeriods.SelectedIndex == 0)
            ////////{
            ////////    var runningDate = Start;
            ////////    while (runningDate < End)
            ////////    {
            ////////        inputArgs.Add(new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
            ////////        {
            ////////            StrategyIds = StrategyIds,
            ////////            FromDate = runningDate.Date,
            ////////            ToDate = runningDate.Date.AddDays(1).AddSeconds(-1),
            ////////            PeriodId = (int)Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Day
            ////////        });
            ////////        runningDate = runningDate.Date.AddDays(1);
            ////////    }
            ////////}
            ////////else if (ddlPeriods.SelectedIndex == 1)
            ////////{
            ////////    var runningDate = Start;
            ////////    while (runningDate < End)
            ////////    {
            ////////        inputArgs.Add(new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
            ////////        {
            ////////            StrategyIds = StrategyIds,
            ////////            FromDate = new DateTime(runningDate.Year, runningDate.Month, runningDate.Day, runningDate.Hour, 0, 0),
            ////////            ToDate = new DateTime(runningDate.Year, runningDate.Month, runningDate.Day, runningDate.Hour, 59, 59),
            ////////            PeriodId = (int)Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Hour
            ////////        });
            ////////        runningDate = new DateTime(runningDate.Year, runningDate.Month, runningDate.Day, runningDate.Hour, 0, 0).AddHours(1);
            ////////    }
            ////////}






            //////////Task t = new Task(() =>
            //////////{
            //////////    BPClient bpClient3 = new BPClient();



            //////////    foreach (var inputArg in inputArgs)
            //////////    {
            //////////        bpClient3.CreateNewProcess(new CreateProcessInput { InputArguments = inputArg });
            //////////    }





            //////////    Console.WriteLine("END");
            //////////});
            //////////t.ContinueWith((tt) =>
            //////////{
            //////////    this.CloseFormAsync();
            //////////});
            //////////t.Start();
            //////////this.WindowState = FormWindowState.Minimized;


            //Console.ReadKey();

            //this.WindowState = FormWindowState.Minimized;
        }

        void CloseFormAsync()
        {
            if (this.InvokeRequired)
                this.Invoke(new VoidDelegate(CloseFormAsync));
            else
                this.Close();
        }

        delegate void VoidDelegate();

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Saving Imported started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();


            Vanrise.Fzero.CDRImport.BP.Arguments.SaveCDRToDBProcessInput inputArg = new Vanrise.Fzero.CDRImport.BP.Arguments.SaveCDRToDBProcessInput();

            Task t = new Task(() =>
            {
                BPClient bpClient2 = new BPClient();
                bpClient2.CreateNewProcess(new CreateProcessInput
                {
                    InputArguments = inputArg
                });

                Console.WriteLine("END");
            });
            t.ContinueWith((tt) =>
            {
                this.CloseFormAsync();
            });
            t.Start();
            this.WindowState = FormWindowState.Minimized;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Import started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();


            //Vanrise.Fzero.CDRImport.BP.Arguments.CDRImportProcessInput inputArg = new Vanrise.Fzero.CDRImport.BP.Arguments.CDRImportProcessInput();

            //Task t = new Task(() =>
            //{
            //    BPClient bpClient2 = new BPClient();
            //    bpClient2.CreateNewProcess(new CreateProcessInput
            //    {
            //        InputArguments = inputArg
            //    });

            //    Console.WriteLine("END");
            //});
            //t.ContinueWith((tt) =>
            //{
            //    this.CloseFormAsync();
            //});
            //t.Start();
            //this.WindowState = FormWindowState.Minimized;


        }




    }
}
