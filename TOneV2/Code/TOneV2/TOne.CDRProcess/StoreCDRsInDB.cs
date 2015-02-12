﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDRProcess.Arguments;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;

namespace TOne.CDRProcess
{
    public partial class StoreCDRsInDB : Activity, IBPWorkflow
    {
        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            StoreCDRsInDBInput inputArg = createProcessInput.InputArguments as StoreCDRsInDBInput;
            if (inputArg == null)
                throw new ArgumentNullException("StoreCDRsInDBInput");
            return String.Format("Store CDRs In DB for Switch {0}", inputArg.SwitchID);
        }

        #endregion

    }
}
