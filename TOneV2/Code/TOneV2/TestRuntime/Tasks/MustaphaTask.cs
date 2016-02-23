﻿using System;
using System.Collections.Generic;
using System.Timers;
using TOne.CDR.Entities;
using TOne.CDR.QueueActivators;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;

namespace TestRuntime.Tasks
{
    public class MustaphaTask : ITask
    {
        public void Execute()
        {
            System.Threading.ThreadPool.SetMaxThreads(10000, 10000);

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
             

            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);


            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

        }

    }
}
