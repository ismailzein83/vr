using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation
{
    public class LCR1TargetMatchCalculation : TargetMatchCalculationMethod
    {
        public override Guid ConfigId
        {
            get { return new Guid("1BA9A5CA-BEB1-4071-B7D1-A8C19227CFBA"); }
        }

        public override void Evaluate(ITargetMatchCalculationMethodContext context)
        {
            RPRouteOptionDetail lcr1 = context.RPRouteDetail.RouteOptionsDetails.ElementAtOrDefault(0);
            if (lcr1 != null)
            {
                RPRouteOptionDetail rpRouteOptionDetail = Vanrise.Common.Utilities.CloneObject<RPRouteOptionDetail>(lcr1);
                rpRouteOptionDetail.Entity.SupplierRate = EvaluateRate(lcr1.Entity.SupplierRate, context);
                context.RPRouteOptionDetail = rpRouteOptionDetail;
            }
        }

        private decimal EvaluateRate(decimal originalRate, ITargetMatchCalculationMethodContext context)
        {
            decimal value = 0;
            switch (context.MarginType)
            {
                case MarginType.Percentage:
                    value = originalRate * (100 - context.MarginValue) / 100;
                    break;
                case MarginType.Fixed:
                    value = context.MarginValue;
                    break;
            }
            return value;
        }
    }
}
