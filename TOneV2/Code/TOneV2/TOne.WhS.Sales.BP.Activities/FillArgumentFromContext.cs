using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;

namespace TOne.WhS.Sales.BP.Activities
{
    public class FillArgumentFromContext : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public OutArgument<bool> IsFirstSellingProductOffer { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            IsFirstSellingProductOffer.Set(context, ratePlanContext.IsFirstSellingProductOffer);
        }
    }
}
