using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Runtime.Business
{
    public class DefaultTaskActionExtentedSettings : ActionTypeExtendedSettings
    {
        public override bool DoesUserHaveViewAccess(IActionTypeDoesUserHaveViewAccessContext context)
        {
            var action = context.ActionTypeInfo;
            if (action!=null && action.Security != null && action.Security.ViewPermission != null && !DoesUserHaveSchedulePermission(context.UserId, action.Security.ViewPermission))
                return false;
            return true;
        }

        public override bool DoesUserHaveConfigureTaskAccess(IActionTypeDoesUserHaveConfigureInstanceAccessContext context)
        {
            var action = context.ActionTypeInfo;
            if (action != null &&  action.Security != null && action.Security.ConfigurePermission != null && !DoesUserHaveSchedulePermission(context.UserId, action.Security.ConfigurePermission))
                return false;
            return true;
        }

        public override bool DoesUserHaveRunAccess(IActionTypeDoesUserHaveRunAccessContext context)
        {
            var action = context.ActionTypeInfo;
            if (action != null && action.Security != null && action.Security.RunPermission != null && !DoesUserHaveSchedulePermission(context.UserId, action.Security.RunPermission))
                return false;
            return true;
        }
        
        private bool DoesUserHaveSchedulePermission(int userId, RequiredPermissionSettings permission)
        {
            return ContextFactory.GetContext().IsAllowed(permission, userId);
        }
       
    }
}
