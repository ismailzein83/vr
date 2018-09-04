using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class GetRPCodeMatchesInput
    {
        public int RoutingDatabaseId { get; set; }
        public long FromZoneId { get; set; }
        public long ToZoneId { get; set; }
        public BaseQueue<RPCodeMatchesByZone> OutputQueue { get; set; }
    }

    public sealed class GetRPCodeMatches : BaseAsyncActivity<GetRPCodeMatchesInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<long> FromZoneId { get; set; }

        [RequiredArgument]
        public InArgument<long> ToZoneId { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<RPCodeMatchesByZone>> OutputQueue { get; set; }

        protected override void DoWork(GetRPCodeMatchesInput inputArgument, AsyncActivityHandle handle)
        {
            RoutingDatabase routingDatabase = new RoutingDatabaseManager().GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            ICodeMatchesDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICodeMatchesDataManager>();
            dataManager.RoutingDatabase = routingDatabase;

            IRPZoneCodeGroupDataManager rpZoneCodeGroupDataManager = RoutingDataManagerFactory.GetDataManager<IRPZoneCodeGroupDataManager>();
            rpZoneCodeGroupDataManager.RoutingDatabase = routingDatabase;

            Dictionary<bool, Dictionary<long, HashSet<string>>> zoneCodeGroupsDict = rpZoneCodeGroupDataManager.GetZoneCodeGroups();
            Dictionary<long, HashSet<string>> saleZoneCodeGroups = zoneCodeGroupsDict != null ? zoneCodeGroupsDict.GetRecord(true) : null;
            Dictionary<long, HashSet<string>> costZoneCodeGroups = zoneCodeGroupsDict != null ? zoneCodeGroupsDict.GetRecord(false) : null;

            var codeMatches = dataManager.GetRPCodeMatchesBySaleZone(inputArgument.FromZoneId, inputArgument.ToZoneId, () => ShouldStop(handle));

            foreach (var codeMatchKvp in codeMatches)
            {
                if (ShouldStop(handle))
                    break;

                var codeMatch = codeMatchKvp.Value;
                HashSet<string> saleCodeGroups = saleZoneCodeGroups != null ? saleZoneCodeGroups.GetRecord(codeMatch.SaleZoneId) : null;

                Dictionary<long, SupplierCodeMatchWithRate> currentSupplierCodeMatchesWithRate = new Dictionary<long, SupplierCodeMatchWithRate>();
                if (codeMatch.SupplierCodeMatches != null)
                {
                    foreach (var item in codeMatch.SupplierCodeMatches)
                    {
                        if (saleCodeGroups != null)
                        {
                            HashSet<string> costCodeGroups = costZoneCodeGroups != null ? costZoneCodeGroups.GetRecord(item.CodeMatch.SupplierZoneId) : null;
                            if (costCodeGroups == null)
                                continue;

                            if (!costCodeGroups.Any(saleCodeGroups.Contains))
                                continue;
                        }

                        if (!currentSupplierCodeMatchesWithRate.ContainsKey(item.CodeMatch.SupplierZoneId))
                            currentSupplierCodeMatchesWithRate.Add(item.CodeMatch.SupplierZoneId, item);
                    }
                }

                RPCodeMatchesByZone codeMatchByZone = new RPCodeMatchesByZone()
               {
                   SaleZoneId = codeMatch.SaleZoneId,
                   SupplierCodeMatches = (currentSupplierCodeMatchesWithRate.Count > 0) ? currentSupplierCodeMatchesWithRate.Values : null,
                   SupplierCodeMatchesBySupplier = (currentSupplierCodeMatchesWithRate.Count > 0) ? GetSupplierCodeMatchesBySupplier(currentSupplierCodeMatchesWithRate.Values) : null
               };

                inputArgument.OutputQueue.Enqueue(codeMatchByZone);
            }

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Getting RP Code Matches is done", null);
        }

        private SupplierCodeMatchesWithRateBySupplier GetSupplierCodeMatchesBySupplier(IEnumerable<SupplierCodeMatchWithRate> supplierCodeMatchesWithRate)
        {
            SupplierCodeMatchesWithRateBySupplier codeMatchBySupplierId = new SupplierCodeMatchesWithRateBySupplier();

            foreach (var supplierCodeMatch in supplierCodeMatchesWithRate)
            {
                List<SupplierCodeMatchWithRate> currentCodeMatches;
                if (!codeMatchBySupplierId.TryGetValue(supplierCodeMatch.CodeMatch.SupplierId, out currentCodeMatches))
                {
                    currentCodeMatches = new List<SupplierCodeMatchWithRate>();
                    codeMatchBySupplierId.Add(supplierCodeMatch.CodeMatch.SupplierId, currentCodeMatches);
                }
                currentCodeMatches.Add(supplierCodeMatch);
            }

            return codeMatchBySupplierId;
        }

        protected override GetRPCodeMatchesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetRPCodeMatchesInput()
            {
                OutputQueue = this.OutputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                FromZoneId = this.FromZoneId.Get(context),
                ToZoneId = this.ToZoneId.Get(context)
            };
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue == null)
                this.OutputQueue.Set(context, new MemoryQueue<RPCodeMatchesByZone>());
            base.OnBeforeExecute(context, handle);
        }

    }
}
