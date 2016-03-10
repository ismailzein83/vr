using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldDateTimeType : DataRecordFieldType
    {
        public FieldDateTimeDataType DataType { get; set; }

        public override Type GetRuntimeType()
        {
            var attributeInfo = Utilities.GetEnumAttribute<FieldDateTimeDataType, FieldDateTimeDataTypeInfoAttribute>(this.DataType);
            if (attributeInfo == null)
                throw new NullReferenceException("FieldDateTimeDataTypeInfoAttribute");
            return attributeInfo.RuntimeType;
        }

        public override string GetDescription(Object value)
        {
            if (value == null)
                return null;

            IEnumerable<DateTime> selectedDateTimeValues = ConvertValueToSelectedDateTimeValues(value);

            if (selectedDateTimeValues == null)
                return Convert.ToDateTime(value).ToShortDateString();

            var descriptions = new List<string>();

            foreach (DateTime selectedDateTimeValue in selectedDateTimeValues)
                descriptions.Add(selectedDateTimeValue.ToShortDateString());

            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            return Convert.ToDateTime(fieldValue).CompareTo(Convert.ToDateTime(filterValue)) == 0;
        }

        #region Private Methods

        IEnumerable<DateTime> ConvertValueToSelectedDateTimeValues(object value)
        {
            var staticValues = value as StaticValues;
            if (staticValues != null)
                return staticValues.Values.MapRecords(itm => Convert.ToDateTime(itm));

            var objList = value as List<object>;
            if (objList != null)
                return objList.MapRecords(itm => Convert.ToDateTime(itm));

            return null;
        }

        #endregion
    }
    public enum FieldDateTimeDataType
    {
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(DateTime))]
        DateTime = 0,
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(DateTime))]
        Time = 1,
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(DateTime))]
        Date = 2
    }

    public class FieldDateTimeDataTypeInfoAttribute : Attribute
    {
        public Type RuntimeType { get; set; }
    }
}
