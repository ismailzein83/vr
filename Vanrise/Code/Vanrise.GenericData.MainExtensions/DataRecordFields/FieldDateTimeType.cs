using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldDateTimeType : DataRecordFieldType
    {
        #region Public Methods

        public FieldDateTimeDataType DataType { get; set; }

        public override Type GetRuntimeType()
        {
            var attributeInfo = Utilities.GetEnumAttribute<FieldDateTimeDataType, FieldDateTimeDataTypeInfoAttribute>(this.DataType);
            if (attributeInfo == null)
                throw new NullReferenceException("FieldDateTimeDataTypeInfoAttribute");
            return attributeInfo.RuntimeType;
        }

        public override string GetDescription(object value)
        {
            if (value == null) { return null; }
            return (DataType == FieldDateTimeDataType.Time) ? GetTimeDescription(value) : GetDateTimeDescription(value);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            return (DataType == FieldDateTimeDataType.Time) ? DoTimesMatch(fieldValue, filterValue) : DoDateTimesMatch(fieldValue, filterValue);
        }
        
        #endregion

        #region Private Methods

        string GetTimeDescription(object fieldValue)
        {
            IEnumerable<Time> timeValues = FieldTypeHelper.ConvertFieldValueToList<Time>(fieldValue);

            if (timeValues == null)
            {
                Time time = fieldValue as Time;
                return time.ToLongTimeString();
            }

            var descriptions = new List<string>();

            foreach (Time timeValue in timeValues)
                descriptions.Add(timeValue.ToLongTimeString());

            return String.Join(",", descriptions);
        }

        string GetDateTimeDescription(object fieldValue)
        {
            IEnumerable<DateTime> dateTimeValues = FieldTypeHelper.ConvertFieldValueToList<DateTime>(fieldValue);

            if (dateTimeValues == null)
                return Convert.ToDateTime(fieldValue).ToString();

            var descriptions = new List<string>();

            foreach (DateTime dateTimeValue in dateTimeValues)
                descriptions.Add(GetDateTimeDescription(dateTimeValue));

            return String.Join(",", descriptions);
        }
        
        bool DoDateTimesMatch(object fieldValue, object filterValue)
        {
            IEnumerable<DateTime> dateTimeValues = FieldTypeHelper.ConvertFieldValueToList<DateTime>(fieldValue);
            return (dateTimeValues != null) ? dateTimeValues.Contains(Convert.ToDateTime(filterValue)) : Convert.ToDateTime(fieldValue).CompareTo(Convert.ToDateTime(filterValue)) == 0;
        }

        bool DoTimesMatch(object fieldValue, object filterValue)
        {
            IEnumerable<Time> timeValues = FieldTypeHelper.ConvertFieldValueToList<Time>(fieldValue);
            var filterValueAsTime = filterValue as Time;
            if (timeValues != null) {
                foreach (Time timeValue in timeValues)
                {
                    if (timeValue.Equals(filterValueAsTime))
                        return true;
                }
                return false;
            }
            else
            {
                var fieldValueAsTime = fieldValue as Time;
                return fieldValueAsTime.Equals(filterValueAsTime);
            }
        }

        #endregion

        public override GridColumnAttribute GetGridColumnAttribute()
        {
            string type;
            switch (DataType)
            {
                case FieldDateTimeDataType.DateTime: type = "LongDatetime"; break;
                case FieldDateTimeDataType.Date: type = "Date"; break;
                default: type = "Datetime"; break;
            }
            return new Vanrise.Entities.GridColumnAttribute() { Type = type, NumberPrecision = "NoDecimal" };
        }
    }
    public enum FieldDateTimeDataType
    {
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(DateTime))]
        DateTime = 0,
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(Vanrise.Entities.Time))]
        Time = 1,
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(DateTime))]
        Date = 2
    }

    public class FieldDateTimeDataTypeInfoAttribute : Attribute
    {
        public Type RuntimeType { get; set; }
    }
}
