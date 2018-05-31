using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanrise.Fzero.Services.DailyReportsSummary;

namespace Vanrise.Fzero.Services.NonFruadReport
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new DailyReportsSummaryService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
