using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Activities;
using Vanrise.Fzero.FraudAnalysis.Business;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
   
    public class GetCriteriaDefinition : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public OutArgument<Dictionary<int, CriteriaDefinition>> CriteriaDefinitions { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            context.SetValue(CriteriaDefinitions, new CriteriaManager().GetCriteriaDefinitions());
        }
    }
}
