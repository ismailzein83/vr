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

    public class GetCriteriaDefinitionInput
    {
    }

    public class GetCriteriaDefinitionOutput
    {
        public Dictionary<int, CriteriaDefinition> CriteriaDefinitions { get; set; }

    }

    #endregion
    public class GetCriteriaDefinition : BaseAsyncActivity<GetCriteriaDefinitionInput, GetCriteriaDefinitionOutput>
    {
        #region Arguments

        public OutArgument<Dictionary<int, CriteriaDefinition>> CriteriaDefinitions { get; set; }

        #endregion


        protected override GetCriteriaDefinitionOutput DoWorkWithResult(GetCriteriaDefinitionInput inputArgument, AsyncActivityHandle handle)
        {
            return new GetCriteriaDefinitionOutput
            {
                CriteriaDefinitions = new CriteriaManager().GetCriteriaDefinitions()
            };
        }

        protected override GetCriteriaDefinitionInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetCriteriaDefinitionInput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetCriteriaDefinitionOutput result)
        {
            this.CriteriaDefinitions.Set(context, result.CriteriaDefinitions);
        }
    }
}
