using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldNumberType : DataRecordFieldType
    {
        public FieldNumberDataType DataType { get; set; }

        public override Type GetRuntimeType()
        {
            var attributeInfo = Utilities.GetEnumAttribute<FieldNumberDataType, FieldNumberDataTypeInfoAttribute>(this.DataType);
            if (attributeInfo == null)
                throw new NullReferenceException("FieldNumberDataTypeInfoAttribute");
            return attributeInfo.RuntimeType;
        }

        public override string GetDescription(Object value)
        {
            var staticValues = value as StaticValues;

            if (staticValues == null)
                return value.ToString();

            var descriptions = new List<string>();
            foreach (var staticValue in staticValues.Values)
                descriptions.Add(staticValue.ToString());
            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            if (fieldValue != null && filterValue != null)
            {
                var fieldValueIds = fieldValue as List<object>;
                fieldValueIds = (fieldValueIds == null) ? new List<object>() { fieldValue } : fieldValueIds;

                foreach (var fieldValueId in fieldValueIds)
                {
                    if (fieldValueId.Equals(filterValue))
                        return true;
                }
                return false;
            }
            return true;
        }
    }

    public enum FieldNumberDataType
    {
        [FieldNumberDataTypeInfo(RuntimeType = typeof(Decimal))]
        Decimal = 0,
        [FieldNumberDataTypeInfo(RuntimeType = typeof(int))]
        Int = 1,
        [FieldNumberDataTypeInfo(RuntimeType = typeof(long))]
        BigInt = 2
    }

    public class FieldNumberDataTypeInfoAttribute : Attribute
    {
        public Type RuntimeType { get; set; }
    }
}
