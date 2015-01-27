using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.Entities;
using System.Collections.Concurrent;
using TABS;
using TOne.CDRProcess.Arguments;
using TOne.Caching;
using Vanrise.Caching;
namespace TOne.CDRProcess.Activities
{

    #region Arguments Classes

    public class GetCDRsInput
    {
        public int SwitchID { get; set; }

        public CDRBatch CDRs { get; set; }
    }


    #endregion

    public sealed class GetCDRs : BaseAsyncActivity<GetCDRsInput>
    {
        [RequiredArgument]
        public InArgument<int> SwitchID { get; set; }

        [RequiredArgument]
        public InOutArgument<CDRBatch> CDRs { get; set; }


        protected override GetCDRsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetCDRsInput
            {
                SwitchID = this.SwitchID.Get(context),
                CDRs = this.CDRs.Get(context)
            };
        }

        protected override void DoWork(GetCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            CDRManager manager = new CDRManager();
            inputArgument.CDRs = manager.GetCDRs(inputArgument.SwitchID);
        }
    }


}
