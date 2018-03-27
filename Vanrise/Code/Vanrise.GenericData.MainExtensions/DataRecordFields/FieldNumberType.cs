﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using System.Globalization;
using Vanrise.Common.Business;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public enum FieldNumberPrecision { Normal = 0, Long = 1 }

    public class FieldNumberType : DataRecordFieldType
    {
        public override Guid ConfigId { get { return new Guid("75aef329-27bd-4108-b617-f5cc05ff2aa3"); } }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            if (newValue == null && oldValue == null)
                return true;
         
            if (newValue == null || oldValue == null)
                return false;

            switch (DataType)
            {
                case FieldNumberDataType.BigInt:
                    long newLongValue = (long)newValue;
                    long oldLongValue = (long)oldValue;
                    return newLongValue.Equals(oldLongValue);

                case FieldNumberDataType.Decimal:
                    decimal newDecimalValue = (decimal)newValue;
                    decimal oldDecimalValue = (decimal)oldValue;
                    return newDecimalValue.Equals(oldDecimalValue);

                case FieldNumberDataType.Int:
                    int newIntValue = (int)newValue;
                    int oldIntValue = (int)oldValue;
                    return newIntValue.Equals(oldIntValue);
                default: return false;
            }
        }
        public override string ViewerEditor { get { return "vr-genericdata-fieldtype-number-viewereditor"; } }

        public override string RuntimeEditor { get { return "vr-genericdata-fieldtype-number-runtimeeditor"; } }

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
                    decimalPrecision = "N";
                    switch (DataPrecision)
                    {
                        case FieldNumberPrecision.Long:
                            var longPrecision = new GeneralSettingsManager().GetLongPrecision();
                            decimalPrecision += longPrecision;
                            break;
                        case FieldNumberPrecision.Normal:
                            var normalPrecision = new GeneralSettingsManager().GetNormalPrecision();
                            decimalPrecision += normalPrecision;
                            break;
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
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Number", NumberPrecision = numberPrecision, Field = context != null ? context.ValueFieldPath : null };
        }

        public override RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues)
        {
            if (filterValues == null || filterValues.Count == 0)
                return null;

            var values = filterValues.Select(value => Convert.ToDecimal(value)).ToList();
            List<RecordFilter> recordFilters = new List<RecordFilter>();

            foreach (var value in values)
            {
                recordFilters.Add(new NumberRecordFilter
                {
                    CompareOperator = NumberRecordFilterOperator.Equals,
                    Value = value,
                    FieldName = fieldName
                });
            }
            return recordFilters.Count > 1 ? new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.Or, Filters = recordFilters } : recordFilters.First();
        }

        public override bool CanRoundValue { get { return true; } }
        public override dynamic GetRoundedValue(dynamic value)
        {
            switch (this.DataType)
            {
                case FieldNumberDataType.Int:
                case FieldNumberDataType.BigInt:
                    return value;

                case FieldNumberDataType.Decimal:
                    GeneralSettingsManager generalSettingsManager = new GeneralSettingsManager();

                    decimal valueAsDecimal = Convert.ToDecimal(value);
                    switch (this.DataPrecision)
                    {
                        case FieldNumberPrecision.Normal:
                            int normalePrecision = generalSettingsManager.GetNormalPrecisionValue();
                            return Math.Round(valueAsDecimal, normalePrecision);

                        case FieldNumberPrecision.Long:
                            int longPrecision = generalSettingsManager.GetLongPrecisionValue();
                            return Math.Round(valueAsDecimal, longPrecision);

                        default: throw new NotSupportedException(string.Format("FieldNumberPrecision {0} not supported.", this.DataPrecision));
                    }

                default: throw new NotSupportedException(string.Format("FieldNumberDataType {0} not supported.", this.DataType));
            }
        }
        public override void GetValueByDescription(IDataRecordFieldTypeTryGetValueByDescriptionContext context)
        {

            if (context.FieldDescription == null)
                return;
            else
            {
                switch (this.DataType)
                {
                    case FieldNumberDataType.Decimal:
                        Decimal outcome;
                        if (context.FieldDescription is Decimal)
                        {
                            outcome = (Decimal)context.FieldDescription;

                            GeneralSettingsManager generalSettingsManager = new GeneralSettingsManager();
                            switch (this.DataPrecision)
                            {
                                case FieldNumberPrecision.Normal:
                                    int normalPrecision = generalSettingsManager.GetNormalPrecisionValue();
                                    Math.Round(outcome, normalPrecision);
                                    context.FieldValue = outcome;
                                    break;
                                case FieldNumberPrecision.Long:
                                    int longPrecision = generalSettingsManager.GetLongPrecisionValue();
                                    Math.Round(outcome, longPrecision);
                                    context.FieldValue = outcome;
                                    break;
                                default: throw new NotSupportedException(string.Format("FieldNumberPrecision {0} not supported.", this.DataPrecision));
                            }
                        }
                        else
                        {
                            bool success;

                            success = Decimal.TryParse(context.FieldDescription.ToString(), out outcome);
                            if (success)
                            {
                                context.FieldValue = outcome;

                                GeneralSettingsManager generalSettingsManager = new GeneralSettingsManager();
                                switch (this.DataPrecision)
                                {
                                    case FieldNumberPrecision.Normal:
                                        int normalPrecision = generalSettingsManager.GetNormalPrecisionValue();
                                        Math.Round(outcome, normalPrecision);
                                        context.FieldValue = outcome;
                                        break;
                                    case FieldNumberPrecision.Long:
                                        int longPrecision = generalSettingsManager.GetLongPrecisionValue();
                                        Math.Round(outcome, longPrecision);
                                        context.FieldValue = outcome;
                                        break;
                                    default: throw new NotSupportedException(string.Format("FieldNumberPrecision {0} not supported.", this.DataPrecision));
                                }
                            }

                        }
                        break;
                    case FieldNumberDataType.Int:
                         if (context.FieldDescription is int)
                             context.FieldValue = (int)context.FieldDescription;
                        else
                        {
                            bool success;
                            int result;
                            success = int.TryParse(context.FieldDescription.ToString(), out result);
                            if (success)
                                context.FieldValue = result;
 
                        }
                        break;

                    case FieldNumberDataType.BigInt:
                        if (context.FieldDescription is long)
                            context.FieldValue = (long)context.FieldDescription;
                        else
                        {
                            bool success;
                            long result;
                            success = long.TryParse(context.FieldDescription.ToString(), out result);
                            if (success)
                                context.FieldValue = result;
                        }
                        break;
                    default: 
                        context.ErrorMessage = "Error while parsing FieldNumberType";
                        break;
                        
                }
            }

        }

        #endregion
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
