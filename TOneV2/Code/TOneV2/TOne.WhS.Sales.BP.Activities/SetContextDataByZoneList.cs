using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class SetContextDataByZoneList : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<DataByZone>> DataByZoneList { get; set; }

        protected override void Execute(System.Activities.CodeActivityContext context)
        {
            var ratePlanContext = context.GetRatePlanContext() as RatePlanContext;
            ratePlanContext.DataByZoneList = DataByZoneList.Get(context);
        }
    }
}
