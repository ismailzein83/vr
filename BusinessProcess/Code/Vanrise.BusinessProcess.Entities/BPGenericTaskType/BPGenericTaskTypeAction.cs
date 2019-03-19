using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPGenericTaskTypeAction
    {
        public Guid TaskTypeActionId { get; set; }
        public string Name { get; set; }
        public VRButtonType ButtonType { get; set; }
        public BPGenericTaskTypeActionSettings Settings { get; set; }
        public BPGenericTaskTypeActionFilterCondition FilterCondition { get; set; }
        public RequiredPermissionSettings RequiredPermission { get; set; }
    }
    public abstract class BPGenericTaskTypeActionFilterCondition
    {
        public abstract Guid ConfigId { get; }
        public abstract bool IsFilterMatch(IBPGenericTaskTypeActionFilterConditionContext context);
    }
    public interface IBPGenericTaskTypeActionFilterConditionContext
    {
    }
    public class BPGenericTaskTypeActionFilterConditionContext : IBPGenericTaskTypeActionFilterConditionContext
    {
    }
    public abstract class BPGenericTaskTypeActionSettings
    {
        public virtual string ActionTypeName { get; set; }
        public abstract Guid ConfigId { get; }
        public virtual bool DoesUserHaveAccess(IBPGenericTaskTypeActionSettingsCheckAccessContext context)
        {
            if (context.TaskTypeAction.RequiredPermission == null)
                return true;
            return ContextFactory.GetContext().IsAllowed(context.TaskTypeAction.RequiredPermission, context.UserId);
        }
    }
    public interface IBPGenericTaskTypeActionSettingsCheckAccessContext
    {
        int UserId { get; }
        BPGenericTaskTypeAction TaskTypeAction { get; }
    }
    public class BPGenericTaskTypeActionSettingsCheckAccessContext : IBPGenericTaskTypeActionSettingsCheckAccessContext
    {
        public int UserId { get; set; }
        public BPGenericTaskTypeAction TaskTypeAction { get; set; }

    }
}
