using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TABS.Addons.UtilitiesMultipleQueue.AMA;
using TABS.Addons.UtilitiesMultipleQueue.ProxyCommon;
using TOne.Runtime;
using Vanrise.BusinessProcess.WFActivities;
using File = System.IO.File;

namespace TOne.RuntimeService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            TOneRuntimeService service = new TOneRuntimeService();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { service };
            ServiceBase.Run(ServicesToRun);
        }
    }
}