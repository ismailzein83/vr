using System;
using System.Collections.Generic;
using System.Linq;
using System.Activities;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using TOne.WhS.Routing.Business;
using Vanrise.Common.Business;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class DropOldRoutingDatabases : CodeActivity
    {
        [RequiredArgument]
        public InArgument<RoutingProcessType> ProcessType { get; set; }

        [RequiredArgument]
        public InArgument<RoutingDatabaseType> DatabaseType { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            RoutingProcessType processType = context.GetValue<RoutingProcessType>(ProcessType);
            RoutingDatabaseType databaseType = context.GetValue<RoutingDatabaseType>(DatabaseType);

            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            IEnumerable<RoutingDatabase> routingdatabases = routingDatabaseManager.GetRoutingDatabasesReady(processType, databaseType);

            if (routingdatabases != null)
            {
                var orderedDatabases = routingdatabases.OrderByDescending(itm => itm.CreatedTime);
                int maximumExecutionTimeInSeconds;
                List<int> excludedDatabaseIds = GetExculdedDatabases(orderedDatabases, processType, databaseType, out maximumExecutionTimeInSeconds);
                DateTime now = DateTime.Now;

                foreach (var db in routingdatabases)
                {
                    if (!excludedDatabaseIds.Contains(db.ID) || (!db.IsReady && (now - db.CreatedTime).TotalSeconds > maximumExecutionTimeInSeconds))
                    {
                        try
                        {
                            routingDatabaseManager.DropDatabase(db);
                        }
                        catch (Exception ex)
                        {
                            context.WriteTrackingMessage(LogEntryType.Warning, "Exception occurred while deleting the routing database Id '{0}', db Title: '{1}'. Error message: {2}", db.ID, db.Title, ex.Message);
                        }
                    }
                }
            }
        }

        List<int> GetExculdedDatabases(IEnumerable<RoutingDatabase> orderedDatabases, RoutingProcessType processType, RoutingDatabaseType databaseType, out int maximumExecutionTimeInSeconds)
        {
            SettingManager settingManager = new SettingManager();
            RouteSettingsData settings = settingManager.GetSetting<RouteSettingsData>(Routing.Business.Constants.RouteSettings);

            if (settings.RouteDatabasesToKeep == null)
                throw new NullReferenceException("settings.RouteDatabasesToKeep");

            RouteDatabaseConfiguration routeDatabaseConfiguration;
            switch (processType)
            {
                case RoutingProcessType.CustomerRoute:
                    routeDatabaseConfiguration = settings.RouteDatabasesToKeep.CustomerRouteConfiguration;
                    switch (settings.RouteDatabasesToKeep.CustomerRouteConfiguration.TimeUnit)
                    {
                        case TimeUnit.Minutes: maximumExecutionTimeInSeconds = settings.RouteDatabasesToKeep.CustomerRouteConfiguration.MaximumEstimatedExecutionTime * 60; break;
                        case TimeUnit.Hours: maximumExecutionTimeInSeconds = settings.RouteDatabasesToKeep.CustomerRouteConfiguration.MaximumEstimatedExecutionTime * 3600; break;
                        default: throw new Exception(string.Format("Unsupported TimeUnit: {0}", settings.RouteDatabasesToKeep.CustomerRouteConfiguration.TimeUnit));
                    }

                    break;
                case RoutingProcessType.RoutingProductRoute:
                    routeDatabaseConfiguration = settings.RouteDatabasesToKeep.ProductRouteConfiguration;
                    switch (settings.RouteDatabasesToKeep.ProductRouteConfiguration.TimeUnit)
                    {
                        case TimeUnit.Minutes: maximumExecutionTimeInSeconds = settings.RouteDatabasesToKeep.ProductRouteConfiguration.MaximumEstimatedExecutionTime * 60; break;
                        case TimeUnit.Hours: maximumExecutionTimeInSeconds = settings.RouteDatabasesToKeep.ProductRouteConfiguration.MaximumEstimatedExecutionTime * 3600; break;
                        default: throw new Exception(string.Format("Unsupported TimeUnit: {0}", settings.RouteDatabasesToKeep.ProductRouteConfiguration.TimeUnit));
                    }
                    break;
                default: throw new Exception(string.Format("Unsupported RoutingProcessType: {0}", processType));
            }

            if (routeDatabaseConfiguration == null)
                throw new NullReferenceException("routeDatabaseConfiguration");

            int databaseCount;
            switch (databaseType)
            {
                case RoutingDatabaseType.Current: databaseCount = routeDatabaseConfiguration.CurrentDBToKeep; break;
                case RoutingDatabaseType.Future: databaseCount = routeDatabaseConfiguration.FutureDBToKeep; break;
                //case RoutingDatabaseType.SpecificDate: databaseCount = routeDatabaseConfiguration.SpecificDBToKeep; break;
                default: throw new Exception(string.Format("Unsupported RoutingDatabaseType: {0}", databaseType));
            }


            int excludedItem = 0;
            List<int> excludedDatabaseIds = new List<int>();

            foreach (var db in orderedDatabases)
            {
                if (excludedItem == databaseCount)
                    break;

                if (db.IsReady)
                {
                    excludedDatabaseIds.Add(db.ID);
                    excludedItem++;
                }
            }
            return excludedDatabaseIds;
        }
    }
}