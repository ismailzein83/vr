using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes

    public class CreateAndFillZoneRateTableInput
    {
        public bool IsFuture { get; set; }

        public bool ForSupplier { get; set; }

        public DateTime EffectiveOn { get; set; }
    }

    #endregion

    public sealed class CreateAndFillZoneRateTable : BaseAsyncActivity<CreateAndFillZoneRateTableInput>
    {
        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<bool> ForSupplier { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveOn { get; set; }

        protected override CreateAndFillZoneRateTableInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new CreateAndFillZoneRateTableInput
            {
                IsFuture = this.IsFuture.Get(context),
                ForSupplier = this.ForSupplier.Get(context),
                EffectiveOn = this.EffectiveOn.Get(context)
            };
        }

        protected override void DoWork(CreateAndFillZoneRateTableInput inputArgument, AsyncActivityHandle handle)
        {
            IZoneRateDataManager dataManager = LCRDataManagerFactory.GetDataManager<IZoneRateDataManager>();
            dataManager.CreateAndFillTable(inputArgument.IsFuture, inputArgument.ForSupplier, inputArgument.EffectiveOn);
        }
    }
}
