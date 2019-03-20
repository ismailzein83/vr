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
            context.ThrowIfNull("context");

            context.BPTaskType.ThrowIfNull("context.BPTaskType");

            context.BPTaskType.Settings.ThrowIfNull("context.BPTaskType.Settings", context.BPTaskType.BPTaskTypeId);

            if (context.BPTaskType.Settings is BPGenericTaskTypeSettings)
                return true;

            else return false;
        }
    }
}
