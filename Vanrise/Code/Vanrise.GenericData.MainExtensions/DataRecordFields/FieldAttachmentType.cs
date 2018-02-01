using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
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
            var oldAttachmentFieldTypeEntities = new AttachmentFieldTypeEntityCollection();
            var newAttachmentFieldTypeEntities = new AttachmentFieldTypeEntityCollection();

            var oldEnumerator = (context.OldValue as System.Collections.IEnumerable).GetEnumerator();
            if (oldEnumerator != null)
            {
                while (oldEnumerator.MoveNext())
                {
                    if (oldEnumerator.Current != null)
                    {
                        var oldEnumeratorEntity = Vanrise.Common.Serializer.Deserialize<AttachmentFieldTypeEntity>(oldEnumerator.Current.ToString());
                        oldAttachmentFieldTypeEntities.Add(oldEnumeratorEntity);
                    }
                }
            }

            var newEnumerator = (context.NewValue as System.Collections.IEnumerable).GetEnumerator();
            if (newEnumerator != null)
            {
                while (newEnumerator.MoveNext())
                {
                    if (newEnumerator.Current != null)
                    {
                        var newEnumeratorEntity = Vanrise.Common.Serializer.Deserialize<AttachmentFieldTypeEntity>(newEnumerator.Current.ToString());
                        newAttachmentFieldTypeEntities.Add(newEnumeratorEntity);
                    }
                }
            }

            var changesFieldTypeEntities = new List<AttachmentFieldTypeEntityChangeInfo>();
            VRFileManager vrFileManager = new VRFileManager();

            if (newAttachmentFieldTypeEntities != null)
            {
                foreach (var newAttachmentFieldTypeEntity in newAttachmentFieldTypeEntities)
                {
                    var itemFound = oldAttachmentFieldTypeEntities.FindRecord(x => x.FileId == newAttachmentFieldTypeEntity.FileId);
                    if (itemFound != null)
                    {

                        if (!itemFound.Notes.Equals(newAttachmentFieldTypeEntity.Notes))
                        {
                            var fileInfo = vrFileManager.GetFileInfo(newAttachmentFieldTypeEntity.FileId);

                            changesFieldTypeEntities.Add(new AttachmentFieldTypeEntityChangeInfo
                            {
                                FileName = fileInfo != null ? fileInfo.Name : null,
                                FileId = newAttachmentFieldTypeEntity.FileId,
                                Description = string.Format("Notes changed from '{0}' to '{1}'", itemFound.Notes, newAttachmentFieldTypeEntity.Notes)
                            });
                        }
                        oldAttachmentFieldTypeEntities.Remove(itemFound);
                    }
                    else
                    {
                        var fileInfo = vrFileManager.GetFileInfo(newAttachmentFieldTypeEntity.FileId);
                        changesFieldTypeEntities.Add(new AttachmentFieldTypeEntityChangeInfo
                        {
                            FileName = fileInfo != null ? fileInfo.Name : null,
                            FileId = newAttachmentFieldTypeEntity.FileId,
                            Description = string.Format("Added (Notes: {0})", newAttachmentFieldTypeEntity.Notes)
                        });
                    }
                }
                if (oldAttachmentFieldTypeEntities != null)
                {
                    foreach (var copyOldAttachmentFieldTypeEntity in oldAttachmentFieldTypeEntities)
                    {
                        var fileInfo = vrFileManager.GetFileInfo(copyOldAttachmentFieldTypeEntity.FileId);
                        changesFieldTypeEntities.Add(new AttachmentFieldTypeEntityChangeInfo
                        {
                            FileName = fileInfo != null? fileInfo.Name:null,
                            FileId = copyOldAttachmentFieldTypeEntity.FileId,
                            Description = string.Format("Deleted")
                        });
                    }
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
