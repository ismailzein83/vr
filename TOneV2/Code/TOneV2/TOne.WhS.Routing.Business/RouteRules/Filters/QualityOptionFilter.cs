using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Filters
{
    public enum QualityOptionType { Equal = 0, Different = 1, GreaterThan = 2, GreaterThanOrEqual = 3, LessThan = 4, LessThanOrEqual = 5, }
    public class QualityOptionFilter : RouteOptionFilterSettings
    {
        public static Guid s_ConfigId { get { return new Guid("A4CC3BEC-B983-4283-8C82-1C354BBE103C"); } }
        public override Guid ConfigId { get { return s_ConfigId; } }

        public QualityOptionType QualityOptionType { get; set; }
        public decimal QualityOptionValue { get; set; }
        public Guid? QualityConfigurationId { get; set; }

        public override void Execute(IRouteOptionFilterExecutionContext context)
        {
            decimal? supplierTQI;

            var defaultRouteRuleQualityConfiguration = new ConfigManager().GetDefaultRouteRuleQualityConfiguration();
            QualityConfigurationManager manager = new QualityConfigurationManager();

            supplierTQI = manager.GetCustomerRouteQualityValue(context.SupplierZoneId, context.RoutingDatabase, defaultRouteRuleQualityConfiguration, this.QualityConfigurationId);

            if (supplierTQI.HasValue)
            {
                switch (QualityOptionType)
                {
                    case Filters.QualityOptionType.Equal: context.FilterOption = supplierTQI.Value != QualityOptionValue; break;
                    case Filters.QualityOptionType.Different: context.FilterOption = supplierTQI.Value == QualityOptionValue; break;
                    case Filters.QualityOptionType.GreaterThan: context.FilterOption = supplierTQI.Value <= QualityOptionValue; break;
                    case Filters.QualityOptionType.GreaterThanOrEqual: context.FilterOption = supplierTQI.Value < QualityOptionValue; break;
                    case Filters.QualityOptionType.LessThan: context.FilterOption = supplierTQI.Value >= QualityOptionValue; break;
                    case Filters.QualityOptionType.LessThanOrEqual: context.FilterOption = supplierTQI.Value > QualityOptionValue; break;
                }
            }
        }
    }
}