using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.BusinessProcess;
using System.Activities;
using Vanrise.Fzero.FraudAnalysis.Business;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class GetCriteriaValueInput
    {
        public CriteriaDefinition criteria { get; set; }
        public NumberProfile numberProfile { get; set; }
    }

    public class GetCriteriaValueOutput
    {
        public Decimal value { get; set; }

    }

    #endregion
    public class GetCriteriaValue : BaseAsyncActivity<GetCriteriaValueInput, GetCriteriaValueOutput>
    {
        #region Arguments
        public InArgument<CriteriaDefinition> criteria { get; set; }
        public InArgument<NumberProfile> numberProfile { get; set; }
        public OutArgument<Decimal> value { get; set; }

        #endregion


        protected override GetCriteriaValueOutput DoWorkWithResult(GetCriteriaValueInput inputArgument, AsyncActivityHandle handle)
        {
            return new GetCriteriaValueOutput
            {
                value = new CriteriaManager().GetCriteriaValue(inputArgument.criteria,inputArgument.numberProfile)
            };
        }

        protected override GetCriteriaValueInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetCriteriaValueInput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetCriteriaValueOutput result)
        {
            this.value.Set(context, result.value);
        }
    }
}
