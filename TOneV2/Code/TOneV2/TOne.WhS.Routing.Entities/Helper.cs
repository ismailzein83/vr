using System;
using System.Collections.Generic;
using System.Text;

namespace TOne.WhS.Routing.Entities
{
    public static class Helper
    {
        public static string SerializeOptions(List<RouteOption> options)
        {
            StringBuilder str = new StringBuilder();
            foreach (var option in options)
            {
                if (str.Length > 0)
                    str.Append("|");

                string concatenatedBackups = string.Empty;
                if (option.Backups != null && option.Backups.Count > 0)
                {
                    List<string> serializedBackups = new List<string>();
                    foreach (RouteBackupOption backup in option.Backups)
                    {
                        string backupNumberOfTries = backup.NumberOfTries == 1 ? string.Empty : backup.NumberOfTries.ToString();
                        string isBackupBlocked = !backup.IsBlocked ? string.Empty : "1";
                        string isBackupForced = !backup.IsForced ? string.Empty : "1";
                        string isBackupLossy = !backup.IsLossy ? string.Empty : "1";
                        string serializedBackup = string.Format("{0}${1}${2}${3}${4}${5}${6}", backup.SupplierCode, backup.ExecutedRuleId, backup.SupplierZoneId, backupNumberOfTries, isBackupBlocked, isBackupForced, isBackupLossy);
                        serializedBackups.Add(serializedBackup);
                    }
                    concatenatedBackups = string.Join<string>("@", serializedBackups);
                }
                string numberOfTries = option.NumberOfTries == 1 ? string.Empty : option.NumberOfTries.ToString();
                string isBlocked = !option.IsBlocked ? string.Empty : "1";
                string isForced = !option.IsForced ? string.Empty : "1";
                string isLossy = !option.IsLossy ? string.Empty : "1";
                str.AppendFormat("{0}~{1}~{2}~{3}~{4}~{5}~{6}~{7}~{8}", option.SupplierCode, option.ExecutedRuleId, option.Percentage, option.SupplierZoneId, numberOfTries, isBlocked, isForced, isLossy, concatenatedBackups);
            }
            return str.ToString();
        }

        public static List<RouteOption> DeserializeOptions(string serializedOptions)
        {
            List<RouteOption> options = new List<RouteOption>();

            string[] lines = serializedOptions.Split('|');
            foreach (var line in lines)
            {
                string[] parts = line.Split('~');
                var option = new RouteOption
                {
                    SupplierCode = parts[0],
                    SupplierZoneId = long.Parse(parts[3]),
                };

                int ruleId;
                if (int.TryParse(parts[1], out ruleId))
                    option.ExecutedRuleId = ruleId;

                int percentage;
                if (int.TryParse(parts[2], out percentage))
                    option.Percentage = percentage;

                string numberOfTriesAsString = parts[4];
                if (!string.IsNullOrEmpty(numberOfTriesAsString))
                {
                    int numberOfTries;
                    if (int.TryParse(numberOfTriesAsString, out numberOfTries))
                        option.NumberOfTries = numberOfTries;
                }
                else
                {
                    option.NumberOfTries = 1;
                }

                string isBlockedAsString = parts[5];
                if (!string.IsNullOrEmpty(isBlockedAsString))
                {
                    int isBlocked;
                    if (int.TryParse(isBlockedAsString, out isBlocked))
                        option.IsBlocked = isBlocked > 0;
                }

                string isForcedAsString = parts[6];
                if (!string.IsNullOrEmpty(isForcedAsString))
                {
                    int isForced;
                    if (int.TryParse(isForcedAsString, out isForced))
                        option.IsForced = isForced > 0;
                }

                string isLossyAsString = parts[7];
                if (!string.IsNullOrEmpty(isLossyAsString))
                {
                    int isLossy;
                    if (int.TryParse(isLossyAsString, out isLossy))
                        option.IsLossy = isLossy > 0;
                }

                string backupsAsString = parts[8];
                if (!string.IsNullOrEmpty(backupsAsString))
                {
                    option.Backups = new List<RouteBackupOption>();
                    string[] serializedBackups = backupsAsString.Split('@');

                    foreach (string serializedBackup in serializedBackups)
                    {
                        string[] serializedBackupParts = serializedBackup.Split('$');
                        RouteBackupOption backup = new RouteBackupOption()
                        {
                            SupplierCode = serializedBackupParts[0],
                            SupplierZoneId = long.Parse(serializedBackupParts[2]),
                        };

                        int backupRuleId;
                        if (int.TryParse(serializedBackupParts[1], out backupRuleId))
                            backup.ExecutedRuleId = backupRuleId;

                        string backupNumberOfTriesAsString = serializedBackupParts[3];
                        if (!string.IsNullOrEmpty(backupNumberOfTriesAsString))
                        {
                            int backupNumberOfTries;
                            if (int.TryParse(backupNumberOfTriesAsString, out backupNumberOfTries))
                                backup.NumberOfTries = backupNumberOfTries;
                        }
                        else
                        {
                            backup.NumberOfTries = 1;
                        }

                        string isBackupBlockedAsString = serializedBackupParts[4];
                        if (!string.IsNullOrEmpty(isBackupBlockedAsString))
                        {
                            int isBlocked;
                            if (int.TryParse(isBackupBlockedAsString, out isBlocked))
                                backup.IsBlocked = isBlocked > 0;
                        }

                        string isBackupForcedAsString = serializedBackupParts[5];
                        if (!string.IsNullOrEmpty(isBackupForcedAsString))
                        {
                            int isForced;
                            if (int.TryParse(isBackupForcedAsString, out isForced))
                                backup.IsForced = isForced > 0;
                        }

                        string isBackupLossyAsString = serializedBackupParts[6];
                        if (!string.IsNullOrEmpty(isBackupLossyAsString))
                        {
                            int isLossy;
                            if (int.TryParse(isBackupLossyAsString, out isLossy))
                                backup.IsLossy = isLossy > 0;
                        }

                        option.Backups.Add(backup);
                    }
                }

                options.Add(option);

            }

            return options;
        }

