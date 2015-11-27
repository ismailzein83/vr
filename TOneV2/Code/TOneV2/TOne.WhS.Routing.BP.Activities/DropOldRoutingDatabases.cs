using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using System.Configuration;
using TOne.Business;
using Vanrise.Common;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Routing.BP.Activities
{

    public sealed class DropOldRoutingDatabases : CodeActivity
    {
        static TimeSpan s_DropOldDatabasesInterval;

        static DropOldRoutingDatabases()
        {
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["DropOldRoutingDatabasesInterval"], out s_DropOldDatabasesInterval))
                s_DropOldDatabasesInterval = TimeSpan.FromHours(2);
        }

        [RequiredArgument]
        public InArgument<RoutingProcessType> ProcessType { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IRoutingDatabaseDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRoutingDatabaseDataManager>();
            List<RoutingDatabase> routingdatabases = dataManager.GetNotDeletedDatabases(ProcessType.Get(context));
            var orderedDatabases = routingdatabases.OrderByDescending(itm => itm.EffectiveTime);
            List<int> excludedDatabaseIds = new List<int>();

            AddedExcludedDatabaseIds(excludedDatabaseIds, orderedDatabases, RoutingDatabaseType.Current);
            AddedExcludedDatabaseIds(excludedDatabaseIds, orderedDatabases, RoutingDatabaseType.Future);

            if (routingdatabases != null)
            {
                TimeSpan dropOldDatabasesInterval = ConfigParameterManager.Current.GetDropOldRoutingDatabasesInterval();
                foreach (var db in routingdatabases)
                {
                    if (!excludedDatabaseIds.Contains(db.ID) && (DateTime.Now - db.EffectiveTime) > s_DropOldDatabasesInterval)
                    {
                        try
                        {
                            dataManager.DropDatabase(db.ID);
                        }
                        catch (Exception ex)
                        {
                            context.WriteTrackingMessage(LogEntryType.Warning, "Exception occurred while deleting the routing database Id '{0}', db Title: '{1}'. Error message: {2}", db.ID, db.Title, ex.Message);
                        }
                    }
                }
            }
        }

        void AddedExcludedDatabaseIds(List<int> excludedDatabaseIds, IEnumerable<RoutingDatabase> orderedDatabases, RoutingDatabaseType type)
        {
            int addedItem = 0;
            foreach (var db in orderedDatabases)
            {
                if (db.Type == type && db.IsReady)
                {
                    excludedDatabaseIds.Add(db.ID);
                    addedItem++;
                    if (addedItem == 2)
                        return;
                }
            }
        }
    }
}