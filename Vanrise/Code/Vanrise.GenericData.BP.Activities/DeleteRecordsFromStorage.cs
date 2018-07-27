using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.GenericData.Entities;
using Vanrise.BusinessProcess;
using Vanrise.GenericData.Business;
using Vanrise.Entities;

namespace Vanrise.GenericData.BP.Activities
{
    #region Arguments

    public class DeleteRecordsFromStorageInput
    {
        public BaseQueue<List<Object>> InputQueue { get; set; }

        public Guid DataRecordStorageId { get; set; }
    }

    public class DeleteRecordsFromStorageOutput
    {

    }

    #endregion

    public sealed class DeleteRecordsFromStorage : DependentAsyncActivity<DeleteRecordsFromStorageInput, DeleteRecordsFromStorageOutput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<List<Object>>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<Guid> DataRecordStorageId { get; set; }

        protected override DeleteRecordsFromStorageOutput DoWorkWithResult(DeleteRecordsFromStorageInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, DeleteRecordsFromStorageOutput result)
        {
            throw new NotImplementedException();
        }

        protected override DeleteRecordsFromStorageInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}