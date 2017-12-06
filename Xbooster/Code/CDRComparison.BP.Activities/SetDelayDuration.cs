using CDRComparison.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace CDRComparison.BP.Activities
{
    public class SetDelayDuration : CodeActivity
    {
        #region Output Arguments
        [RequiredArgument]
        public OutArgument<TimeSpan> DelayDuration { get; set; }
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            CDRComparisonSettingData cdrComparisonSettings = new Vanrise.Common.Business.SettingManager().GetSetting<CDRComparisonSettingData>("CDRComparison_CDRComparisonSettings");
            cdrComparisonSettings.ThrowIfNull("cdrComparisonSettings");
            var delayDuration = new TimeSpan(0, 0, cdrComparisonSettings.TaskTimeoutInSeconds);
            DelayDuration.Set(context, delayDuration);
        }
    }
}
