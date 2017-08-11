using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation
{
    public class LCR3TargetMatchCalculation : TargetMatchCalculationMethod
    {
        public override Guid ConfigId
        {
            get { return new Guid("1BA9A5CA-BEB1-4071-B7D1-A8C19227CFBA"); }
        }

        public override void Evaluate(ITargetMatchCalculationMethodContext context)
        {
            var options = new List<SupplierTargetMatchAnalyticOption>();
            for (int i = 0; i < 3; i++)
            {

                RPRouteOptionDetail lcr = context.RPRouteDetail.RouteOptionsDetails.ElementAtOrDefault(i);
                if (lcr != null)
                {
                    var supplierAnalyticInfo = context.GetSupplierAnalyticInfo(lcr.Entity.SupplierId);
                    if (supplierAnalyticInfo != null)
                    {
                        SupplierTargetMatchAnalyticOption option = new SupplierTargetMatchAnalyticOption
                        {
                            Rate = context.EvaluateRate(lcr.Entity.SupplierRate),
                            ACD = supplierAnalyticInfo.ACD,
                            ASR = supplierAnalyticInfo.ACD,
                            Duration = supplierAnalyticInfo.Duration
                        };
                        options.Add(option);
                    }
                }
            }
            context.Options = options;
        }
    }
}
