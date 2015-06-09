using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{

    public class BuildZoneMatchInput
    {
        public BaseQueue<ZoneMatchWithCodeGroup> CodeMatches { get; set; }

        public int RoutingDatabaseId { get; set; }
    }

    public sealed class BuildZoneMatch : DependentAsyncActivity<BuildZoneMatchInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<ZoneMatchWithCodeGroup>> CodeMatches { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override void DoWork(BuildZoneMatchInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IZoneMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<IZoneMatchDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            SaleZoneMatches zoneMatches = new SaleZoneMatches();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.CodeMatches.TryDequeue(
                        (preparedCodeMatches) =>
                        {
                            SupplierZones supplierZones;
                            if (!zoneMatches.TryGetValue(preparedCodeMatches.SaleZoneId, out supplierZones))
                            {
                                supplierZones = new SupplierZones();
                                zoneMatches.Add(preparedCodeMatches.SaleZoneId, supplierZones);
                            }

                            foreach (CodeMatch supplierCodeMatch in preparedCodeMatches.SupplierCodeMatches)
                            {
                                if (!supplierZones.ContainsKey(supplierCodeMatch.SupplierZoneId))
                                    supplierZones.Add(supplierCodeMatch.SupplierZoneId, new SupplierZoneInfo() { IsCodeGroup = preparedCodeMatches.IsMatchingCodeGroup, SupplierId = supplierCodeMatch.SupplierId });
                            }
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
            dataManager.ApplyZoneMatchesToTempTable(zoneMatches);
        }

        protected override BuildZoneMatchInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new BuildZoneMatchInput()
            {
                CodeMatches = this.CodeMatches.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
    }
}
