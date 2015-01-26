﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDRProcess.Activities
{
    #region Arguments Classes

    public class SaveCDRsToDBInput
    {
        public int SwitchID { get; set; }

        public CDRBatch CDRs { get; set; }

    }

    #endregion
    public sealed class SaveCDRToDB : Vanrise.BusinessProcess.BaseAsyncActivity<SaveCDRsToDBInput>
    {

        [RequiredArgument]
        public InArgument<CDRBatch> CDRs { get; set; }

        [RequiredArgument]
        public InArgument<int> SwitchID { get; set; }




        protected override void DoWork(SaveCDRsToDBInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            CDRManager manager = new CDRManager();
            manager.SaveCDRstoDB(inputArgument.CDRs);
        }

        protected override SaveCDRsToDBInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new SaveCDRsToDBInput
            {
                CDRs = this.CDRs.Get(context),
                SwitchID = this.SwitchID.Get(context),
            };
        }
    }
}
