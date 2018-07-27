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

    public class InsertCorrelatedCDRsInput
    {
        public List<CDRCorrelationDefinition> CDRCorrelationDefinition { get; set; }

        public BaseQueue<CDRCorrelationBatch> InputQueueToInsert { get; set; }
    }

    public class InsertCorrelatedCDRsOutput
    {
        public BaseQueue<List<Object>> OutputQueueToDelete { get; set; }
    }

    #endregion

    public sealed class InsertCorrelatedCDRs : DependentAsyncActivity<InsertCorrelatedCDRsInput, InsertCorrelatedCDRsOutput>
    {
        [RequiredArgument]
        public InArgument<CDRCorrelationDefinition> CDRCorrelationDefinition { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRCorrelationBatch>> InputQueueToInsert { get; set; }

        [RequiredArgument]
        public OutArgument<BaseQueue<List<Object>>> OutputQueueToDelete { get; set; }


        protected override InsertCorrelatedCDRsOutput DoWorkWithResult(InsertCorrelatedCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override InsertCorrelatedCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, InsertCorrelatedCDRsOutput result)
        {
            throw new NotImplementedException();
        }
    }
}