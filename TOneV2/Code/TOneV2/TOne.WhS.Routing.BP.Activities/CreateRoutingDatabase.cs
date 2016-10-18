using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.BP.Activities
{

    public sealed class CreateRoutingDatabase : CodeActivity
    {
        [RequiredArgument]
        public InArgument<RoutingDatabaseType> Type { get; set; }

        [RequiredArgument]
        public InArgument<RoutingProcessType> ProcessType { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveTime { get; set; }

        [RequiredArgument]
        public InArgument<List<SupplierZoneToRPOptionPolicy>> Policies { get; set; }

        public InArgument<bool> IncludeBlockedSupplierZones { get; set; }

        [RequiredArgument]
        public OutArgument<int> DatabaseId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            RoutingDatabaseInformation information = GetDatabaseInformation(this.ProcessType.Get(context), this.Policies.Get(context), this.IncludeBlockedSupplierZones.Get(context));
            RoutingDatabaseSettings settings = BuildRoutingDatabaseSettings();

            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            int databaseId = routingDatabaseManager.CreateDatabase(String.Format("{0}_{1}_{2:yyyyMMdd-HHmm}", this.Type.Get(context), this.ProcessType.Get(context), this.EffectiveTime.Get(context)), this.Type.Get(context), this.ProcessType.Get(context), this.EffectiveTime.Get(context), information, settings);
            
            this.DatabaseId.Set(context, databaseId);
        }

        private RoutingDatabaseInformation GetDatabaseInformation(RoutingProcessType processType, List<SupplierZoneToRPOptionPolicy> policies, bool includeBlockedSupplierZones)
        {
            RoutingDatabaseInformation information = null;
            switch (processType)
            {
                case RoutingProcessType.RoutingProductRoute:
                    information = new RPRoutingDatabaseInformation()
                    {
                        DefaultPolicyId = policies.Where(p => p.IsDefault).First().ConfigId,
                        SelectedPoliciesIds = policies.Select(p => p.ConfigId).ToArray(),
                        IncludeBlockedSupplierZones = includeBlockedSupplierZones
                    };
                    break;
                case RoutingProcessType.CustomerRoute:
                    information = null;
                    break;
                default:
                    information = null;
                    break;
            }
            return information;
        }

        /// <summary>
        /// For futur use
        /// </summary>
        /// <returns></returns>
        private RoutingDatabaseSettings BuildRoutingDatabaseSettings()
        {
            RoutingDatabaseSettings settings = new RoutingDatabaseSettings() { };
            return settings;
        }
    }
}
