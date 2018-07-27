using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;

namespace Vanrise.GenericData.BP.Activities.Correlation
{
	#region Classes

	public class LoadCDRsInput
	{
		public Guid InputDataRecordStorageId { get; set; }

		public DateTime FromTime { get; set; }

		public BaseQueue<RecordBatch> OutputQueue { get; set; }
	}

	#endregion
	public sealed class LoadCDRs : DependentAsyncActivity<LoadCDRsInput>
	{
		[RequiredArgument]
		public InArgument<Guid> InputDataRecordStorageId { get; set; }

		[RequiredArgument]
		public InArgument<DateTime> FromTime { get; set; }

		[RequiredArgument]
		public InOutArgument<BaseQueue<RecordBatch>> OutputQueue { get; set; }

		protected override LoadCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
		{
			return new LoadCDRsInput()
			{
				FromTime = this.FromTime.Get(context),
				OutputQueue = this.OutputQueue.Get(context),
				InputDataRecordStorageId = this.InputDataRecordStorageId.Get(context),
			};
		}

		protected override void DoWork(LoadCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
		{
			if (inputArgument.OutputQueue == null)
				throw new NullReferenceException("inputArgument.OutputQueue");

			if (inputArgument.InputDataRecordStorageId == null || inputArgument.InputDataRecordStorageId == Guid.Empty)
				throw new NullReferenceException("inputArgument.InputDataRecordStorageId");

			DataRecordStorageManager manager = new DataRecordStorageManager();
			RecordBatch recordBatch = new RecordBatch() { Records = new List<dynamic>() };

			manager.GetDataRecords(inputArgument.InputDataRecordStorageId, inputArgument.FromTime, null, null, () => ShouldStop(handle), ((itm) =>
			{
				recordBatch.Records.Add(itm);
				if (recordBatch.Records.Count >= 10000)
				{
					inputArgument.OutputQueue.Enqueue(recordBatch);

					recordBatch = new RecordBatch() { Records = new List<dynamic>() };
				}
			}));


			if (recordBatch.Records.Count > 0)
				inputArgument.OutputQueue.Enqueue(recordBatch);

			handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Loading Source Records is done");
		}

	}
}
