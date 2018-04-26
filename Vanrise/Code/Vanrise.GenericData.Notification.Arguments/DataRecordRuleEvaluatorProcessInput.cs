using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Notification.Arguments
{
    public class DataRecordRuleEvaluatorProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid DataRecordRuleEvaluatorDefinitionId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public override string GetTitle()
        {
            return "Data Record Rule Evaluator";
        }
    }
}
