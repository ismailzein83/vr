using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordRuleEvaluatorDefinitionInfo
    {
        public Guid DataRecordRuleEvaluatorDefinitionId { get; set; }
        public string Name { get; set; }
    }
    public class DataRecordRuleEvaluatorDefinitionInfoFilter
    {
        public List<IDataRecordRuleEvaluatorDefinitionInfoFilter> Filters { get; set; }
    }
    public interface IDataRecordRuleEvaluatorDefinitionInfoFilter
    {
        bool IsMatched(IDataRecordRuleEvaluatorDefinitionInfoFilterContext context);
    }
    public interface IDataRecordRuleEvaluatorDefinitionInfoFilterContext
    {
        Guid DataRecordRuleEvaluatorDefinitionId { get; }
    }
}
