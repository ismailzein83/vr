using System;
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
    public partial class GenerateDailyStatisticsProcess : Activity, IBPWorkflow
    {
        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            GenerateDailyStatisticsProcessInput inputArg = createProcessInput.InputArguments as GenerateDailyStatisticsProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("GenerateDailyStatisticsProcessInput");
            return String.Format("Generate Daily Statistics Process for Switch {0}", inputArg.SwitchID);
        }

        #endregion
    }
}
