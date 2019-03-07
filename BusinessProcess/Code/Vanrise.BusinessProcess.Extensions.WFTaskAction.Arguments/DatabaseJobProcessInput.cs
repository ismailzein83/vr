using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments
{
    public class DatabaseJobProcessInput : BaseProcessInputArgument
    {
        public string ConnectionString { get; set; }
        public string ConnectionStringName { get; set; }
        public string ConnectionStringAppSettingName { get; set; }
        public string Query { get; set; }
        private string Title { get; set; }

        public override string GetTitle()
        {
            return Title;
        }

        public override void PrepareArgumentForExecutionFromTask(Entities.IProcessInputArgumentPrepareArgumentForExecutionFromTaskContext context)
        {
            this.Query = null;
        }

        public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {
            this.Title = evaluatedExpressions.GetRecord("TaskName").ToString();
        }
    }
}