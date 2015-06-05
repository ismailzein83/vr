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
    public partial class GenerateStatisticsProcess : Activity, IBPWorkflow
    {
        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            GenerateStatisticsProcessInput inputArg = createProcessInput.InputArguments as GenerateStatisticsProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("GenerateStatisticsProcessInput");
            return String.Format("Generate Statistics Process for Switch {0}", inputArg.SwitchID);
        }

        #endregion
    }
}
