using System;
using System.Collections.Generic;
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

namespace Vanrise.Fzero.DevRuntime.Tasks
{
    public class MustaphaTask : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Mustapha Task started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            //PersistentQueue<ImportedCDRBatch> queue =  Vanrise.Fzero.CDRImport.Business.CDRQueueFactory.GetImportedCDRQueue("DS1");

            //String fileName = @"C:\Users\mustafa.hawi\Desktop\x.Dat";

          //  queue.Enqueue(new ImportedCDRBatch()
           // {
                //file =File.ReadAllBytes(fileName)
          //  });

            //BPClient bpClient = new BPClient();
            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    ProcessName = "CDRImportProcess",
            //    InputArguments = new Vanrise.Fzero.CDRImport.BP.Arguments.CDRImportProcessInput
            //    {
            //    }
            //});


            BPClient bpClient2 = new BPClient();
            bpClient2.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "SaveCDRToDBProcess",
                InputArguments = new Vanrise.Fzero.CDRImport.BP.Arguments.SaveCDRToDBProcessInput
                {
                }
            });


            Console.WriteLine("END");
            Console.ReadKey();
        }
    }
}
