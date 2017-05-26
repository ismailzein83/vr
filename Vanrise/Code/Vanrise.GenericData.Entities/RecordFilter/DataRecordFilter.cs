using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.GenericData.Entities
{
    public enum RecordQueryLogicalOperator
    {
        [Description("AND")]
        And = 0,
        [Description("OR")]
        Or = 1
    }
    public abstract class RecordFilter
    {
        public string FieldName { get; set; }

        public abstract string GetDescription(IRecordFilterGetDescriptionContext context);

        public virtual void SetValueFromParameters(IRecordFilterSetValueFromParametersContext context)
        {

        }
    }

    public class RecordFilterGroup : RecordFilter
    {
        public RecordQueryLogicalOperator LogicalOperator { get; set; }

        public List<RecordFilter> Filters { get; set; }

        public override string GetDescription(IRecordFilterGetDescriptionContext context)
        {
            List<string> filterExpressions = new List<string>();

            foreach (var filter in Filters)
            {
                filterExpressions.Add(filter.GetDescription(context));
            }

            string logicalOperatorSymbol = string.Format(" {0} ", Utilities.GetEnumDescription(LogicalOperator));
            string result = string.Format(" ( {0} ) ", string.Join(logicalOperatorSymbol, filterExpressions));

            return result;
        }

        public override void SetValueFromParameters(IRecordFilterSetValueFromParametersContext context)
        {
            if (this.Filters != null)
            {
                foreach (var filter in this.Filters)
                {
                    filter.SetValueFromParameters(context);
                }
            }
        }
    }

    public interface IRecordFilterSetValueFromParametersContext
    {
        bool TryGetParameterValue(Guid parameterId, out dynamic value);
    }

    public interface IRecordFilterGetDescriptionContext
    {
        string GetFieldTitle(string fieldName);

        string GetFieldValueDescription(string fieldName, object fieldValue);
    }

    public class RecordFilterGetDescriptionContext : IRecordFilterGetDescriptionContext
    {
        Dictionary<string, RecordFilterFieldInfo> _recordFieldsByFieldName;

        public RecordFilterGetDescriptionContext(Dictionary<string, RecordFilterFieldInfo> recordFieldsByFieldName)
        {
            _recordFieldsByFieldName = recordFieldsByFieldName;
        }

        public string GetFieldTitle(string fieldName)
        {
            RecordFilterFieldInfo recordFilterFieldInfo = _recordFieldsByFieldName.GetRecord(fieldName);
            if (recordFilterFieldInfo == null)
                return null;

            return recordFilterFieldInfo.Title;
        }

        public string GetFieldValueDescription(string fieldName, object fieldValue)
        {
            RecordFilterFieldInfo recordFilterFieldInfo = _recordFieldsByFieldName.GetRecord(fieldName);
            if (recordFilterFieldInfo == null)
                return null;

            return recordFilterFieldInfo.Type.GetDescription(fieldValue);
        }
    }
}
