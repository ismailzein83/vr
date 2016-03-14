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
            IEnumerable<DateTime> dateTimeValues = ConvertFieldValueToList<DateTime>(fieldValue);
            return (dateTimeValues != null) ? dateTimeValues.Contains(Convert.ToDateTime(filterValue)) : Convert.ToDateTime(fieldValue).CompareTo(Convert.ToDateTime(filterValue)) == 0;
        }
        
        #endregion

        #region Private Methods

        string GetTimeDescription(object fieldValue)
        {
            IEnumerable<Time> timeValues = ConvertFieldValueToList<Time>(fieldValue);

            if (timeValues == null)
            {
                Time time = Serializer.Deserialize<Time>(fieldValue.ToString());
                return time.ToLongTimeString();
            }

            var descriptions = new List<string>();

            foreach (Time timeValue in timeValues)
                descriptions.Add(timeValue.ToLongTimeString());

            return String.Join(",", descriptions);
        }

        string GetDateTimeDescription(object fieldValue)
        {
            IEnumerable<DateTime> dateTimeValues = ConvertFieldValueToList<DateTime>(fieldValue);

            if (dateTimeValues == null)
                return Convert.ToDateTime(fieldValue).ToString();

            var descriptions = new List<string>();

            foreach (DateTime dateTimeValue in dateTimeValues)
                descriptions.Add(GetDateTimeDescription(dateTimeValue));

            return String.Join(",", descriptions);
        }
        
        #endregion
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
