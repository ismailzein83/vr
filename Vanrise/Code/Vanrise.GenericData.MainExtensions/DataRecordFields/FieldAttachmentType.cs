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
        public override bool AreEqual(Object newValue, Object oldValue)
        {
            if (newValue == null && oldValue == null)
                return true;

            if (newValue == null || oldValue == null)
                return false;

            var newValueObject = newValue as List<AttachmentFieldTypeEntity>;
            var oldValueObject = oldValue as List<AttachmentFieldTypeEntity>;

            if (newValueObject != null && oldValue != null)
            {
                foreach (var newAttachmentField in newValueObject)
                {
                    if (!oldValueObject.Any(x => x.FileId == newAttachmentField.FileId && x.Notes == newAttachmentField.Notes && x.CreatedTime == newAttachmentField.CreatedTime))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public override Type GetNonNullableRuntimeType()
        {
            return typeof(List<AttachmentFieldTypeEntity>);
        }
        public override bool StoreValueSerialized { get { return true; } }

        public override string GetDescription(object value)
        {
            return null;
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
