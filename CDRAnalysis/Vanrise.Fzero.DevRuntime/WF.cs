using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Fzero.FraudAnalysis.Data.MySQL;

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
            ddlStrategy.DataSource = dataManager.GetAllStrategies();
            ddlStrategy.ValueMember = "Id";
            ddlStrategy.DisplayMember = "Name";


            ddlPeriods.Items.Add("Daily");
            ddlPeriods.Items.Add("Hourly");
            ddlPeriods.SelectedIndex = 1;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            Console.WriteLine("Walid Task started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            List<int> StrategyIds = new List<int>();
            StrategyIds.Add( int.Parse( ddlStrategy.SelectedValue.ToString()));

            var inputArgs = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
            {
                StrategyIds = StrategyIds,
                FromDate = DateTime.Parse(dateTimePicker1.Text),
                ToDate = DateTime.Parse(dateTimePicker2.Text),
                PeriodId = (ddlPeriods.SelectedText == "Day" ? (int)Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Day : (int)Vanrise.Fzero.FraudAnalysis.Entities.Enums.Period.Hour)
            };

            Task t = new Task(() =>
            {
                BPClient bpClient3 = new BPClient();

                //bpClient3.CreateNewProcess(new CreateProcessInput
                //{
                //    ProcessName = "ExecuteStrategy",
                //    InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                //    {

                //    }
                //});

               



                bpClient3.CreateNewProcess(new CreateProcessInput
                {
                    ProcessName = "ExecuteStrategyProcess",
                    InputArguments = inputArgs
                });


                Console.WriteLine("END");
            });
            t.ContinueWith((tt) =>
            {
                this.CloseFormAsync();
            });
            t.Start();
            this.WindowState = FormWindowState.Minimized;

           
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

      

       
    }
}
