using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class RecordFilterManager
    {
        public bool IsFilterGroupMatch(RecordFilterGroup filterGroup, IRecordFilterGenericFieldMatchContext context)
        {
            if(filterGroup == null)
                return true;
            if (filterGroup.Filters == null)
                throw new NullReferenceException("filterGroup.Filters");
            foreach(var filter in filterGroup.Filters)
            {
                bool isMatch = IsFilterMatch(filter, context);
                if (isMatch && filterGroup.LogicalOperator == RecordQueryLogicalOperator.Or)
                    return true;
                if (!isMatch && filterGroup.LogicalOperator == RecordQueryLogicalOperator.And)
                    return false;
            }
            return true;
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
