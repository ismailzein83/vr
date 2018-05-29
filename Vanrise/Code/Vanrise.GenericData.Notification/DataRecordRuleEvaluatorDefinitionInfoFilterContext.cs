using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordRuleEvaluatorDefinitionInfoFilterContext : IDataRecordRuleEvaluatorDefinitionInfoFilterContext
    {
        public Guid DataRecordRuleEvaluatorDefinitionId { get; set; }
    }
}
