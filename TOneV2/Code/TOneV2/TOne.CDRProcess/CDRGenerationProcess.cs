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
    public partial class CDRGenerationProcess : Activity, IBPWorkflow
    {
        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            CDRGenerationProcessInput inputArg = createProcessInput.InputArguments as CDRGenerationProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("CDRGenerationProcessInput");
            return String.Format("CDR Generation Process for Switch {0}", inputArg.SwitchID);
        }

        #endregion
    }
}
