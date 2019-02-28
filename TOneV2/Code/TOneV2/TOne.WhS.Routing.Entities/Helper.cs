using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.WhS.Routing.Entities
{
    public static class Helper
    {
        #region CustomerRoute Serialization

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

        #endregion

        #region Product Route Serialization

        const char RouteOptionSuppliersSeparator = '|';
        const char RouteOptionSupplierPropertiesSeparator = '~';
        const char SupplierZonesSeparator = '#';
        const string SupplierZonesSeparatorAsString = "#";
        const char SupplierZonePropertiesSeparator = '$';
        const char SupplierServicesSeparator = '@';
        const string SupplierServicesSeparatorAsString = "@";

        const char PolicyRouteOptionsSeparator = '|';
        const char PolicyRouteOptionPropertiesSeparator = '~';
        const char RouteOptionsSeparator = '#';
        const string RouteOptionsSeparatorAsString = "#";
        const char RouteOptionPropertiesSeparator = '$';

        /// <summary>
        /// SupplierID~SZ1#SZ2#...#SZn~SupplierStatus~...~SupplierServiceWeight|SupplierID~SZ1#SZ2#...#SZn~SupplierStatus~...~SupplierServiceWeight
        /// SZ1 --> SupplierZoneId$SupplierRate$SupplierServiceID1@SupplierServiceID2@...@SupplierServiceID1SupplierServiceID1n$IsBlocked$SupplierRateId
        /// </summary>
        /// <param name="optionsDetailsBySupplier"></param>
        /// <returns></returns>
        public static string SerializeOptionsDetailsBySupplier(Dictionary<int, RPRouteOptionSupplier> optionsDetailsBySupplier)
        {
            if (optionsDetailsBySupplier == null)
                return string.Empty;

            StringBuilder str = new StringBuilder();

            foreach (var routeOptionSupplier in optionsDetailsBySupplier.Values)
            {
                if (str.Length > 0)
                    str.Append(RouteOptionSuppliersSeparator);

                List<string> serializedSupplierZones = new List<string>();

                foreach (var supplierZone in routeOptionSupplier.SupplierZones)
                {
                    string exactSupplierServiceIdsAsString = supplierZone.ExactSupplierServiceIds != null ? string.Join(SupplierServicesSeparatorAsString, supplierZone.ExactSupplierServiceIds) : string.Empty;
                    string isBlocked = !supplierZone.IsBlocked ? string.Empty : "1";
                    string isForced = !supplierZone.IsForced ? string.Empty : "1";
                    serializedSupplierZones.Add(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}", SupplierZonePropertiesSeparator, supplierZone.SupplierZoneId, supplierZone.SupplierRate,
                                                    exactSupplierServiceIdsAsString, supplierZone.ExecutedRuleId, supplierZone.SupplierRateId, isBlocked, isForced));
                }

                string serializedSupplierZonesAsString = string.Join(SupplierZonesSeparatorAsString, serializedSupplierZones);

                str.AppendFormat("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}", RouteOptionSupplierPropertiesSeparator, routeOptionSupplier.SupplierId, serializedSupplierZonesAsString,
                                    routeOptionSupplier.NumberOfBlockedZones, routeOptionSupplier.NumberOfUnblockedZones, routeOptionSupplier.Percentage, routeOptionSupplier.SupplierServiceWeight);
            }
            return str.ToString();
        }

        public static Dictionary<int, RPRouteOptionSupplier> DeserializeOptionsDetailsBySupplier(string serializedOptionsDetailsBySupplier)
        {
            if (string.IsNullOrEmpty(serializedOptionsDetailsBySupplier))
                return null;

            Dictionary<int, RPRouteOptionSupplier> optionsDetailsBySupplier = new Dictionary<int, RPRouteOptionSupplier>();

            string[] lines = serializedOptionsDetailsBySupplier.Split(RouteOptionSuppliersSeparator);

            foreach (var line in lines)
            {
                string[] parts = line.Split(RouteOptionSupplierPropertiesSeparator);

                var routeOptionSupplier = new RPRouteOptionSupplier();
                routeOptionSupplier.SupplierId = int.Parse(parts[0]);
                routeOptionSupplier.NumberOfBlockedZones = int.Parse(parts[2]);
                routeOptionSupplier.NumberOfUnblockedZones = int.Parse(parts[3]);
                routeOptionSupplier.Percentage = !string.IsNullOrEmpty(parts[4]) ? int.Parse(parts[4]) : default(int?);
                routeOptionSupplier.SupplierServiceWeight = int.Parse(parts[5]);

                string supplierZonesAsString = parts[1];
                if (!string.IsNullOrEmpty(supplierZonesAsString))
                {
                    string[] supplierZones = supplierZonesAsString.Split(SupplierZonesSeparator);
                    routeOptionSupplier.SupplierZones = new List<RPRouteOptionSupplierZone>();

                    foreach (var supplierZone in supplierZones)
                    {
                        string[] supplierZoneParts = supplierZone.Split(SupplierZonePropertiesSeparator);

                        var routeOptionSupplierZone = new RPRouteOptionSupplierZone();
                        //routeOptionSupplierZone.SupplierCode = supplierZoneParts[0];
                        routeOptionSupplierZone.SupplierZoneId = long.Parse(supplierZoneParts[0]);
                        routeOptionSupplierZone.SupplierRate = decimal.Parse(supplierZoneParts[1]);
                        routeOptionSupplierZone.ExecutedRuleId = !string.IsNullOrEmpty(supplierZoneParts[3]) ? int.Parse(supplierZoneParts[3]) : default(int?);
                        routeOptionSupplierZone.SupplierRateId = !string.IsNullOrEmpty(supplierZoneParts[4]) ? long.Parse(supplierZoneParts[4]) : default(long?);

                        if (!string.IsNullOrEmpty(supplierZoneParts[2]))
                            routeOptionSupplierZone.ExactSupplierServiceIds = new HashSet<int>(supplierZoneParts[2].Split(SupplierServicesSeparator).Select(itm => int.Parse(itm)));

                        string isBlockedAsString = supplierZoneParts[5];
                        if (!string.IsNullOrEmpty(isBlockedAsString))
                        {
                            int isBlocked;
                            if (int.TryParse(isBlockedAsString, out isBlocked))
                                routeOptionSupplierZone.IsBlocked = isBlocked > 0;
                        }

                        string isFrocedAsString = supplierZoneParts[6];
                        if (!string.IsNullOrEmpty(isFrocedAsString))
                        {
                            int isForced;
                            if (int.TryParse(isFrocedAsString, out isForced))
                                routeOptionSupplierZone.IsForced = isForced > 0;
                        }

                        routeOptionSupplier.SupplierZones.Add(routeOptionSupplierZone);
                    }
                }

                optionsDetailsBySupplier.Add(routeOptionSupplier.SupplierId, routeOptionSupplier);
            }

            return optionsDetailsBySupplier;
        }

        /// <summary>
        /// PolicyID~S1#S2#...#Sn|PolicyID~S1#S2#...#Sn
        /// S1 --> SupplierId$SupplierRate$...$SupplierZoneMatchHasClosedRate
        /// </summary>
        /// <param name="optionsByPolicy"></param>
        /// <returns></returns>
        public static string SerializeOptionsByPolicy(Dictionary<Guid, IEnumerable<RPRouteOption>> optionsByPolicy)
        {
            if (optionsByPolicy == null)
                return string.Empty;

            StringBuilder str = new StringBuilder();
            foreach (var kvp in optionsByPolicy)
            {
                Guid policyId = kvp.Key;
                IEnumerable<RPRouteOption> routeOptions = kvp.Value;

                if (str.Length > 0)
                    str.Append(PolicyRouteOptionsSeparator);

                List<string> serializedRouteOptions = new List<string>();

                foreach (var routeOption in routeOptions)
                {
                    string supplierZoneMatchHasClosedRate = !routeOption.SupplierZoneMatchHasClosedRate ? string.Empty : "1";
                    string isForced = !routeOption.IsForced ? string.Empty : "1";
                    string supplierZoneId = routeOption.SupplierZoneId.HasValue ? routeOption.SupplierZoneId.ToString() : string.Empty;
                    string supplierServicesIds = routeOption.SupplierServicesIds != null ? string.Join(",", routeOption.SupplierServicesIds) : string.Empty;

                    serializedRouteOptions.Add(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}", RouteOptionPropertiesSeparator, routeOption.SupplierId, routeOption.SupplierRate, routeOption.Percentage,
                                        routeOption.OptionWeight, routeOption.SaleZoneId, routeOption.SupplierServiceWeight, supplierZoneMatchHasClosedRate, Convert.ToInt32(routeOption.SupplierStatus), isForced, supplierZoneId, supplierServicesIds));
                }

                string serializadedRouteOptionsAsString = string.Join(RouteOptionsSeparatorAsString, serializedRouteOptions);

                str.AppendFormat("{1}{0}{2}", PolicyRouteOptionPropertiesSeparator, policyId, serializadedRouteOptionsAsString);
            }
            return str.ToString();
        }

        public static Dictionary<Guid, IEnumerable<RPRouteOption>> DeserializeOptionsByPolicy(string serializedOptionsDetailsBySupplier)
        {
            if (string.IsNullOrEmpty(serializedOptionsDetailsBySupplier))
                return null;

            Dictionary<Guid, IEnumerable<RPRouteOption>> optionsByPolicy = new Dictionary<Guid, IEnumerable<RPRouteOption>>();

            string[] lines = serializedOptionsDetailsBySupplier.Split(PolicyRouteOptionsSeparator);

            foreach (var line in lines)
            {
                string[] parts = line.Split(PolicyRouteOptionPropertiesSeparator);

                Guid policyId = Guid.Parse(parts[0]);
                string routeOptionsAsString = parts[1];

                if (!string.IsNullOrEmpty(routeOptionsAsString))
                {
                    List<RPRouteOption> routeOptionsList = new List<RPRouteOption>();
                    string[] routeOptions = routeOptionsAsString.Split(RouteOptionsSeparator);

                    foreach (var routeOption in routeOptions)
                    {
                        string[] routeOptionParts = routeOption.Split(RouteOptionPropertiesSeparator);

                        var rpRouteOption = new RPRouteOption();
                        rpRouteOption.SupplierId = int.Parse(routeOptionParts[0]);
                        rpRouteOption.SupplierRate = decimal.Parse(routeOptionParts[1]);
                        rpRouteOption.Percentage = !string.IsNullOrEmpty(routeOptionParts[2]) ? int.Parse(routeOptionParts[2]) : default(int?);
                        rpRouteOption.OptionWeight = decimal.Parse(routeOptionParts[3]);
                        rpRouteOption.SaleZoneId = long.Parse(routeOptionParts[4]);
                        rpRouteOption.SupplierServiceWeight = int.Parse(routeOptionParts[5]);
                        rpRouteOption.SupplierStatus = (SupplierStatus)int.Parse(routeOptionParts[7]);

                        string supplierZoneMatchHasClosedRateAsString = routeOptionParts[6];
                        if (!string.IsNullOrEmpty(supplierZoneMatchHasClosedRateAsString))
                        {
                            int supplierZoneMatchHasClosedRate;
                            if (int.TryParse(supplierZoneMatchHasClosedRateAsString, out supplierZoneMatchHasClosedRate))
                                rpRouteOption.SupplierZoneMatchHasClosedRate = supplierZoneMatchHasClosedRate > 0;
                        }

                        string isFrocedAsString = routeOptionParts[8];
                        if (!string.IsNullOrEmpty(isFrocedAsString))
                        {
                            int isForced;
                            if (int.TryParse(isFrocedAsString, out isForced))
                                rpRouteOption.IsForced = isForced > 0;
                        }

                        string supplierZoneIdAsString = routeOptionParts[9];
                        if (!string.IsNullOrEmpty(supplierZoneIdAsString))
                            rpRouteOption.SupplierZoneId = long.Parse(supplierZoneIdAsString);

                        string supplierServicesAsString = routeOptionParts[10];
                        if (!string.IsNullOrEmpty(supplierServicesAsString))
                        {
                            var supplierServicesIds = supplierServicesAsString.Split(',');
                            rpRouteOption.SupplierServicesIds = new HashSet<int>();
                            foreach (var supplierServiceId in supplierServicesIds)
                            {
                                rpRouteOption.SupplierServicesIds.Add(int.Parse(supplierServiceId));
                            }
                        }

                        routeOptionsList.Add(rpRouteOption);
                    }

                    optionsByPolicy.Add(policyId, routeOptionsList);
                }
            }

            return optionsByPolicy;
        }

        #endregion

        #region Public  Methods

        public static string GetConcatenatedSupplierIds(CustomerRoute route)
        {
            HashSet<string> supplierIdsSet = new HashSet<string>();

            if (route != null && route.Options != null && route.Options.Count > 0)
            {
                foreach (var option in route.Options)
                {
                    supplierIdsSet.Add(string.Format("${0}$", option.SupplierId));

                    if (option.Backups != null && option.Backups.Count > 0)
                    {
                        foreach (var backup in option.Backups)
                            supplierIdsSet.Add(string.Format("${0}$", backup.SupplierId));
                    }
                }
            }

            string supplierIds = null;
            if (supplierIdsSet.Count > 0)
                supplierIds = String.Join(",", supplierIdsSet);

            return supplierIds;
        }

        public static RouteSync.Entities.Route BuildRouteFromCustomerRoute(CustomerRoute customerRoute)
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

        public static List<RouteSync.Entities.Route> BuildRoutesFromCustomerRoutes(IEnumerable<CustomerRoute> customerRoutes)
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

        #endregion

        #region Private Methods

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

        #endregion
    }
}