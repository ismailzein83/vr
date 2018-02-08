﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation
{
    public class LCR2TargetMatchCalculation : TargetMatchCalculationMethod
    {
        public override Guid ConfigId
        {
            get { return new Guid("E393CCB2-E929-4A50-9AE4-AB59565DD69D"); }
        }

        public override void Evaluate(ITargetMatchCalculationMethodContext context)
        {
            var options = new List<SupplierTargetMatchAnalyticOption>();
            for (int i = 0; i < 2; i++)
            {
                RPRouteOptionDetail lcr = context.RPRouteDetail.RouteOptionsDetails.ElementAtOrDefault(i);
                if (lcr != null)
                {
                    SupplierTargetMatchAnalyticOption option = new SupplierTargetMatchAnalyticOption
                    {
                        Rate = context.EvaluateRate(lcr.SupplierRate)
                    };
                    var supplierAnalyticInfo = context.GetSupplierAnalyticInfo(lcr.SupplierId);
                    if (supplierAnalyticInfo != null)
                    {
                        option.ACD = supplierAnalyticInfo.ACD;
                        option.ASR = supplierAnalyticInfo.ACD;
                        option.Duration = supplierAnalyticInfo.Duration;
                    }
                    context.ValidateAnalyticInfo(option);
                    options.Add(option);
                }
            }
            context.Options = options;

        }

    }
}
