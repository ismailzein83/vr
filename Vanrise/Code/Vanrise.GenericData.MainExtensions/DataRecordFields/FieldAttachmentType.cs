using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldAttachmentType : DataRecordFieldType
    {
        public override Guid ConfigId
        {
            get { return new Guid("A80260FD-4492-45C9-8E60-41D91DCD4E9E"); }
        }
        public bool IsNullable { get; set; }

        public override Type GetRuntimeType()
        {
            var type = GetNonNullableRuntimeType();
            return (IsNullable) ? GetNullableType(type) : type;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(string);
        }
        public override bool StoreValueSerialized { get { return true; } }

        public override string GetDescription(object value)
        {
            throw new NotImplementedException();
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            throw new NotImplementedException();
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            throw new NotImplementedException();
        }

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            throw new NotImplementedException();
        }

        public override RecordFilter ConvertToRecordFilter(string fieldName, List<object> filterValues)
        {
            throw new NotImplementedException();
        }


        public override string RuntimeEditor
        {
            get { return null; }
        }
    }
}
