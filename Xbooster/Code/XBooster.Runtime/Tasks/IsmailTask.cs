﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Runtime;

namespace XBooster.Runtime.Tasks
{
    class IsmailTask : ITask
    {
        public void Execute()
        {
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            var runtimeServices = new List<RuntimeService>();

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
        }
    }
}
