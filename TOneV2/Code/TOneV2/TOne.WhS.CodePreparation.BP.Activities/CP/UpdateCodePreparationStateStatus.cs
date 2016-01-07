using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.CodePreparation.Data;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public sealed class UpdateCodePreparationStateStatus : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int sellingNumberPlanId = SellingNumberPlanId.Get(context);
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            dataManager.UpdateCodePreparationStatus(sellingNumberPlanId);
        }
    }
}
