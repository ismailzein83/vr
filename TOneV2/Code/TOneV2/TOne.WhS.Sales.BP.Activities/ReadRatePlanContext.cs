using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;

namespace TOne.WhS.Sales.BP.Activities
{
    public class ReadRatePlanContext : CodeActivity
    {
        #region Out Arguments

        public OutArgument<bool> ProcessHasChanges { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            ProcessHasChanges.Set(context, ratePlanContext.ProcessHasChanges);
        }
    }
}
