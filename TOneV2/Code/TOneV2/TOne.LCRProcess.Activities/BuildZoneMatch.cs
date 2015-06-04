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
        public BaseQueue<SingleSaleCodeMatch> CodeMatches { get; set; }

        public int RoutingDatabaseId { get; set; }
    }

    public sealed class BuildZoneMatch : DependentAsyncActivity<BuildZoneMatchInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<SingleSaleCodeMatch>> CodeMatches { get; set; }

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
                                zoneMatches.Add(preparedCodeMatches.SaleZoneId, GetSupplierZones(preparedCodeMatches));
                            else
                            {
                                supplierZones = zoneMatches[preparedCodeMatches.SaleZoneId];
                                foreach (CodeMatch supplierCodeMatch in preparedCodeMatches.SupplierCodeMatches)
                                {
                                    SupplierZoneInfo supplierZoneInfo;
                                    if (!supplierZones.TryGetValue(supplierCodeMatch.SupplierZoneId, out supplierZoneInfo))
                                        supplierZones.Add(supplierCodeMatch.SupplierZoneId, new SupplierZoneInfo() { IsCodeGroup = preparedCodeMatches.IsMatchingCodeGroup, SupplierId = supplierCodeMatch.SupplierId });
                                }
                            }
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
            dataManager.ApplyZoneMatchesToTempTable(zoneMatches);
        }

        private SupplierZones GetSupplierZones(SingleSaleCodeMatch saleCodeMatch)
        {
            SupplierZones supplierZones = new SupplierZones();
            foreach (CodeMatch codeMatch in saleCodeMatch.SupplierCodeMatches)
                supplierZones.Add(codeMatch.SupplierZoneId, new SupplierZoneInfo() { IsCodeGroup = saleCodeMatch.IsMatchingCodeGroup, SupplierId = codeMatch.SupplierId });


            return supplierZones;
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
