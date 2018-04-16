using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.UCT;

namespace Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments
{
    public class CustomCodeBPArgument : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return string.Format("Custom Code: {0}", this.Name);
        }
        public string Name { get; set; }

        public string ClassDefinitions { get; set; }

        public string TaskCode { get; set; }

        public override void PrepareArgumentForExecutionFromTask(Entities.IProcessInputArgumentPrepareArgumentForExecutionFromTaskContext context)
        {
            this.ClassDefinitions = null;
            this.TaskCode = null;
        }
    }

    public interface ICustomCodeHandler
    {
        void Execute(ICustomCodeExecutionContext context);
    }

    public interface ICustomCodeExecutionContext
    {
        void LogException(Exception ex);

        void LogError(string messageFormat, params object[] args);

        void LogWarning(string messageFormat, params object[] args);

        void LogInfo(string messageFormat, params object[] args);

        void SendMail(UctMailMessage mailMessage);
    }
}
