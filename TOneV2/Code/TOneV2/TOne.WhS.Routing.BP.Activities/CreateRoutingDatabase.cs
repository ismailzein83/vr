using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using System.Threading;

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
            RoutingProcessType processType = this.ProcessType.Get(context);
            RoutingDatabaseInformation information = GetDatabaseInformation(processType, this.Policies.Get(context));
            RoutingDatabaseSettings settings = BuildRoutingDatabaseSettings();

            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            int databaseId = routingDatabaseManager.CreateDatabase(String.Format("{0}_{1}_{2:yyyyMMdd-HHmm}", this.Type.Get(context), this.ProcessType.Get(context), this.EffectiveTime.Get(context)), this.Type.Get(context), this.ProcessType.Get(context), this.EffectiveTime.Get(context), information, settings);

            Thread.Sleep(2000);

            IRoutingDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRoutingDataManager>();
            dataManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(databaseId);

            if (processType == RoutingProcessType.CustomerRoute)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                IEnumerable<CarrierAccountInfo> carrierAccounts = carrierAccountManager.GetCarrierAccountInfo(null);

                if (carrierAccounts != null)
                    dataManager.StoreCarrierAccounts(carrierAccounts.ToList());
            }

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            Dictionary<long, SaleZone> saleZones = saleZoneManager.GetCachedSaleZones();

            if (saleZones != null)
                dataManager.StoreSaleZones(saleZones.Values.ToList());

            this.DatabaseId.Set(context, databaseId);
        }

        private RoutingDatabaseInformation GetDatabaseInformation(RoutingProcessType processType, List<SupplierZoneToRPOptionPolicy> policies)
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
