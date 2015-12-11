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
        [RequiredArgument]
        public OutArgument<int> DatabaseId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            RoutingDatabaseInformation information = GetDatabaseInformation(this.Policies.Get(context), this.ProcessType.Get(context));
            IRoutingDatabaseDataManager routingDatabaseManager = RoutingDataManagerFactory.GetDataManager<IRoutingDatabaseDataManager>();
            int databaseId = routingDatabaseManager.CreateDatabase(String.Format("{0}_{1}_{2:yyyyMMdd-HHmm}", this.Type.Get(context), this.ProcessType.Get(context), this.EffectiveTime.Get(context)), this.Type.Get(context), this.ProcessType.Get(context), this.EffectiveTime.Get(context), information);
            this.DatabaseId.Set(context, databaseId);
        }

        private RoutingDatabaseInformation GetDatabaseInformation(List<SupplierZoneToRPOptionPolicy> policies, RoutingProcessType processType)
        {
            RoutingDatabaseInformation information = null;
            switch (processType)
            {
                case RoutingProcessType.RoutingProductRoute:
                    information = new RPRoutingDatabaseInformation()
                    {
                        DefaultPolicyId = policies.Where(p => p.IsDefault).First().ConfigId,
                        SelectedPoliciesIds = policies.Select(p => p.ConfigId).ToArray()
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
    }
}
