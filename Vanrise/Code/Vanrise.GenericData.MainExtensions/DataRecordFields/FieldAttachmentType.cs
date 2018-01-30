using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
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
        public override bool TryResolveDifferences(IDataRecordFieldTypeTryResolveDifferencesContext context)
        {
            var oldAttachmentFieldTypeEntities = context.OldValue as AttachmentFieldTypeEntityCollection;
            var copyOldAttachmentFieldTypeEntities = oldAttachmentFieldTypeEntities.VRDeepCopy();

            var newAttachmentFieldTypeEntities = context.NewValue as AttachmentFieldTypeEntityCollection;

            var changesFieldTypeEntities = new List<AttachmentFieldTypeEntityChangeInfo>();

            if (newAttachmentFieldTypeEntities != null)
            {
                foreach (var newAttachmentFieldTypeEntity in newAttachmentFieldTypeEntities)
                {
                    var itemFound = copyOldAttachmentFieldTypeEntities.FindRecord(x => x.FileId == newAttachmentFieldTypeEntity.FileId);
                    if (itemFound != null)
                    {
                        if (!itemFound.Notes.Equals(newAttachmentFieldTypeEntity.Notes))
                        {
                            changesFieldTypeEntities.Add(new AttachmentFieldTypeEntityChangeInfo
                            {
                                FileId = newAttachmentFieldTypeEntity.FileId,
                                Description = string.Format("Notes changed from {0} to {1}", itemFound.Notes, newAttachmentFieldTypeEntity.Notes)
                            });
                        }
                        copyOldAttachmentFieldTypeEntities.Remove(itemFound);
                    }
                    else
                    {
                        changesFieldTypeEntities.Add(new AttachmentFieldTypeEntityChangeInfo
                        {
                            FileId = newAttachmentFieldTypeEntity.FileId,
                            Description = string.Format("Added ( Notes: {0} )", newAttachmentFieldTypeEntity.Notes)
                        });
                    }
                }

                foreach (var copyOldAttachmentFieldTypeEntity in copyOldAttachmentFieldTypeEntities)
                {
                    changesFieldTypeEntities.Add(new AttachmentFieldTypeEntityChangeInfo
                    {
                        FileId = copyOldAttachmentFieldTypeEntity.FileId,
                        Description = string.Format("Deleted")
                    });
                }
            }
            context.Changes = changesFieldTypeEntities;
            return true;
        }
        public override string DifferenceEditor { get { return "vr-genericdata-fieldtype-attachment-differenceeditor"; } }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            if (newValue == null && oldValue == null)
                return true;

            if (newValue == null || oldValue == null)
                return false;

            var newValueObject = newValue as AttachmentFieldTypeEntityCollection;
            var oldValueObject = oldValue as AttachmentFieldTypeEntityCollection;

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
            return typeof(AttachmentFieldTypeEntityCollection);
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
