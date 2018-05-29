using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordRuleEvaluatorDefinitionStartInstanceFilter : IDataRecordRuleEvaluatorDefinitionInfoFilter
    {
        public bool IsMatched(IDataRecordRuleEvaluatorDefinitionInfoFilterContext context)
        {
            return new DataRecordRuleEvaluatorDefinitionManager().DoesUserHaveStartInstanceAccess(context.DataRecordRuleEvaluatorDefinitionId);
        }
    }
}
