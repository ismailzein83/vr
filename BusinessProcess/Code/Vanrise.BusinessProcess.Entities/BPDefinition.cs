using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Security.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPDefinition
    {
        public Guid BPDefinitionID { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public Type WorkflowType { get; set; } // Type should inherit System.Activities.Activity
        public BPConfiguration Configuration { get; set; }
    }

    public class BPConfiguration
    {
        public int? MaxConcurrentWorkflows { get; set; }
        public string Url { get; set; }
        public string ScheduleTemplateURL { get; set; }
        public string ManualExecEditor { get; set; }
        public string ScheduledExecEditor { get; set; }
        public bool IsPersistable { get; set; }
        public bool HasChildProcesses { get; set; }
        public bool HasBusinessRules { get; set; }
        public bool NotVisibleInManagementScreen { get; set; }

        public BPDefinitionExtendedSettings ExtendedSettings { get; set; }

        public BPDefinitionSecurity Security { get; set; }
    }

    public abstract class BPDefinitionExtendedSettings
    {
        public abstract bool DoesUserHaveViewAccess(IBPDefinitionDoesUserHaveViewAccessContext context);

        public abstract RequiredPermissionSettings GetViewInstanceRequiredPermissions(IBPDefinitionGetViewInstanceRequiredPermissionsContext context);

        public abstract bool DoesUserHaveStartAccess(IBPDefinitionDoesUserHaveStartAccessContext context);

        public abstract bool DoesUserHaveScheduleTaskAccess(IBPDefinitionDoesUserHaveScheduleTaskContext context);

        public virtual bool DoesUserHaveStartSpecificInstanceAccess(IBPDefinitionDoesUserHaveStartSpecificInstanceAccessContext context)
        {
            return DoesUserHaveStartAccess(context.DefinitionContext);
        }

        public virtual bool DoesUserHaveScheduleSpecificTaskAccess(IBPDefinitionDoesUserHaveScheduleSpecificTaskAccessContext context)
        {
            return DoesUserHaveScheduleTaskAccess(context.DefinitionContext);
        }
    }
    
    public interface IBPDefinitionGetViewInstanceRequiredPermissionsContext
    {
        BPDefinition BPDefinition { get; }

        BaseProcessInputArgument InputArg { get; }
    }

    public class BPDefinitionGetViewInstanceRequiredPermissionsContext : IBPDefinitionGetViewInstanceRequiredPermissionsContext
    {
        public BPDefinition BPDefinition { get; set; }

        public BaseProcessInputArgument InputArg { get; set; }
    }

    public interface IBPDefinitionDoesUserHaveViewAccessContext
    {
        int UserId { get; }

        BPDefinition BPDefinition { get; }
    }

    public class BPDefinitionDoesUserHaveViewAccessContext : IBPDefinitionDoesUserHaveViewAccessContext
    {
        public int UserId { get; set; }

        public BPDefinition BPDefinition { get; set; }
    }

    public interface IBPDefinitionDoesUserHaveStartAccessContext
    {
        int UserId { get; }

        BPDefinition BPDefinition { get; }
    }

    public class BPDefinitionDoesUserHaveStartAccessContext : IBPDefinitionDoesUserHaveStartAccessContext
    {
        public int UserId { get; set; }

        public BPDefinition BPDefinition { get; set; }
    }

    public interface IBPDefinitionDoesUserHaveScheduleTaskContext
    {
        int UserId { get; }

        BPDefinition BPDefinition { get; }
    }

    public class BPDefinitionDoesUserHaveScheduleTaskContext : IBPDefinitionDoesUserHaveScheduleTaskContext
    {
        public int UserId { get; set; }

        public BPDefinition BPDefinition { get; set; }
    }

    public interface IBPDefinitionDoesUserHaveStartSpecificInstanceAccessContext
    {
        IBPDefinitionDoesUserHaveStartAccessContext DefinitionContext { get; }

        BaseProcessInputArgument InputArg { get; }
    }

    public class BPDefinitionDoesUserHaveStartSpecificInstanceAccessContext : IBPDefinitionDoesUserHaveStartSpecificInstanceAccessContext
    {
        public IBPDefinitionDoesUserHaveStartAccessContext DefinitionContext { get; set; }

        public BaseProcessInputArgument InputArg { get; set; }
    }

    public interface IBPDefinitionDoesUserHaveScheduleSpecificTaskAccessContext
    {
        IBPDefinitionDoesUserHaveScheduleTaskContext DefinitionContext { get; }

        BaseProcessInputArgument InputArg { get; }
    }

    public class BPDefinitionDoesUserHaveScheduleSpecificTaskAccessContext : IBPDefinitionDoesUserHaveScheduleSpecificTaskAccessContext
    {
        public IBPDefinitionDoesUserHaveScheduleTaskContext DefinitionContext { get; set; }

        public BaseProcessInputArgument InputArg { get; set; }
    }


    public class BPDefinitionSecurity
    {
        public RequiredPermissionSettings View { get; set; }
        public RequiredPermissionSettings StartNewInstance { get; set; }
        public RequiredPermissionSettings ScheduleTask { get; set; }

    }
}