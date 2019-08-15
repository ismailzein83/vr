﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class RecordFilterManager
    {
        public string BuildRecordFilterGroupExpression(RecordFilterGroup filterGroup, Dictionary<string, RecordFilterFieldInfo> recordFilterFieldInfosByFieldName)
        {
            StringBuilder expression = new StringBuilder();
            var context = new RecordFilterGetDescriptionContext(recordFilterFieldInfosByFieldName);
            return filterGroup.GetDescription(context);
        }

        public bool IsFilterGroupMatch(RecordFilterGroup filterGroup, Dictionary<Guid, dynamic> parameterValues, IRecordFilterGenericFieldMatchContext context)
        {
            if (parameterValues != null)
                ApplyParametersToFilterGroup(filterGroup, parameterValues);
            return IsFilterGroupMatch(filterGroup, context);
        }

        public bool IsFilterGroupMatch(RecordFilterGroup filterGroup, IRecordFilterGenericFieldMatchContext context)
        {
            if (filterGroup == null)
                return true;
            if (filterGroup.Filters == null)
                throw new NullReferenceException("filterGroup.Filters");
            if (filterGroup.Filters.Count == 0)
                return true;
            if (filterGroup.LogicalOperator == RecordQueryLogicalOperator.And)
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

            if (filter is AlwaysTrueRecordFilter)
                return true;

            if (filter is AlwaysFalseRecordFilter)
                return false;

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

        public void ApplyParametersToFilterGroup(RecordFilterGroup filterGroup, Dictionary<Guid, dynamic> parameterValues)
        {
            var context = new RecordFilterSetValueFromParametersContext(parameterValues);
            filterGroup.SetValueFromParameters(context);
        }

        public RecordFilterGroup BuildRecordFilterGroup(Guid dataRecordTypeId, List<DataRecordFilter> filters, RecordFilter filterGroup)
        {
            DataRecordTypeManager dataRecordTypeManager = new GenericData.Business.DataRecordTypeManager();
            var recordType = dataRecordTypeManager.GetDataRecordType(dataRecordTypeId);

            RecordFilterGroup recordFilterGroup = new RecordFilterGroup();
            recordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.And;
            recordFilterGroup.Filters = new List<RecordFilter>();

            if (filterGroup != null)
            {
                recordFilterGroup.Filters.Add(filterGroup);
            }
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    if (filter.FilterValues == null)
                    {
                        recordFilterGroup.Filters.Add(new EmptyRecordFilter() { FieldName = filter.FieldName });
                        continue;
                    }

                    var record = recordType.Fields.FindRecord(x => x.Name == filter.FieldName);
                    List<object> notNullFilterValues = filter.FilterValues.Where(itm => itm != null).ToList();
                    RecordFilter notNullValuesRecordFilter = notNullFilterValues.Count > 0 ? record.Type.ConvertToRecordFilter(new DataRecordFieldTypeConvertToRecordFilterContext { FieldName = record.Name, FilterValues = notNullFilterValues }) : null;
                    EmptyRecordFilter emptyRecordFilter = notNullFilterValues.Count != filter.FilterValues.Count ? new EmptyRecordFilter { FieldName = record.Name } : null;

                    if (notNullValuesRecordFilter != null && emptyRecordFilter != null)
                    {
                        RecordFilterGroup dimensionsRecordFilterGroup = new RecordFilterGroup();
                        dimensionsRecordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.Or;
                        dimensionsRecordFilterGroup.Filters = new List<RecordFilter>();
                        dimensionsRecordFilterGroup.Filters.Add(emptyRecordFilter);
                        dimensionsRecordFilterGroup.Filters.Add(notNullValuesRecordFilter);

                        recordFilterGroup.Filters.Add(dimensionsRecordFilterGroup);
                    }
                    else if (notNullValuesRecordFilter != null)
                    {
                        recordFilterGroup.Filters.Add(notNullValuesRecordFilter);
                    }
                    else if (emptyRecordFilter != null)
                    {
                        recordFilterGroup.Filters.Add(emptyRecordFilter);
                    }
                }
            }

            return recordFilterGroup;
        }

        public bool IsFieldValueMatched(string fieldValue, StringRecordFilter stringRecordFilter)
        {
            string filterValue = stringRecordFilter.Value;
            if (filterValue == null)
                throw new NullReferenceException("filterValue");

            switch (stringRecordFilter.CompareOperator)
            {
                case StringRecordFilterOperator.Equals: return String.Compare(fieldValue, filterValue, true) == 0;
                case StringRecordFilterOperator.NotEquals: return String.Compare(fieldValue, filterValue, true) != 0;
                case StringRecordFilterOperator.Contains: return fieldValue.IndexOf(filterValue, StringComparison.OrdinalIgnoreCase) >= 0;
                case StringRecordFilterOperator.NotContains: return fieldValue.IndexOf(filterValue, StringComparison.OrdinalIgnoreCase) < 0;
                case StringRecordFilterOperator.StartsWith: return fieldValue.StartsWith(filterValue, StringComparison.InvariantCultureIgnoreCase);
                case StringRecordFilterOperator.NotStartsWith: return !fieldValue.StartsWith(filterValue, StringComparison.InvariantCultureIgnoreCase);
                case StringRecordFilterOperator.EndsWith: return fieldValue.EndsWith(filterValue, StringComparison.InvariantCultureIgnoreCase);
                case StringRecordFilterOperator.NotEndsWith: return !fieldValue.EndsWith(filterValue, StringComparison.InvariantCultureIgnoreCase);
                case StringRecordFilterOperator.GreaterThanOrEqual: return String.Compare(fieldValue, filterValue, true) >= 0;
                case StringRecordFilterOperator.LessThanOrEqual: return String.Compare(fieldValue, filterValue, true) <= 0;
            }

            return false;
        }

        public void AddFieldNamesFromFilterGroup(RecordFilterGroup filterGroup, HashSet<string> fieldNames)
        {
            foreach (var filter in filterGroup.Filters)
            {
                RecordFilterGroup childFilterGroup = filter as RecordFilterGroup;
                if (childFilterGroup != null)
                    AddFieldNamesFromFilterGroup(childFilterGroup, fieldNames);
                else
                    fieldNames.Add(filter.FieldName);
            }
        }

        public RecordFilterGroup ReBuildRecordFilterGroupWithExcludedFields(RecordFilterGroup recordFilter, List<string> fieldsToExclude)
        {
            RecordFilterGroup newFilterGroup = new RecordFilterGroup
            {
                Filters = new List<RecordFilter>(),
                LogicalOperator = recordFilter.LogicalOperator
            };
            if (recordFilter.Filters != null)
            {
                foreach (var childFilter in recordFilter.Filters)
                {
                    RecordFilterGroup childFilterGroup = childFilter as RecordFilterGroup;
                    if (childFilterGroup != null)
                    {
                        newFilterGroup.Filters.Add(ReBuildRecordFilterGroupWithExcludedFields(childFilterGroup, fieldsToExclude));
                    }
                    else
                    {
                        if (childFilter.FieldName != null && fieldsToExclude.Contains(childFilter.FieldName))
                        {
                            newFilterGroup.Filters.Add(new AlwaysTrueRecordFilter());
                        }
                        else
                        {
                            newFilterGroup.Filters.Add(childFilter);
                        }
                    }
                }
            }
            return newFilterGroup;
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

        private class RecordFilterSetValueFromParametersContext : IRecordFilterSetValueFromParametersContext
        {
            Dictionary<Guid, dynamic> _parameterValues;

            public RecordFilterSetValueFromParametersContext(Dictionary<Guid, dynamic> parameterValues)
            {
                _parameterValues = parameterValues;
            }

            public bool TryGetParameterValue(Guid parameterId, out dynamic value)
            {
                if (_parameterValues != null)
                {
                    return _parameterValues.TryGetValue(parameterId, out value);
                }
                else
                {
                    value = null;
                    return false;
                }
            }
        }

        private class RecordFilterGetDescriptionContext : IRecordFilterGetDescriptionContext
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

        #endregion
    }

    public interface IRecordFilterGenericFieldMatchContext
    {
        Object GetFieldValue(string fieldName, out DataRecordFieldType fieldType);
    }

    public class DataRecordFilterGenericFieldMatchContext : IRecordFilterGenericFieldMatchContext
    {
        Dictionary<string, DataRecordField> _recordTypeFieldsByName;
        DataRecordObject _dataRecordObject;

        public DataRecordFilterGenericFieldMatchContext(DataRecordObject dataRecordObject)
        {
            if (dataRecordObject == null)
                throw new ArgumentNullException("dataRecordObject");
            _dataRecordObject = dataRecordObject;
            _recordTypeFieldsByName = (new DataRecordTypeManager()).GetDataRecordTypeFields(_dataRecordObject.DataRecordTypeId);
            if (_recordTypeFieldsByName == null)
                throw new NullReferenceException("_recordTypeFieldsByName");
        }

        public DataRecordFilterGenericFieldMatchContext(dynamic dataRecord, Guid recordTypeId)
            : this(new DataRecordObject(recordTypeId, dataRecord))
        {

        }

        public object GetFieldValue(string fieldName, out DataRecordFieldType fieldType)
        {
            DataRecordField field;
            if (!_recordTypeFieldsByName.TryGetValue(fieldName, out field))
                throw new NullReferenceException(String.Format("field. fieldName '{0}'", fieldName));
            fieldType = field.Type;
            return _dataRecordObject.GetFieldValue(fieldName);
        }
    }

    public class DataRecordDictFilterGenericFieldMatchContext : IRecordFilterGenericFieldMatchContext
    {
        Dictionary<string, dynamic> _dataRecord;
        Dictionary<string, DataRecordField> _recordTypeFieldsByName;

        public DataRecordDictFilterGenericFieldMatchContext(Dictionary<string, dynamic> dataRecord, Dictionary<string, DataRecordField> recordTypeFieldsByName)
        {
            if (dataRecord == null)
                throw new ArgumentNullException("dataRecord");
            _dataRecord = dataRecord;
            _recordTypeFieldsByName = recordTypeFieldsByName;
            if (_recordTypeFieldsByName == null)
                throw new NullReferenceException("_recordTypeFieldsByName");
        }

        public DataRecordDictFilterGenericFieldMatchContext(Dictionary<string, dynamic> dataRecord, Guid recordTypeId)
            : this(dataRecord, (new DataRecordTypeManager()).GetDataRecordTypeFields(recordTypeId))
        {

        }

        public object GetFieldValue(string fieldName, out DataRecordFieldType fieldType)
        {
            DataRecordField field;
            if (!_recordTypeFieldsByName.TryGetValue(fieldName, out field))
                throw new NullReferenceException(String.Format("field. fieldName '{0}'", fieldName));
            fieldType = field.Type;

            object fieldValue;
            _dataRecord.TryGetValue(fieldName, out fieldValue);
            return fieldValue;
        }
    }
}