using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TestRuntime
{
	public class HassanMakhourTask : ITask
	{
		public void Execute()
		{
			var runtimeServices = new List<RuntimeService>();

			BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
			runtimeServices.Add(bpService);

			BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
			runtimeServices.Add(bpRegulatorRuntimeService);

			RuntimeHost host = new RuntimeHost(runtimeServices);
			host.Start();
			Console.ReadKey();
		}
	}

}
