using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.Entities;
using TOne.LCR.Entities;
using TOne.LCR.Data;
using Vanrise.Queueing;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes

    public class SaveZoneRatesToDBInput
    {
        public int RoutingDatabaseId { get; set; }

        public BaseQueue<ZoneRateBatch> InputQueue { get; set; }
    }

    #endregion

    public sealed class SaveZoneRatesToDB : DependentAsyncActivity<SaveZoneRatesToDBInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<ZoneRateBatch>> InputQueue { get; set; }

        protected override SaveZoneRatesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new SaveZoneRatesToDBInput
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                InputQueue = this.InputQueue.Get(context)
            };
        }

        protected override void DoWork(SaveZoneRatesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IZoneRateDataManager dataManager = LCRDataManagerFactory.GetDataManager<IZoneRateDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (zoneRateBatch) =>
                        {
                            dataManager.InsertZoneRates(zoneRateBatch.IsSupplierZoneRateBatch, zoneRateBatch.ZoneRates);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }
    }
}