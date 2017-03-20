using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Runtime.Entities
{
    public class SchedulerTaskActionType
    {
        public Guid ActionTypeId { get; set; }

        public string Name { get; set; }

        public ActionTypeInfo Info { get; set; }
    }

    public class ActionTypeInfo
    {
        public string URL { get; set; }

        public bool SystemType { get; set; }

        public string FQTN { get; set; }

        public string Editor { get; set; }
        
        public ActionTypeInfoSecurity Security { get; set; }

        public ActionTypeExtendedSettings ExtendedSettings { get; set; }

        public bool IsUserTask { get; set; }

    }

    public abstract class ActionTypeExtendedSettings
    {
        public abstract bool DoesUserHaveViewAccess(IActionTypeDoesUserHaveViewAccessContext context);

        public virtual bool DoesUserHaveViewSpecificTaskAccess(IActionTypeDoesUserHaveViewSpecificInstanceAccessContext context)
        {
            return DoesUserHaveViewAccess(context.DefinitionContext);
        }

        public abstract bool DoesUserHaveConfigureTaskAccess(IActionTypeDoesUserHaveConfigureInstanceAccessContext context);

        public virtual bool DoesUserHaveConfigureSpecificTaskAccess(IActionTypeDoesUserHaveConfigureSpecificInstanceAccessContext context)
        {
            return DoesUserHaveConfigureTaskAccess(context.DefinitionContext);
        }

        public abstract bool DoesUserHaveRunAccess(IActionTypeDoesUserHaveRunAccessContext context);

        public virtual bool DoesUserHaveRunSpecificTaskAccess(IActionTypeDoesUserHaveRunSpecificInstanceAccessContext context)
        {
            return DoesUserHaveRunAccess(context.DefinitionContext);
        }
    }

    public interface IActionTypeDoesUserHaveViewAccessContext
    {
        int UserId { get; }

        ActionTypeInfo ActionTypeInfo { get; }
    }

    public class ActionTypeDoesUserHaveViewAccessContext : IActionTypeDoesUserHaveViewAccessContext
    {
        public int UserId { get; set; }

        public ActionTypeInfo ActionTypeInfo { get; set; }
    }

    public interface IActionTypeDoesUserHaveViewSpecificInstanceAccessContext
    {
        IActionTypeDoesUserHaveViewAccessContext DefinitionContext { get; }

        BaseTaskActionArgument TaskActionArgument { get; }
    }

    public class ActionTypeDoesUserHaveViewSpecificInstanceAccessContext : IActionTypeDoesUserHaveViewSpecificInstanceAccessContext
    {
        public IActionTypeDoesUserHaveViewAccessContext DefinitionContext { get; set; }

        public BaseTaskActionArgument TaskActionArgument { get; set; }
    }
 
    public interface IActionTypeDoesUserHaveConfigureInstanceAccessContext
    {
        int UserId { get; }

        ActionTypeInfo ActionTypeInfo { get; }
    }

    public class ActionTypeDoesUserHaveConfigureAccessContext : IActionTypeDoesUserHaveConfigureInstanceAccessContext
    {
        public int UserId { get; set; }

        public ActionTypeInfo ActionTypeInfo { get; set; }
    }

    public interface IActionTypeDoesUserHaveConfigureSpecificInstanceAccessContext
    {
        IActionTypeDoesUserHaveConfigureInstanceAccessContext DefinitionContext { get; }

        BaseTaskActionArgument TaskActionArgument { get; }
    }

    public class ActionTypeDoesUserHaveConfigureSpecificInstanceAccessContext : IActionTypeDoesUserHaveConfigureSpecificInstanceAccessContext
    {
        public IActionTypeDoesUserHaveConfigureInstanceAccessContext DefinitionContext { get; set; }

        public BaseTaskActionArgument TaskActionArgument { get; set; }
    }

    public interface IActionTypeDoesUserHaveRunAccessContext
    {
        int UserId { get; }

        ActionTypeInfo ActionTypeInfo { get; }
    }

    public class ActionTypeDoesUserHaveRunAccessContext : IActionTypeDoesUserHaveRunAccessContext
    {
        public int UserId { get; set; }

        public ActionTypeInfo ActionTypeInfo { get; set; }
    }

    public interface IActionTypeDoesUserHaveRunSpecificInstanceAccessContext
    {
        IActionTypeDoesUserHaveRunAccessContext DefinitionContext { get; }

        BaseTaskActionArgument TaskActionArgument { get; }
    }

    public class ActionTypeDoesUserHaveRunSpecificInstanceAccessContext : IActionTypeDoesUserHaveRunSpecificInstanceAccessContext
    {
        public IActionTypeDoesUserHaveRunAccessContext DefinitionContext { get; set; }

        public BaseTaskActionArgument TaskActionArgument { get; set; }
    }
 
    public class ActionTypeInfoSecurity
    {
        public RequiredPermissionSettings ViewPermission { get; set; }
        public RequiredPermissionSettings ConfigurePermission { get; set; }
        public RequiredPermissionSettings RunPermission { get; set; }

    }

}
