﻿using System;
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
    }

    public class GetCDRsOutput
    {
        public CDRBatch CDRs { get; set; }
    }


    #endregion

    public sealed class GetCDRs : BaseAsyncActivity<GetCDRsInput, GetCDRsOutput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<int> SwitchID { get; set; }

        [RequiredArgument]
        public OutArgument<CDRBatch> CDRs { get; set; }

        #endregion

        #region Private Methods

        private CDRBatch GetCDRsBySwitchId(int SwitchID)
        {
            List<TABS.CDR> ToneCdrs = new List<TABS.CDR>();
            CDRBatch BatchCdrs = new CDRBatch();

            TABS.Switch CurrentSwitch = null;
            if (TABS.Switch.All.ContainsKey(SwitchID))
                CurrentSwitch = TABS.Switch.All[SwitchID];

            if (CurrentSwitch != null)
            {
                var rawCDRs = CurrentSwitch.SwitchManager.GetCDR(CurrentSwitch);

                // create CDRs from Standard CDRs
                foreach (TABS.Addons.Utilities.Extensibility.CDR rawCDR in rawCDRs)
                    ToneCdrs.Add(new TABS.CDR(CurrentSwitch, rawCDR));
            }
            BatchCdrs.CDRs = ToneCdrs;
            return BatchCdrs;
        }

        #endregion

        protected override GetCDRsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetCDRsInput
            {
                SwitchID = this.SwitchID.Get(context)
            };
        }

        protected override GetCDRsOutput DoWorkWithResult(GetCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            GetCDRsOutput output = new GetCDRsOutput();
            output.CDRs = GetCDRsBySwitchId(inputArgument.SwitchID);
            return output;
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetCDRsOutput result)
        {
            this.CDRs.Set(context, result.CDRs);
        }
    }


}
