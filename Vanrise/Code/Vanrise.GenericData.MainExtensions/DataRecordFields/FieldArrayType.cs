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
        public override Guid ConfigId { get { return new Guid("034021E9-3BA1-4971-8AA9-CCF6ED2C2C80"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-fieldtype-array-runtimeeditor"; } }

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

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", NumberPrecision = "NoDecimal", Field = context != null ? context.DescriptionFieldPath : null };
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            throw new NotImplementedException();
        }

        public override RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues)
        {
            throw new NotImplementedException();
        }
        protected override dynamic ParseNonNullValueToFieldType(Object originalValue)
        {
            return originalValue;
        }
    }
}
