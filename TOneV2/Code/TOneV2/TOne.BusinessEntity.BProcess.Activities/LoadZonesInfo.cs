using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using TOne.BusinessEntity.Entities;
using TOne.BusinessEntity.Data;
using TOne.Business;

namespace TOne.BusinessEntity.BProcess.Activities
{
    #region Argument Classes

    public class LoadZonesInfoInput
    {
        public DateTime EffectiveTime { get; set; }

        public bool IsFuture { get; set; }

        public List<CarrierAccountInfo> ActiveSuppliers { get; set; }

        public BaseQueue<List<ZoneInfo>> OutputQueue { get; set; }
    }

    #endregion

    public sealed class LoadZonesInfo : BaseAsyncActivity<LoadZonesInfoInput>
    {
        public InArgument<DateTime> EffectiveTime { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<List<CarrierAccountInfo>> ActiveSuppliers { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<List<ZoneInfo>>> OutputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if(this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<List<ZoneInfo>>());
            base.OnBeforeExecute(context, handle);
        }

        protected override LoadZonesInfoInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadZonesInfoInput
            {
                EffectiveTime = this.EffectiveTime.Get(context),
                IsFuture = this.IsFuture.Get(context),
                ActiveSuppliers = this.ActiveSuppliers.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void DoWork(LoadZonesInfoInput inputArgument, AsyncActivityHandle handle)
        {
            IZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneDataManager>();
            dataManager.LoadZonesInfo(inputArgument.EffectiveTime, inputArgument.IsFuture, inputArgument.ActiveSuppliers, ConfigParameterManager.Current.GetLoadZoneBatchSize(), (zoneInfoBatch) =>
                {
                    inputArgument.OutputQueue.Enqueue(zoneInfoBatch);
                });
        }
    }
}
