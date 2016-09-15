using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class RecordFilterManager
    {
        public string BuildRecordFilterGroupExpression(RecordFilterGroup filterGroup, List<RecordFilterFieldInfo> recordFields)
        {
            StringBuilder expression = new StringBuilder();
            bool firstFilter = true;
            foreach (var filter in filterGroup.Filters)
            {
                BuildChildRecordFilterGroupExpression(filter, recordFields, expression, filterGroup.LogicalOperator, firstFilter);
                firstFilter = false;
            }
            return expression.ToString();
        }

        public void BuildChildRecordFilterGroupExpression(RecordFilter recordFilter, List<RecordFilterFieldInfo> recordFields, StringBuilder expression, RecordQueryLogicalOperator logicalOperator, bool isFirstFilter)
        {
            
            RecordFilterGroup childFilterGroup = recordFilter as RecordFilterGroup;
            if (expression == null)
                expression = new StringBuilder();
            if (childFilterGroup != null)
            {
                if (expression.Length != 0)
                  expression.Append(string.Format(" {0} ", Utilities.GetEnumDescription(logicalOperator)));
                bool firstFilter = true;
                expression.Append(" ( ");
                foreach (var filter in childFilterGroup.Filters)
                {
                    BuildChildRecordFilterGroupExpression(filter, recordFields, expression, childFilterGroup.LogicalOperator, firstFilter);
                    firstFilter = false;
                }
                expression.Append(" ) ");
            }

            if (expression.Length != 0 && !isFirstFilter && childFilterGroup == null)
                expression.Append(string.Format(" {0} ", Utilities.GetEnumDescription(logicalOperator)));

            if (recordFilter is EmptyRecordFilter)
            {
                expression.Append(string.Format(" {0} Is Empty ", recordFilter.FieldName));
            }
            else if (recordFilter is NonEmptyRecordFilter)
            {
                expression.Append(string.Format(" {0} Is Non Empty ", recordFilter.FieldName));
            }
            else if (recordFilter.FieldName != null)
            {
                var record = recordFields.FindRecord(x => x.Name == recordFilter.FieldName);
                if (record != null)
                {
                    expression.Append(record.Type.GetFilterDescription(recordFilter));
                }
            }
        }

        public bool IsFilterGroupMatch(RecordFilterGroup filterGroup, IRecordFilterGenericFieldMatchContext context)
        {
            if(filterGroup == null)
                return true;
            if (filterGroup.Filters == null)
                throw new NullReferenceException("filterGroup.Filters");
            if (filterGroup.Filters.Count == 0)
                return true;
            if(filterGroup.LogicalOperator == RecordQueryLogicalOperator.And)
            {
                foreach (var filter in filterGroup.Filters)
                {
                    bool isMatch = IsFilterMatch(filter, context);
                    if (!isMatch)
                        return false;
                }
                return true;
            }
            else//OR
            {
                foreach (var filter in filterGroup.Filters)
                {
                    bool isMatch = IsFilterMatch(filter, context);
                    if (isMatch)
                        return true;
                }
                return false;
            }
        }

        private bool IsFilterMatch(RecordFilter filter, IRecordFilterGenericFieldMatchContext context)
        {
            RecordFilterGroup filterGroup = filter as RecordFilterGroup;
            if (filterGroup != null)
                return IsFilterGroupMatch(filterGroup, context);

            DataRecordFieldType fieldType;
            var fieldValue = context.GetFieldValue(filter.FieldName, out fieldType);
            
            EmptyRecordFilter emptyFilter = filter as EmptyRecordFilter;
            if (emptyFilter != null)
                return fieldValue == null;

            NonEmptyRecordFilter nonEmptyFilter = filter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
                return fieldValue != null;

            return fieldType.IsMatched(fieldValue, filter);
        }

        public bool IsSingleFieldFilterMatch(RecordFilter filter, Object fieldValue, DataRecordFieldType fieldType)
        {
            return IsFilterMatch(filter, new SingleFieldRecordFilterGenericFieldMatchContext(fieldValue, fieldType));
        }

        #region Private Classes

        private class SingleFieldRecordFilterGenericFieldMatchContext : IRecordFilterGenericFieldMatchContext
        {
            Object _fieldValue;
            DataRecordFieldType _fieldType;

            public SingleFieldRecordFilterGenericFieldMatchContext(Object fieldValue, DataRecordFieldType fieldType)
            {
                _fieldValue = fieldValue;
                _fieldType = fieldType;
            }

            public object GetFieldValue(string fieldName, out DataRecordFieldType fieldType)
            {
                fieldType = _fieldType;
                return _fieldValue;
            }
        }

        #endregion
    }

    public interface IRecordFilterGenericFieldMatchContext
    {
        Object GetFieldValue(string fieldName, out DataRecordFieldType fieldType);
    }

    public class DataRecordFilterGenericFieldMatchContext : IRecordFilterGenericFieldMatchContext
    {
        dynamic _dataRecord;
        Dictionary<string, DataRecordField> _recordTypeFieldsByName;
        public DataRecordFilterGenericFieldMatchContext(dynamic dataRecord, int recordTypeId)
        {
            if (dataRecord == null)
                throw new ArgumentNullException("dataRecord");
            _dataRecord = dataRecord;
            _recordTypeFieldsByName = (new DataRecordTypeManager()).GetDataRecordTypeFields(recordTypeId);
            if (_recordTypeFieldsByName == null)
                throw new NullReferenceException("_recordTypeFieldsByName");
        }

        public object GetFieldValue(string fieldName, out DataRecordFieldType fieldType)
        {
            DataRecordField field;
            if (!_recordTypeFieldsByName.TryGetValue(fieldName, out field))
                throw new NullReferenceException(String.Format("field. fieldName '{0}'", fieldName));
            fieldType = field.Type;
            var propReader = Common.Utilities.GetPropValueReader(fieldName);
            return propReader.GetPropertyValue(_dataRecord);
        }
    }

}
