using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.BusinessProcess;
using System.Activities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class CalculateCriteriaValuesInput
    {
        public BaseQueue<NumberProfileBatch> InputQueue { get; set; }
    }

    public class CalculateCriteriaValuesOutput
    {
        public BaseQueue<NumberProfileBatch> OutputQueue { get; set; }
    }

    #endregion
    public class CalculateCriteriaValues : BaseAsyncActivity<CalculateCriteriaValuesInput, CalculateCriteriaValuesOutput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<NumberProfileBatch>> InputQueue { get; set; }

        public OutArgument<BaseQueue<NumberProfileBatch>> OutputQueue { get; set; }

        #endregion


        //protected override CalculateCriteriaValuesOutput DoWorkWithResult(CalculateCriteriaValuesInput inputArgument, AsyncActivityHandle handle)
        //{
        //    return new CalculateCriteriaValuesOutput
        //    {
        //        value = new CriteriaManager().GetCriteriaValue(inputArgument.criteria,inputArgument.numberProfile)
        //    };
        //}

        //protected override CalculateCriteriaValuesInput GetInputArgument(AsyncCodeActivityContext context)
        //{
        //    return new CalculateCriteriaValuesInput();
        //}

        //protected override void OnWorkComplete(AsyncCodeActivityContext context, CalculateCriteriaValuesOutput result)
        //{
        //    this.value.Set(context, result.value);
        //}

        protected override CalculateCriteriaValuesOutput DoWorkWithResult(CalculateCriteriaValuesInput inputArgument, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override CalculateCriteriaValuesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CalculateCriteriaValuesOutput result)
        {
            throw new NotImplementedException();
        }
    }
}
