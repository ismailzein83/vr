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
    public partial class SaveDailyStatisticsToDBProcess : Activity, IBPWorkflow
    {
        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            SaveDailyStatisticsToDBProcessInput inputArg = createProcessInput.InputArguments as SaveDailyStatisticsToDBProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("SaveDailyStatisticsToDBProcessInput");
            return String.Format("Save Daily Statistics To DB Process for Switch {0}", inputArg.SwitchID);
        }

        #endregion
    }
}
