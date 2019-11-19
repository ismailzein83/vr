using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldArrayType : DataRecordFieldType
    {
        public override Guid ConfigId { get { return new Guid("82E04A93-5A3F-4C32-80A4-08E1DAFCF305"); } } 

        public override string RuntimeEditor { get { return "vr-genericdata-fieldtype-array-runtimeeditor"; } }

        public DataRecordFieldType FieldType { get; set; }

        public override Type GetRuntimeType()
        {
            return GetNonNullableRuntimeType();
        }

        Type _nonNullableRuntimeType;
        public override Type GetNonNullableRuntimeType()
        {
            if (_nonNullableRuntimeType == null)
            {
                lock (this)
                {
                    if (_nonNullableRuntimeType == null)
                    {
                        Type fieldType = FieldType.GetRuntimeType();
                        _nonNullableRuntimeType = typeof(List<>).MakeGenericType(fieldType);
                    }
                }

            }
            return _nonNullableRuntimeType;
        }

        public override string GenerateValueCode(object value)
        {
            if (value == null)
                return "null";
            var valueItem = Utilities.ConvertJsonToList<dynamic>(value);
            return string.Concat("{", string.Join(",", valueItem), "}");
        }

        public override RDBDataRecordFieldAttribute GetDefaultRDBFieldAttribute(IDataRecordFieldTypeDefaultRDBFieldAttributeContext context)
        {
            return new RDBDataRecordFieldAttribute
            {
                RdbDataType = RDBDataType.NVarchar
            };
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

        protected override dynamic ParseNonNullValueToFieldType(Object originalValue)
        {
            return originalValue;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Array";
        }

        public override RecordFilter ConvertToRecordFilter(IDataRecordFieldTypeConvertToRecordFilterContext context)
        {
            throw new NotImplementedException();
        }

        public override bool StoreValueSerialized { get { return true; } }
        public override string SerializeValue(ISerializeDataRecordFieldValueContext context)
        {
            context.ThrowIfNull("context");
            if (context.Object == null)
                return string.Empty;

            return Vanrise.Common.Serializer.Serialize(context.Object, true);
        }

        public override object DeserializeValue(IDeserializeDataRecordFieldValueContext context)
        {
            context.ThrowIfNull("context");
            if (string.IsNullOrEmpty(context.Value))
                return null;

            return Vanrise.Common.Serializer.Deserialize(context.Value, GetRuntimeType());
        }

        public override bool IsCompatibleWithFieldType(DataRecordFieldType fieldType)
        {
            FieldArrayType fieldArrayType = fieldType as FieldArrayType;
            if (fieldArrayType == null)
                return false;
            return fieldArrayType.FieldType.IsCompatibleWithFieldType(this.FieldType);
        }
    }
}