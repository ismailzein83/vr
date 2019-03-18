using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.BPTaskTypes
{
    public class BPGenericTaskTypeSettingsFilter : IBPTaskTypeSettingsFilter
    {
        public bool IsMatch(BPTaskType bPTaskType)
        {
            if (bPTaskType == null)
                throw new NullReferenceException("bPTaskType");

            if (bPTaskType.Settings == null)
                throw new NullReferenceException("bPTaskType.Settings");

            if (bPTaskType.Settings is BPGenericTaskTypeSettings)
                return true;

            else return false;
        }
    }
}
