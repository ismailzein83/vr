using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BaseProcessInputArgument
    {
        public virtual string ProcessName { get { return this.GetType().FullName; } }

        public virtual string EntityId { get; set; }

        public int UserId { get; set; }

        public abstract string GetTitle();

        public virtual string GetDefinitionTitle()
        {
            return BusinessManagerFactory.GetManager<IBPDefinitionManager>().GetDefinitionTitle(this.ProcessName);
        }

        public virtual void OnAfterSaveAction(IProcessInputArgumentOnAfterSaveActionContext context)
        {
        }

        public virtual void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {
        }

        public virtual void PrepareArgumentForExecutionFromTask(IProcessInputArgumentPrepareArgumentForExecutionFromTaskContext context)
        {
        }
    }

    public interface IProcessInputArgumentOnAfterSaveActionContext
    {
        Guid TaskId { get; }
    }

    public interface IProcessInputArgumentPrepareArgumentForExecutionFromTaskContext
    {
    }

    public class ProcessInputArgumentOnAfterSaveActionContext : IProcessInputArgumentOnAfterSaveActionContext
    {
        public Guid TaskId { get; set; }
    }
}