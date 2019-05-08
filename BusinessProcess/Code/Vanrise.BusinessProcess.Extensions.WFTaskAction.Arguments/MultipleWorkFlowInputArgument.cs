using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
namespace Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments
{
    public enum MultipleWorkFlowExecutionType { Parrallel = 1,Sequential = 2}
    public class MultipleWorkFlowInputArgument : BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return string.Format("Multiple WorkFlows");
        }
        public Dictionary<string, object> EvaluatedExpressions { get; set; }
        public MultipleWorkFlowExecutionType ExecutionType { get; set; }
        public List<MultipleWorkFlowInput> InputArguments { get; set; }
    }
    public class MultipleWorkFlowInput
    {
        public Guid MultipleWorkFlowInputId { get; set; }
        public string Name { get; set; }
        public BaseProcessInputArgument InputArgument { get; set; }
    }
}