        public static TOne.WhS.RouteSync.Entities.Route BuildRouteFromCustomerRoute(CustomerRoute customerRoute)
        {
            TOne.WhS.RouteSync.Entities.Route route = new TOne.WhS.RouteSync.Entities.Route
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


                    RouteSync.Entities.RouteOption routeSyncOption = new RouteSync.Entities.RouteOption
                    {
                        SupplierId = customerRouteOption.SupplierId.ToString(),
                        SupplierRate = customerRouteOption.SupplierRate,
                        Percentage = customerRouteOption.Percentage,
                        IsBlocked = customerRouteOption.IsBlocked,
                        NumberOfTries = customerRouteOption.NumberOfTries
                    };

                    if (customerRouteOption.Backups != null && customerRouteOption.Backups.Count > 0)
                    {
                        routeSyncOption.Backups = new List<RouteSync.Entities.BackupRouteOption>();
                        foreach (RouteBackupOption backup in customerRouteOption.Backups)
                        {
                            RouteSync.Entities.BackupRouteOption backupRouteSyncOption = new RouteSync.Entities.BackupRouteOption()
                            {
                                IsBlocked = backup.IsBlocked,
                                NumberOfTries = backup.NumberOfTries,
                                SupplierId = backup.SupplierId.ToString(),
                                SupplierRate = backup.SupplierRate
                            };
                            routeSyncOption.Backups.Add(backupRouteSyncOption);
                        }
                    }
                    route.Options.Add(routeSyncOption);
                }
            }
            return route;
        }

        public static List<TOne.WhS.RouteSync.Entities.Route> BuildRoutesFromCustomerRoutes(IEnumerable<CustomerRoute> customerRoutes)
        {
            if (customerRoutes == null)
                return null;

            List<TOne.WhS.RouteSync.Entities.Route> routes = new List<TOne.WhS.RouteSync.Entities.Route>();
            foreach (CustomerRoute customerRoute in customerRoutes)
                routes.Add(BuildRouteFromCustomerRoute(customerRoute));

            return routes;
        }

        public static RouteOptionRuleTarget CreateRouteOptionRuleTarget(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate, IRouteBackupOptionSettings backup, int? percentage)
        {
            RouteOptionRuleTarget routeOptionRuleTarget = CreateOption<RouteOptionRuleTarget>(routeRuleTarget, supplierCodeMatchWithRate, backup);
            routeOptionRuleTarget.NumberOfTries = backup != null ? backup.NumberOfTries : 1;
            routeOptionRuleTarget.Percentage = percentage;
            routeOptionRuleTarget.Backups = new List<RouteBackupOptionRuleTarget>();
            return routeOptionRuleTarget;
        }

        public static RouteOptionRuleTarget CreateRouteOptionRuleTarget(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate, IRouteOptionSettings option)
        {
            RouteOptionRuleTarget routeOptionRuleTarget = CreateOption<RouteOptionRuleTarget>(routeRuleTarget, supplierCodeMatchWithRate, option);
            if (option != null)
            {
                routeOptionRuleTarget.NumberOfTries = option.NumberOfTries;
                routeOptionRuleTarget.Percentage = option.Percentage;
            }
            else
            {
                routeOptionRuleTarget.NumberOfTries = 1;
            }

            routeOptionRuleTarget.Backups = new List<RouteBackupOptionRuleTarget>();
            return routeOptionRuleTarget;
        }

        public static RouteBackupOptionRuleTarget CreateRouteBackupOptionRuleTarget(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate, IRouteBackupOptionSettings backup)
        {
            RouteBackupOptionRuleTarget routeBackupOptionRuleTarget = CreateOption<RouteBackupOptionRuleTarget>(routeRuleTarget, supplierCodeMatchWithRate, backup);
            if (backup != null)
                routeBackupOptionRuleTarget.NumberOfTries = backup.NumberOfTries;
            else
                routeBackupOptionRuleTarget.NumberOfTries = 1;

            return routeBackupOptionRuleTarget;
        }

        public static RouteOptionRuleTarget CreateRouteOptionRuleTargetFromBackup(RouteBackupOptionRuleTarget routeBackupOptionRuleTarget, int? percentage)
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
                BlockOption = routeBackupOptionRuleTarget.BlockOption,
                IsForced = routeBackupOptionRuleTarget.IsForced,
                Percentage = percentage,
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
                NumberOfTries = routeOptionRuleTarget.NumberOfTries,
                BlockOption = routeOptionRuleTarget.BlockOption,
                IsForced = routeOptionRuleTarget.IsForced
            };
        }

        public static RouteOptionEvaluatedStatus? GetEvaluatedStatus(bool isBlocked, bool isLossy, bool isForced, int? ExecutedRuleId)
        {
            if (isBlocked)
                return RouteOptionEvaluatedStatus.Blocked;

            if (isLossy)
                return RouteOptionEvaluatedStatus.Lossy;

            if (isForced)
                return RouteOptionEvaluatedStatus.Forced;

            if (ExecutedRuleId.HasValue)
                return RouteOptionEvaluatedStatus.MarketPrice;

            return null;
        }

        private static T CreateOption<T>(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate, object optionSettings) where T : BaseRouteOptionRuleTarget
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
