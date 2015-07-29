//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.BusinessProcess;
//using Vanrise.BusinessProcess.Client;
//using Vanrise.BusinessProcess.Entities;
//using Vanrise.Queueing.Entities;

//namespace Vanrise.Queueing.Extensions
//{
//    public class CreateProcessQueueActivator : QueueActivator
//    {        
//        public BaseProcessInputArgument ProcessInputArguments { get; set; }

//        public override void Run(QueueInstance queueInstance)
//        {
//            BPClient bpClient = new BPClient();
//            bpClient.CreateNewProcess(new BusinessProcess.Entities.CreateProcessInput
//                {
//                    InputArguments = this.ProcessInputArguments
//                });
//        }

//        public override void OnDisposed()
//        {
//        }
//    }
//}