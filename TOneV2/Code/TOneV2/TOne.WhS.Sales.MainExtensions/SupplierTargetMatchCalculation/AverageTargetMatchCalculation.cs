using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation
{
    public class AverageTargetMatchCalculation : TargetMatchCalculationMethod
    {
        public override Guid ConfigId
        {
            get { return new Guid("2CB834AD-16BE-46C7-8B26-E65927C2388B"); }
        }
          
        public override void Evaluate(ITargetMatchCalculationMethodContext context)
        {
            var options = new List<SupplierTargetMatchAnalyticOption>();
            decimal rate = context.RPRouteDetail.RouteOptionsDetails.Average(o => o.ConvertedSupplierRate);
            decimal totalACD = 0;
            decimal totalASR = 0;
            decimal totalDuration = 0;
            foreach (var routeOption in context.RPRouteDetail.RouteOptionsDetails)
            {
                var supplierAnalyticInfo = context.GetSupplierAnalyticInfo(routeOption.SupplierId);
                if (supplierAnalyticInfo != null)
                {
                    totalACD += supplierAnalyticInfo.ACD;
                    totalASR += supplierAnalyticInfo.ASR;
                    totalDuration += supplierAnalyticInfo.Duration;
                }
            }

            SupplierTargetMatchAnalyticOption option = new SupplierTargetMatchAnalyticOption {
                Duration = totalDuration / context.RPRouteDetail.RouteOptionsDetails.Count(),
                ACD = totalACD / context.RPRouteDetail.RouteOptionsDetails.Count(),
                ASR = totalASR / context.RPRouteDetail.RouteOptionsDetails.Count(),
                Rate = rate
            };
            context.ValidateAnalyticInfo(option);
            options.Add(option);
        }
    }
}
