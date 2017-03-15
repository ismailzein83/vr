using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class DefaultBPDefinitionExtendedSettings : BPDefinitionExtendedSettings
    {
        public override bool DoesUserHaveViewAccess(IBPDefinitionDoesUserHaveViewAccessContext context)
        {
            var bpDefinition = context.BPDefinition;
            if (bpDefinition.Configuration.Security != null && bpDefinition.Configuration.Security.View != null && !DoesUserHaveBPPermission(context.UserId, bpDefinition.Configuration.Security.View))
                return false;
            return true;
        }

        public override bool DoesUserHaveStartAccess(IBPDefinitionDoesUserHaveStartAccessContext context)
        {
            var bpDefinition = context.BPDefinition;
            if (bpDefinition.Configuration.Security != null && bpDefinition.Configuration.Security.StartNewInstance != null && !DoesUserHaveBPPermission(context.UserId, bpDefinition.Configuration.Security.StartNewInstance))
                return false;
            return true;
        }
        public override bool DoesUserHaveScheduleTaskAccess(IBPDefinitionDoesUserHaveScheduleTaskContext context)
        {
            var bpDefinition = context.BPDefinition;
            if (bpDefinition.Configuration.Security != null && bpDefinition.Configuration.Security.ScheduleTask != null && !DoesUserHaveBPPermission(context.UserId, bpDefinition.Configuration.Security.ScheduleTask))
                return false;
            return true;
        }

        private bool DoesUserHaveBPPermission(int userId, RequiredPermissionSettings permission)
        {
            SecurityManager secManager = new SecurityManager();
            return secManager.IsAllowed(permission, userId);
        }
       
    }
}
