using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldByteArrayType : DataRecordFieldType
    {
        public override Guid ConfigId => new Guid("A2ABE57B-FCB6-4C05-8F6A-FB28E3D6A360");

        public override string RuntimeEditor => "";

        public override RecordFilter ConvertToRecordFilter(IDataRecordFieldTypeConvertToRecordFilterContext context)
        {
            throw new NotImplementedException();
        }

        public override RDBDataRecordFieldAttribute GetDefaultRDBFieldAttribute(IDataRecordFieldTypeDefaultRDBFieldAttributeContext context)
        {
            return new RDBDataRecordFieldAttribute
            {
                RdbDataType = RDBDataType.VarBinary
            };
        }

        public override string GetDescription(object value)
        {
            return "Binary Content";
        }

        public override GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            return new GridColumnAttribute { Field = context != null ? context.DescriptionFieldPath : null };
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(byte[]);
        }

        public override Type GetRuntimeType()
        {
            return GetNonNullableRuntimeType();
        }

        public override string GetRuntimeTypeDescription()
        {
            return "byte[]";
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            return false;
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            return false;
        }
    }
}
