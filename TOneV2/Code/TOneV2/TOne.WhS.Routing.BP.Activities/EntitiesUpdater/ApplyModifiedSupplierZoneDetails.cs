using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.BP.Activities
{
    public class ApplyModifiedSupplierZoneDetailsInput
    {
        public RoutingDatabase RoutingDatabase { get; set; }
        public BaseQueue<List<SupplierZoneDetail>> InputQueue { get; set; }
    }

    public sealed class ApplyModifiedSupplierZoneDetails : DependentAsyncActivity<ApplyModifiedSupplierZoneDetailsInput>
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<List<SupplierZoneDetail>>> InputQueue { get; set; }

        protected override void DoWork(ApplyModifiedSupplierZoneDetailsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISupplierZoneDetailsDataManager supplierZoneDetailsDataManager = RoutingDataManagerFactory.GetDataManager<ISupplierZoneDetailsDataManager>();
            supplierZoneDetailsDataManager.RoutingDatabase = inputArgument.RoutingDatabase;

            int partialRoutesUpdateBatchSize = new ConfigManager().GetPartialRoutesUpdateBatchSize();

            List<SupplierZoneDetail> supplierZoneDetailToUpdate = new List<SupplierZoneDetail>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((supplierZoneDetails) =>
                    {
                        if (supplierZoneDetails != null)
                        {
                            supplierZoneDetailToUpdate.AddRange(supplierZoneDetails);

                            if (supplierZoneDetailToUpdate.Count > partialRoutesUpdateBatchSize)
                            {
                                supplierZoneDetailsDataManager.UpdateSupplierZoneDetails(supplierZoneDetailToUpdate);
                                supplierZoneDetailToUpdate = new List<SupplierZoneDetail>();
                            }
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);
            });

            if (supplierZoneDetailToUpdate.Count > 0)
                supplierZoneDetailsDataManager.UpdateSupplierZoneDetails(supplierZoneDetailToUpdate);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Updating Modified Supplier Zone Details is done", null);
        }

        protected override ApplyModifiedSupplierZoneDetailsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyModifiedSupplierZoneDetailsInput
            {
                RoutingDatabase = this.RoutingDatabase.Get(context),
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}