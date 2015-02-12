using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;

namespace TOne.LCRProcess.Activities
{
    #region Arguments Classes
    public class GetZoneRatesInput
    {
        public bool IsFuture { get; set; }
    }

    #endregion
    public class GetZoneRates : BaseAsyncActivity<GetZoneRatesInput>
    {
        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }
        protected override void DoWork(GetZoneRatesInput inputArgument, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override GetZoneRatesInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new GetZoneRatesInput
            {
                IsFuture = this.IsFuture.Get(context)
            };
        }
    }
}
