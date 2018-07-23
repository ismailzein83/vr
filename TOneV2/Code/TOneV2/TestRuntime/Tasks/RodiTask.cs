using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestRuntime;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Integration.Business;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TOne.WhS.Runtime.Tasks
{
	public class RodiTask : ITask
	{
		public void Execute()
		{
			#region Runtime
			var runtimeServices = new List<RuntimeService>();

			BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
			runtimeServices.Add(bpService);

			BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
			runtimeServices.Add(bpRegulatorRuntimeService);

			QueueActivationRuntimeService queueActivationRuntimeService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
			runtimeServices.Add(queueActivationRuntimeService);

			QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
			runtimeServices.Add(queueActivationService);

			QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
			runtimeServices.Add(queueRegulatorService);

			SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
			runtimeServices.Add(schedulerService);

			SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
			runtimeServices.Add(summaryQueueActivationService);

			DataSourceRuntimeService dsRuntimeService = new DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
			runtimeServices.Add(dsRuntimeService);

			BigDataRuntimeService bigDataService = new BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
			runtimeServices.Add(bigDataService);

			CachingRuntimeService cachingRuntimeService = new CachingRuntimeService { Interval = new TimeSpan(0, 0, 2) };
			runtimeServices.Add(cachingRuntimeService);

			CachingDistributorRuntimeService cachingDistributorRuntimeService = new CachingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
			runtimeServices.Add(cachingDistributorRuntimeService);

			DataGroupingExecutorRuntimeService dataGroupingExecutorRuntimeService = new Vanrise.Common.Business.DataGroupingExecutorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
			runtimeServices.Add(dataGroupingExecutorRuntimeService);

			DataGroupingDistributorRuntimeService dataGroupingDistributorRuntimeService = new Vanrise.Common.Business.DataGroupingDistributorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
			runtimeServices.Add(dataGroupingDistributorRuntimeService);

			/*var assemblies = new List<Assembly>();

            var tOneFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "TOne*Entities.dll");
            var vanriseFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Vanrise*Entities.dll");

            foreach (var file in tOneFiles)
                assemblies.Add(Assembly.LoadFile(file));
            foreach (var file in vanriseFiles)
                assemblies.Add(Assembly.LoadFile(file));

            Vanrise.Common.Business.UtilityManager.GenerateDocumentationForEnums(assemblies);*/


			//VRFileManager fileManager = new VRFileManager();
			//List<VRFile> files = new List<VRFile>();
			//VRFile z = new VRFile();
			//Stream stream = new MemoryStream(z.Content);
			//using (var fs = new FileStream(@"D:\rodi.zip", FileMode.Open, FileAccess.Read))
			//{
			//	using (var zf = new ZipFile(stream))
			//	{
			//		foreach (ZipEntry ze in zf)
			//		{
			//			if (ze.IsDirectory)
			//				continue;
			//			Console.Out.WriteLine(ze.Name);
			//			using (Stream s = zf.GetInputStream(ze))
			//			{
			//				byte[] buf = new byte[4096];
			//				// Analyze file in memory using MemoryStream.
			//				using (MemoryStream ms = new MemoryStream())
			//				{
			//					StreamUtils.Copy(s, ms, buf);
			//				}
			//				string[] nameastab = ze.Name.Split('.');
			//				VRFile file = new VRFile()
			//				{
			//					Content = buf,
			//					Name = ze.Name,
			//					Extension = nameastab[nameastab.Length - 1],
			//					IsTemp = true,
			//				};
			//				long id = fileManager.AddFile(file);
			//				file.FileId = id;
			//				files.Add(file);
			//				// Uncomment the following lines to store the file on disk.
			//				/*using (FileStream fs = File.Create(@"c:\temp\uncompress_" + ze.Name))
			//                         {
			//                           StreamUtils.Copy(s, fs, buf);
			//                         }*/
			//			}
			//		}
			//	}
			//}


			RuntimeHost host = new RuntimeHost(runtimeServices);
			host.Start();
			Console.ReadKey();
			#endregion
		}
	}
}
