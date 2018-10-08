using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments
{
    public class DatabaseJobProcessInput : BaseProcessInputArgument
    {
        public string ConnectionString { get; set; }
        public string ConnectionStringName { get; set; }
        public string Query { get; set; }

        public override string GetTitle()
        {
            throw new NotImplementedException();
        }

        public override void PrepareArgumentForExecutionFromTask(Entities.IProcessInputArgumentPrepareArgumentForExecutionFromTaskContext context)
        {
            this.Query = null;
        }
    }
}