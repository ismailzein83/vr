using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.Entities;
using TOne.LCR.Entities;

namespace TOne.LCRProcess.Activities
{

    public sealed class GetZoneRateCustomer : BaseAsyncActivity<GetZoneRateSupplier>
    {
        public class LoadCodesForActiveSuppliersInput
        {
            public DateTime EffectiveOn { get; set; }

            public List<SupplierCodeInfo> SuppliersCodeInfo { get; set; }

            public bool OnlySuppliersWithUpdatedCodes { get; set; }

            public TOneQueue<List<ZoneRateCustomer>> OutputQueue { get; set; }
        }
      
        protected override void DoWork(GetZoneRateSupplier inputArgument, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override GetZoneRateSupplier GetInputArgument(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
