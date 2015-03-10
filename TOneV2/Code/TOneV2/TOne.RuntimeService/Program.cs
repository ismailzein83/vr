using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TABS.Addons.UtilitiesMultipleQueue.ProxyCommon;

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
            service.Start();
             //service.on
            string ServiceName = service.ServiceName;
            string filePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;


            // Application.EnableVisualStyles();
            //  Application.SetCompatibleTextRenderingDefault(false);

            if (!WindowsServiceHelper.ServiceRegistered(ServiceName))
            {

                /// Register and Start the service. 
                /// Registering with a command line arg to distinguish as a windows service.
                /// The arg is "service" and will be used below to start service execution.
                ///                     
                WindowsServiceHelper.RegisterService(ServiceName, filePath);
                WindowsServiceHelper.RunService(ServiceName);
            }
            else
            {
                /// The service parameter is the same used when registering the service above.
                /// Parameter is available when the service runs. Here is the actual execution
                /// of the service.
                /// 
                bool runService = (args.Length > 0 && args[0].Trim().ToLower() == "service");

                if (runService)
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[] { service };
                    ServiceBase.Run(ServicesToRun);
                }
                //else
                //    RunClient();
            }
        }
    }
}