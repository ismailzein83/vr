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
            IEnumerable<RoutingDatabase> routingdatabases = routingDatabaseManager.GetRoutingDatabases(processType, databaseType);

            if (routingdatabases != null)
            {
                var orderedDatabases = routingdatabases.OrderByDescending(itm => itm.CreatedTime);
                int maximumExecutionTimeInSeconds;
                List<int> excludedDatabaseIds = GetExculdedDatabases(orderedDatabases, processType, databaseType, out maximumExecutionTimeInSeconds);
                DateTime now = DateTime.Now;

                foreach (var db in routingdatabases)
                {
                    if ((db.IsReady && !excludedDatabaseIds.Contains(db.ID)) || (!db.IsReady && (now - db.CreatedTime).TotalSeconds > maximumExecutionTimeInSeconds))
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
            TOne.WhS.Routing.Business.ConfigManager configManager = new Business.ConfigManager();
            RouteDatabaseConfiguration routeDatabaseConfiguration;
            switch (processType)
            {
                case RoutingProcessType.CustomerRoute: routeDatabaseConfiguration = configManager.GetCustomerRouteConfiguration(); break;
                case RoutingProcessType.RoutingProductRoute: routeDatabaseConfiguration = configManager.GetProductRouteConfiguration(); break;
                default: throw new Exception(string.Format("Unsupported RoutingProcessType: {0}", processType));
            }

            switch (routeDatabaseConfiguration.TimeUnit)
            {
                case TimeUnit.Minutes: maximumExecutionTimeInSeconds = routeDatabaseConfiguration.MaximumEstimatedExecutionTime * 60; break;
                case TimeUnit.Hours: maximumExecutionTimeInSeconds = routeDatabaseConfiguration.MaximumEstimatedExecutionTime * 3600; break;
                default: throw new Exception(string.Format("Unsupported TimeUnit: {0}", routeDatabaseConfiguration.TimeUnit));
            }

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