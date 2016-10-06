using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.GenericData.Entities;
using Vanrise.BusinessProcess;

namespace Vanrise.GenericData.BP.Activities
{
    #region Arguments

    public class LoadRecordsFromStoragesInput
    {

    }

    public class LoadRecordsFromStoragesOutput
    {

    }

    #endregion

    public sealed class LoadRecordsFromStorages : BaseAsyncActivity<LoadRecordsFromStoragesInput, LoadRecordsFromStoragesOutput>
    {
        [RequiredArgument]
        public InArgument<List<Guid>> RecordStorageIds { get; set; }

        public InArgument<DateTime> FromTime { get; set; }

        public InArgument<DateTime> ToTime { get; set; }

        public InOutArgument<BaseQueue<RecordBatch>> OutputQueue { get; set; }
        
        protected override LoadRecordsFromStoragesOutput DoWorkWithResult(LoadRecordsFromStoragesInput inputArgument, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override LoadRecordsFromStoragesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadRecordsFromStoragesOutput result)
        {
            throw new NotImplementedException();
        }
    }
}
