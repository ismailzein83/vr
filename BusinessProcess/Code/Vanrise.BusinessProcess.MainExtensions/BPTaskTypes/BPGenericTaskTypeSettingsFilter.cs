using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.BPTaskTypes
{
    public class BPGenericTaskTypeSettingsFilter : IBPTaskTypeSettingsFilter
    {
        public bool IsMatch(IBPTaskTypeSettingsFilterContext context)
        {
            if (context == null)
                context.ThrowIfNull("context");

            if (context.BPTaskType == null)
                context.BPTaskType.ThrowIfNull("context.BPTaskType");

            if (context.BPTaskType.Settings == null)
                context.BPTaskType.Settings.ThrowIfNull("context.BPTaskType.Settings", context.BPTaskType.BPTaskTypeId);

            if (context.BPTaskType.Settings is BPGenericTaskTypeSettings)
                return true;

            else return false;
        }
    }
}
