using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public static class Helper
    {
        public static Route BuildRouteFromCustomerRoute(CustomerRoute customerRoute)
        {
            Route route = new Route
            {
                CustomerId = customerRoute.CustomerId.ToString(),
                SaleZoneId = customerRoute.SaleZoneId,
                SaleRate = customerRoute.Rate,
                Code = customerRoute.Code
            };
            if (customerRoute.Options != null)
            {
                route.Options = new List<RouteSync.Entities.RouteOption>();
                foreach (var customerRouteOption in customerRoute.Options)
                {
                    if (customerRouteOption.IsBlocked)
                        continue;

                    route.Options.Add(new RouteSync.Entities.RouteOption
                    {
                        SupplierId = customerRouteOption.SupplierId.ToString(),
                        SupplierRate = customerRouteOption.SupplierRate,
                        Percentage = customerRouteOption.Percentage,
                        IsBlocked = customerRouteOption.IsBlocked,
                        NumberOfTries = customerRouteOption.NumberOfTries
                    });
                }
            }
            return route;
        }

        public static List<Route> BuildRoutesFromCustomerRoutes(IEnumerable<CustomerRoute> customerRoutes)
        {
            if (customerRoutes == null)
                return null;

            List<Route> routes = new List<Route>();
            foreach (CustomerRoute customerRoute in customerRoutes)
                routes.Add(BuildRouteFromCustomerRoute(customerRoute));

            return routes;
        }

        public static RouteBackupOptionRuleTarget CreateRouteBackupOptionRuleTarget(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate, IRouteBackupOptionSettings backup)
        {
            RouteBackupOptionRuleTarget routeBackupOptionRuleTarget = CreateOption<RouteBackupOptionRuleTarget>(routeRuleTarget, supplierCodeMatchWithRate, backup);
            routeBackupOptionRuleTarget.NumberOfTries = backup.NumberOfTries;
            return routeBackupOptionRuleTarget;
        }

        public static RouteOptionRuleTarget CreateRouteOptionRuleTarget(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate, IRouteOptionSettings option)
        {
            RouteOptionRuleTarget routeOptionRuleTarget = CreateOption<RouteOptionRuleTarget>(routeRuleTarget, supplierCodeMatchWithRate, option);
            routeOptionRuleTarget.NumberOfTries = option.NumberOfTries;
            routeOptionRuleTarget.Percentage = option.Percentage;
            routeOptionRuleTarget.Backups = new List<RouteBackupOptionRuleTarget>();
            return routeOptionRuleTarget;
        }

        public static RouteOptionRuleTarget CreateRouteOptionRuleTargetFromBackup(RouteBackupOptionRuleTarget routeBackupOptionRuleTarget, IRouteOptionSettings option)
        {
            return new RouteOptionRuleTarget()
            {
                RouteTarget = routeBackupOptionRuleTarget.RouteTarget,
                SupplierId = routeBackupOptionRuleTarget.SupplierId,
                SupplierCode = routeBackupOptionRuleTarget.SupplierCode,
                SupplierZoneId = routeBackupOptionRuleTarget.SupplierZoneId,
                SupplierRate = routeBackupOptionRuleTarget.SupplierRate,
                EffectiveOn = routeBackupOptionRuleTarget.EffectiveOn,
                IsEffectiveInFuture = routeBackupOptionRuleTarget.IsEffectiveInFuture,
                ExactSupplierServiceIds = routeBackupOptionRuleTarget.ExactSupplierServiceIds,
                SupplierServiceIds = routeBackupOptionRuleTarget.SupplierServiceIds,
                SupplierServiceWeight = routeBackupOptionRuleTarget.SupplierServiceWeight,
                SupplierRateId = routeBackupOptionRuleTarget.SupplierRateId,
                SupplierRateEED = routeBackupOptionRuleTarget.SupplierRateEED,
                OptionSettings = routeBackupOptionRuleTarget.OptionSettings,
                NumberOfTries = routeBackupOptionRuleTarget.NumberOfTries,
                Percentage = option.Percentage,
                Backups = new List<RouteBackupOptionRuleTarget>()
            };
        }

        public static RouteBackupOptionRuleTarget CreateRouteBackupOptionRuleTargetFromOption(RouteOptionRuleTarget routeOptionRuleTarget)
        {
            return new RouteBackupOptionRuleTarget()
            {
                RouteTarget = routeOptionRuleTarget.RouteTarget,
                SupplierId = routeOptionRuleTarget.SupplierId,
                SupplierCode = routeOptionRuleTarget.SupplierCode,
                SupplierZoneId = routeOptionRuleTarget.SupplierZoneId,
                SupplierRate = routeOptionRuleTarget.SupplierRate,
                EffectiveOn = routeOptionRuleTarget.EffectiveOn,
                IsEffectiveInFuture = routeOptionRuleTarget.IsEffectiveInFuture,
                ExactSupplierServiceIds = routeOptionRuleTarget.ExactSupplierServiceIds,
                SupplierServiceIds = routeOptionRuleTarget.SupplierServiceIds,
                SupplierServiceWeight = routeOptionRuleTarget.SupplierServiceWeight,
                SupplierRateId = routeOptionRuleTarget.SupplierRateId,
                SupplierRateEED = routeOptionRuleTarget.SupplierRateEED,
                OptionSettings = routeOptionRuleTarget.OptionSettings,
                NumberOfTries = routeOptionRuleTarget.NumberOfTries
            };
        }

        static T CreateOption<T>(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate, object optionSettings) where T : BaseRouteOptionRuleTarget
        {
            var supplierCodeMatch = supplierCodeMatchWithRate.CodeMatch;

            T option = Activator.CreateInstance<T>();
            option.RouteTarget = routeRuleTarget;
            option.SupplierId = supplierCodeMatch.SupplierId;
            option.SupplierCode = supplierCodeMatch.SupplierCode;
            option.SupplierZoneId = supplierCodeMatch.SupplierZoneId;
            option.SupplierRate = supplierCodeMatchWithRate.RateValue;
            option.EffectiveOn = routeRuleTarget.EffectiveOn;
            option.IsEffectiveInFuture = routeRuleTarget.IsEffectiveInFuture;
            option.ExactSupplierServiceIds = supplierCodeMatchWithRate.ExactSupplierServiceIds;
            option.SupplierServiceIds = supplierCodeMatchWithRate.SupplierServiceIds;
            option.SupplierServiceWeight = supplierCodeMatchWithRate.SupplierServiceWeight;
            option.SupplierRateId = supplierCodeMatchWithRate.SupplierRateId;
            option.SupplierRateEED = supplierCodeMatchWithRate.SupplierRateEED;
            option.OptionSettings = optionSettings;

            return option;
        }
    }
}
