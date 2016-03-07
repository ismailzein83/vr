using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

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
            return value.ToString();
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            return Convert.ToDateTime(fieldValue).CompareTo(Convert.ToDateTime(filterValue)) == 0;
        }
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
