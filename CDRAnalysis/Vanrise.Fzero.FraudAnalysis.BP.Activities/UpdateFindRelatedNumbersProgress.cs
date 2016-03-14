using System.Activities;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.BP.Arguments;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class UpdateFindRelatedNumbersProgress : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<FindRelatedNumbersProgress> FindRelatedNumbersProgress { get; set; }


        [RequiredArgument]
        public InArgument<FindRelatedNumbersForNumberRangeProcessOutput> FindRelatedNumbersSubProcessEventOutput { get; set; }


        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            var findRelatedNumbersProgress = context.GetValue(FindRelatedNumbersProgress);
            var currentOutput = context.GetValue(FindRelatedNumbersSubProcessEventOutput);

            findRelatedNumbersProgress.CDRsProcessed += currentOutput.CDRsProcessed;
        }
    }
}
