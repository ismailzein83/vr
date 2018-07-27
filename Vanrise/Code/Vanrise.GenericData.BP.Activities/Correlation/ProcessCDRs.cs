using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;

namespace Vanrise.GenericData.BP.Activities.Correlation
{
	public class ProcessCDRsInput
	{
		public BaseQueue<RecordBatch> InputQueue { get; set; }

		public Guid MergeDataTransformationDefinitionId { get; set; }

		public string CallingNumberFieldName { get; set; }

		public string CalledNumberFieldName { get; set; }

		public string DurationFieldName { get; set; }

		public string DatetimeFieldName { get; set; }

		public BaseQueue<RecordBatch> OutputQueue { get; set; }

		public IEnumerable<RecordBatch> CurrentCDRs { get; set; }
	}

	public sealed class ProcessCDRs : DependentAsyncActivity<ProcessCDRsInput>
	{
		[RequiredArgument]
		public InArgument<BaseQueue<RecordBatch>> InputQueue { get; set; }

		[RequiredArgument]
		public InArgument<Guid> MergeDataTransformationDefinitionId { get; set; }

		[RequiredArgument]
		public InArgument<string> CallingNumberFieldName { get; set; }

		[RequiredArgument]
		public InArgument<string> CalledNumberFieldName { get; set; }

		[RequiredArgument]
		public InArgument<string> DurationFieldName { get; set; }

		[RequiredArgument]
		public InArgument<string> DatetimeFieldName { get; set; }

		[RequiredArgument]
		public OutArgument<BaseQueue<RecordBatch>> OutputQueue { get; set; }

		[RequiredArgument]
		public InOutArgument<IEnumerable<RecordBatch>> CurrentCDRs { get; set; }


		protected override ProcessCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
		{
			return new ProcessCDRsInput()
			{
				InputQueue = this.InputQueue.Get(context),
				MergeDataTransformationDefinitionId = this.MergeDataTransformationDefinitionId.Get(context),
				CallingNumberFieldName = this.CallingNumberFieldName.Get(context),
				CalledNumberFieldName = this.CalledNumberFieldName.Get(context),
				DurationFieldName = this.DurationFieldName.Get(context),
				DatetimeFieldName = this.DatetimeFieldName.Get(context),
				OutputQueue = this.OutputQueue.Get(context),
				CurrentCDRs = this.CurrentCDRs.Get(context),
			};
		}

		protected override void DoWork(ProcessCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
		{
			throw new NotImplementedException();
		}
	}
}
