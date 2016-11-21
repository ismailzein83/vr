using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.BP.Activities
{
    public sealed class ReserveIdForCodePreparation : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<int> CodePreparationId { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            //TOne.WhS.CodePreparation.Business.CodePreparationManager codePreparationManager = new TOne.WhS.CodePreparation.Business.CodePreparationManager();
            //int codePreparationId = (int)codePreparationManager.ReserveIDRange(1);

            //CodePreparationId.Set(context, codePreparationId);
        }
    }
}
