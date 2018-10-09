using System;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Filters
{
    public enum QualityOption { GreaterThan = 0, GreaterThanOrEqual = 1, LessThan = 2, LessThanOrEqual = 3, Equal = 4, Different = 5 }
    public class QualityOptionFilter : RouteOptionFilterSettings
    {
        public override Guid ConfigId { get { return new Guid("A4CC3BEC-B983-4283-8C82-1C354BBE103C"); } }
        public QualityOption QualityOption { get; set; }
        public decimal QualityOptionValue { get; set; }
        public override void Execute(IRouteOptionFilterExecutionContext context)
        {
            return;
            //if (!context.SaleRate.HasValue)
            //    return;

            //decimal qualityValue = 0;


            //decimal valueLimit = 0;
            //switch (QualityOption)
            //{
            //    case Filters.QualityOption.GreaterThan: valueLimit =  QualityOptionValue; break;
            //    case Filters.QualityOption.LessThan: valueLimit = (-1) * QualityOptionValue; break;
            //}

            //if (qualityValue < valueLimit)
            //{
            //    context.FilterOption = true;
            //}
        }
    }
}