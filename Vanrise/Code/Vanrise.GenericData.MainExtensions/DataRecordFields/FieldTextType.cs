﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldTextType : DataRecordFieldType
    {
        public override DataRecordFieldOrderType OrderType
        {
            get
            {
                return DataRecordFieldOrderType.ByFieldDescription;
            }
        }
        public override Type GetRuntimeType()
        {
            return GetNonNullableRuntimeType();
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(string);
        }

        public override string GetDescription(Object value)
        {
            if (value == null)
                return null;

            IEnumerable<string> textValues = FieldTypeHelper.ConvertFieldValueToList<string>(value);

            if (textValues == null)
                return value.ToString();
            
            var descriptions = new List<string>();
            foreach (var textValue in textValues)
                descriptions.Add(textValue.ToString());
            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            if (fieldValue != null && filterValue != null)
            {
                var fieldValueObjList = fieldValue as List<object>;
                fieldValueObjList = (fieldValueObjList == null) ? new List<object>() { fieldValue } : fieldValueObjList;

                var fieldValueStringList = fieldValueObjList.MapRecords(itm => Convert.ToString(itm).ToUpper());
                var filterValueString = filterValue.ToString().ToUpper();
                
                foreach (var fieldValueStringItem in fieldValueStringList)
                {
                    if (fieldValueStringItem.Contains(filterValueString))
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
            StringRecordFilter stringRecordFilter = recordFilter as StringRecordFilter;
            if (stringRecordFilter == null)
                throw new NullReferenceException("stringRecordFilter");
            string valueAsString = fieldValue as string;
            if(valueAsString == null)
                throw new NullReferenceException("valueAsString");
            string filterValue  = stringRecordFilter.Value;
             if(filterValue == null)
                throw new NullReferenceException("filterValue");
            switch(stringRecordFilter.CompareOperator)
            {
                case StringRecordFilterOperator.Equals: return String.Compare(valueAsString, filterValue, false) == 0;
                case StringRecordFilterOperator.NotEquals: return String.Compare(valueAsString, filterValue, false) != 0;
                case StringRecordFilterOperator.Contains: return valueAsString.Contains(filterValue);
                case StringRecordFilterOperator.NotContains: return !valueAsString.Contains(filterValue);
                case StringRecordFilterOperator.StartsWith: return valueAsString.StartsWith(filterValue);
                case StringRecordFilterOperator.NotStartsWith: return !valueAsString.StartsWith(filterValue);
                case StringRecordFilterOperator.EndsWith: return valueAsString.EndsWith(filterValue);
                case StringRecordFilterOperator.NotEndsWith: return !valueAsString.EndsWith(filterValue);
            }
            return false;
        }

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute()
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", NumberPrecision = "NoDecimal" };
        }
    }
}
