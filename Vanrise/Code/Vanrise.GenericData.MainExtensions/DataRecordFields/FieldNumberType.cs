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
        public bool IsNullable { get; set; }

        #region Public Methods

        public override Type GetRuntimeType()
        {
            var attributeInfo = Utilities.GetEnumAttribute<FieldNumberDataType, FieldNumberDataTypeInfoAttribute>(this.DataType);
            if (attributeInfo == null)
                throw new NullReferenceException("FieldNumberDataTypeInfoAttribute");
            return (IsNullable) ? GetNullableType(attributeInfo.RuntimeType) : attributeInfo.RuntimeType;
        }

        public override string GetDescription(Object value)
        {
            if (value == null)
                return null;

            IEnumerable<object> numberValues = FieldTypeHelper.ConvertFieldValueToList<object>(value);

            if (numberValues == null)
                return value.ToString();

            var descriptions = new List<string>();
            foreach (var numberValue in numberValues)
                descriptions.Add(numberValue.ToString());
            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            if (fieldValue != null && filterValue != null)
            {
                FieldNumberDataTypeInfoAttribute infoAttribute = Utilities.GetEnumAttribute<FieldNumberDataType, FieldNumberDataTypeInfoAttribute>(DataType);
                Type dataType = infoAttribute.RuntimeType;

                IEnumerable<object> fieldValues = FieldTypeHelper.ConvertFieldValueToList<object>(fieldValue);
                object convertedFilterValue = Convert.ChangeType(filterValue, dataType);

                if (fieldValues == null)
                    return (Convert.ChangeType(fieldValue, dataType).Equals(convertedFilterValue));

                foreach (var fieldValueObject in fieldValues)
                {
                    if (Convert.ChangeType(fieldValueObject, dataType).Equals(convertedFilterValue))
                        return true;
                }
                return false;
            }
            return true;
        }

        #endregion

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute()
        {
            string numberPrecision;
            switch (DataType)
            {
                case FieldNumberDataType.Decimal: numberPrecision = ""; break;
                case FieldNumberDataType.BigInt:
                case FieldNumberDataType.Int:
                default: numberPrecision = "NoDecimal"; break;
            }
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Number", NumberPrecision = numberPrecision };
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
