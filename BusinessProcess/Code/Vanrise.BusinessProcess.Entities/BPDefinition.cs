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
        public string CompletionViewURL { get; set; }
        public string CompletionViewLinkText { get; set; }
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

        public virtual void OnBPExecutionCompleted(IBPDefinitionBPExecutionCompletedContext context)
        {

        }

        //public virtual bool ShouldRestrictBPInstanceCreation(IBPDefinitionShouldRestrictBPInstanceCreationContext context)
        //{
        //    return false;
        //}

        //public virtual bool CanCreateBPInstance(IBPDefinitionCanCreateBPInstanceContext context)
        //{
        //    return true;
        //}

        public virtual bool CanRunBPInstance(IBPDefinitionCanRunBPInstanceContext context)
        {
            return true;
        }

        public virtual bool CanCancelBPInstance(IBPDefinitionCanCancelBPInstanceContext context)
        {
            return false;
        }

        public virtual List<BPInstanceProgressView> GetBPInstanceProgressViews(IGetBPInstanceProgressViewsContext context)
        {
            return null;
        }

        public virtual bool ShouldCreateScheduledInstance(IBPDefinitionShouldCreateScheduledInstanceContext context) 
        {
            return true;
        }

        public virtual bool ShouldPersist(IBPDefinitionShouldPersistContext context)
        {
            return false;
        }
    }

    //public interface IBPDefinitionShouldRestrictBPInstanceCreationContext
    //{
    //}

    //public interface IBPDefinitionCanCreateBPInstanceContext
    //{
    //    BaseProcessInputArgument InputArgument { get; }

    //    List<BPInstance> GetPendingBPInstances(List<Guid> bpDefinitionIds);
    //}

    public class BPDefinitionSecurity
    {
        public RequiredPermissionSettings View { get; set; }
        public RequiredPermissionSettings StartNewInstance { get; set; }
        public RequiredPermissionSettings ScheduleTask { get; set; }

    }
}