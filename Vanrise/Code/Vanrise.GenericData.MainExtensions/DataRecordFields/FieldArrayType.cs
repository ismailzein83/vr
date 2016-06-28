using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldArrayType : DataRecordFieldType
    {
        public DataRecordFieldType FieldType { get; set; }

        public override Type GetRuntimeType()
        {
            return GetNonNullableRuntimeType();
        }

        public override Type GetNonNullableRuntimeType()
        {
            Type fieldType = FieldType.GetRuntimeType();
            return typeof(List<>).MakeGenericType(fieldType);
        }

        public override string GetDescription(object value)
        {
            return FieldType.GetDescription(value);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            return FieldType.IsMatched(fieldValue, filterValue);
        }

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute()
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", NumberPrecision = "NoDecimal" };
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            throw new NotImplementedException();
        }

        public override RecordFilter ConvertToRecordFilter(List<Object> filterValues)
        {
            throw new NotImplementedException();
        }
    }
}
