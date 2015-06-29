﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.BusinessProcess.Extensions
{
    public class WFSchedulerTaskAction : SchedulerTaskAction
    {
        public int BPDefinitionID { get; set; }

        public BaseProcessInputArgument ProcessInputArguments { get; set; }

        public override void Execute()
        {
            Console.WriteLine("WFSchedulerTaskAction started...");

            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new BusinessProcess.Entities.CreateProcessInput
            {
                InputArguments = this.ProcessInputArguments
            });

            Console.WriteLine("WFSchedulerTaskAction finished...");
        }
    }
}
