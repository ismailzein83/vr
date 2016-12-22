using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using System.Globalization;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public enum FieldNumberPrecision { Normal = 0, Long = 1 }
    public class FieldNumberType : DataRecordFieldType
    {
        public override Guid ConfigId { get { return new Guid("75aef329-27bd-4108-b617-f5cc05ff2aa3"); } }

        public FieldNumberPrecision DataPrecision { get; set; }
        public FieldNumberDataType DataType { get; set; }
        public bool IsNullable { get; set; }

        #region Public Methods

        public override Type GetRuntimeType()
        {
            var type = GetNonNullableRuntimeType();
            return (IsNullable) ? GetNullableType(type) : type;
        }

        public override Type GetNonNullableRuntimeType()
        {
            var attributeInfo = Utilities.GetEnumAttribute<FieldNumberDataType, FieldNumberDataTypeInfoAttribute>(this.DataType);
            if (attributeInfo == null)
                throw new NullReferenceException("FieldNumberDataTypeInfoAttribute");
            return attributeInfo.RuntimeType;
        }

        public override string GetDescription(Object value)
        {
            if (value == null)
                return null;

            IEnumerable<object> numberValues = FieldTypeHelper.ConvertFieldValueToList<object>(value);

            string decimalPrecision = null;
            switch (DataType)
            {
                case FieldNumberDataType.Decimal:
                    switch (DataPrecision)
                    {
                        case FieldNumberPrecision.Long: decimalPrecision = "0.0000"; break;
                        case FieldNumberPrecision.Normal: 
                        default: decimalPrecision = "0.00"; break;
                    }
                    break;

                default: break;
            }


            if (numberValues == null)
            {
                if (string.IsNullOrEmpty(decimalPrecision))
                    return value.ToString();
                else
                    return Convert.ToDecimal(value).ToString(decimalPrecision);
            }

            var descriptions = new List<string>();

            if (DataType == FieldNumberDataType.Decimal)
            {
                foreach (var numberValue in numberValues)
                    descriptions.Add(Convert.ToDecimal(numberValue).ToString(decimalPrecision));
            }
            else
            {
                foreach (var numberValue in numberValues)
                    descriptions.Add(numberValue.ToString());
            }
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

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            if (fieldValue == null)
                return false;
            NumberRecordFilter numberRecordFilter = recordFilter as NumberRecordFilter;
            if (numberRecordFilter == null)
                throw new NullReferenceException("numberRecordFilter");
            Decimal valueAsDecimal = Convert.ToDecimal(fieldValue);
            Decimal filterValue = numberRecordFilter.Value;
            switch (numberRecordFilter.CompareOperator)
            {
                case NumberRecordFilterOperator.Equals: return valueAsDecimal == filterValue;
                case NumberRecordFilterOperator.NotEquals: return valueAsDecimal != filterValue;
                case NumberRecordFilterOperator.Greater: return valueAsDecimal > filterValue;
                case NumberRecordFilterOperator.GreaterOrEquals: return valueAsDecimal >= filterValue;
                case NumberRecordFilterOperator.Less: return valueAsDecimal < filterValue;
                case NumberRecordFilterOperator.LessOrEquals: return valueAsDecimal <= filterValue;
            }
            return false;
        }

        #endregion

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            string numberPrecision;
            switch (DataType)
            {
                case FieldNumberDataType.Decimal:
                    switch (DataPrecision)
                    {
                        case FieldNumberPrecision.Long: numberPrecision = "LongPrecision"; break;
                        case FieldNumberPrecision.Normal:
                        default: numberPrecision = ""; break;
                    }
                    
                    break;
                case FieldNumberDataType.BigInt:
                case FieldNumberDataType.Int:
                default: numberPrecision = "NoDecimal"; break;
            }
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Number", NumberPrecision = numberPrecision };
        }

        public override RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues)
        {
            var values = filterValues.Select(value => Convert.ToDecimal(value)).ToList();
            RecordFilterGroup recordFilterGroup = new RecordFilterGroup
            {
                LogicalOperator = RecordQueryLogicalOperator.Or,
                Filters = new List<RecordFilter>(),
            };
            foreach (var value in values)
            {
                recordFilterGroup.Filters.Add(new NumberRecordFilter
                {
                    CompareOperator = NumberRecordFilterOperator.Equals,
                    Value = value,
                    FieldName = fieldName
                });
            }
            return recordFilterGroup;
        }

        public override string GetFilterDescription(RecordFilter filter)
        {
            NumberRecordFilter numberRecordFilter = filter as NumberRecordFilter;
            return string.Format(" {0} {1} {2} ", numberRecordFilter.FieldName, Utilities.GetEnumDescription(numberRecordFilter.CompareOperator), GetDescription(numberRecordFilter.Value));
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
