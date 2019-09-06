using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldDataRecordType : DataRecordFieldType
    {
        public override Guid ConfigId => new Guid("19519705-330A-4E5B-A2E9-BCD7746BE99F");

        public override string RuntimeEditor => "";

        public Guid DataRecordTypeId { get; set; }

        public override RecordFilter ConvertToRecordFilter(IDataRecordFieldTypeConvertToRecordFilterContext context)
        {
            throw new NotImplementedException();
        }

        public override RDBDataRecordFieldAttribute GetDefaultRDBFieldAttribute(IDataRecordFieldTypeDefaultRDBFieldAttributeContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetDescription(object value)
        {
            throw new NotImplementedException();
        }

        public override GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            throw new NotImplementedException();
        }

        public override Type GetNonNullableRuntimeType()
        {
            return new DataRecordTypeManager().GetDataRecordRuntimeType(this.DataRecordTypeId);
        }

        public override Type GetRuntimeType()
        {
            return GetNonNullableRuntimeType();
        }

        public override string GetNonNullableRuntimeTypeAsString()
        {
            return new DataRecordTypeManager().GetDataRecordRuntimeTypeAsString(this.DataRecordTypeId);
        }

        public override string GetRuntimeTypeAsString()
        {
            return GetNonNullableRuntimeTypeAsString();
        }

        public override bool IsValueType()
        {
            return false;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Data Record";
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            throw new NotImplementedException();
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            throw new NotImplementedException();
        }
    }
}
