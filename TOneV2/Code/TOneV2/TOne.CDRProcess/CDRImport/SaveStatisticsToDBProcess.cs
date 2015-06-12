using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDRProcess.Arguments;
using Vanrise.BusinessProcess;

namespace TOne.CDRProcess
{
    public partial class SaveStatisticsToDBProcess : Activity, IBPWorkflow
    {
        public string GetTitle(Vanrise.BusinessProcess.Entities.CreateProcessInput createProcessInput)
        {
            SaveStatisticsToDBProcessInput inputArg = createProcessInput.InputArguments as SaveStatisticsToDBProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("SaveStatisticsToDBProcessInput");
            return String.Format("Save Statistics To DB Process for Switch {0}", inputArg.SwitchID);
        }
    }
}
