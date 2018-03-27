using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
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
            return GetNonNullableRuntimeType();
        }
        public override bool TryResolveDifferences(IDataRecordFieldTypeTryResolveDifferencesContext context)
        {

            var oldAttachmentFieldTypeEntities = context.OldValue as List<AttachmentFieldTypeEntity>;
            var newAttachmentFieldTypeEntities = context.NewValue as List<AttachmentFieldTypeEntity>;
         
            if (oldAttachmentFieldTypeEntities == null)
                oldAttachmentFieldTypeEntities = Utilities.ConvertJsonToList<AttachmentFieldTypeEntity>(context.OldValue);
            if (newAttachmentFieldTypeEntities == null)
                newAttachmentFieldTypeEntities = Utilities.ConvertJsonToList<AttachmentFieldTypeEntity>(context.NewValue);
          
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
                        var attachmentFieldTypeEntityChangeInfo = new AttachmentFieldTypeEntityChangeInfo
                        {
                            FileName = fileInfo != null ? fileInfo.Name : null,
                            FileId = newAttachmentFieldTypeEntity.FileId,
                            Description = "Added"
                        };
                        if (newAttachmentFieldTypeEntity.Notes != null)
                        {
                            attachmentFieldTypeEntityChangeInfo.Description += string.Format(" (Notes: {0})", newAttachmentFieldTypeEntity.Notes);
                        }
                        changesFieldTypeEntities.Add(attachmentFieldTypeEntityChangeInfo);
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
                    if (!oldValueObject.Any(x => x.FileId == newAttachmentField.FileId && x.Notes == newAttachmentField.Notes))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        Type _nonNullableRuntimeType = typeof(AttachmentFieldTypeEntityCollection);
        public override Type GetNonNullableRuntimeType()
        {
            return _nonNullableRuntimeType;
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

        public override void onBeforeSave(IDataRecordFieldTypeOnBeforeSaveContext context)
        {
            var attachmentFieldTypeEntities = context.FieldValue as List<AttachmentFieldTypeEntity>;
            if (attachmentFieldTypeEntities != null)
            {
                List<long> fileIds = new List<long>();

                foreach (var attachmentFieldTypeEntity in attachmentFieldTypeEntities)
                {
                    attachmentFieldTypeEntity.CreatedTime = DateTime.Now;
                    fileIds.Add(attachmentFieldTypeEntity.FileId);
                }

                if (context.BusinessEntityDefinitionId.HasValue)
                {
                    if (fileIds != null && fileIds.Count() > 0)
                    {
                        SetBusinessEntityFilesUsed(fileIds, context.BusinessEntityDefinitionId.Value, context.BusinessEntityId);
                    }
                }
            }
           
            
        }
        public override void onAfterSave(IDataRecordFieldTypeOnAfterSaveContext context)
        {
            var attachmentFieldTypeEntities = context.FieldValue as List<AttachmentFieldTypeEntity>;
            if (attachmentFieldTypeEntities != null)
            {
                var fileIds = attachmentFieldTypeEntities.MapRecords(x => x.FileId);
                if (context.BusinessEntityDefinitionId.HasValue)
                {
                    if (fileIds != null && fileIds.Count() > 0)
                    {
                        SetBusinessEntityFilesUsed(fileIds, context.BusinessEntityDefinitionId.Value, context.BusinessEntityId);
                    }
                }
            }

        }

        private void SetBusinessEntityFilesUsed(IEnumerable<long> fileIds, Guid businessEntityDefinitionId, Object businessEntityId)
        {
            if (fileIds != null && fileIds.Count() > 0)
            {
                VRFileManager fileManager = new VRFileManager();
                foreach (var fileId in fileIds)
                {
                    var fileSettings = new VRFileSettings { ExtendedSettings = new BusinessEntityFileSettings { BusinessEntityDefinitionId = businessEntityDefinitionId, BusinessEntityId = businessEntityId } };
                    fileManager.SetFileUsedAndUpdateSettings(fileId, fileSettings);
                }
            }
        }
        protected override dynamic ParseNonNullValueToFieldType(Object originalValue)
        {
            return originalValue;
        }
    }

    public class BusinessEntityFileSettings : VRFileExtendedSettings
    {
        public Guid BusinessEntityDefinitionId { get; set; }
        public Object BusinessEntityId { get; set; }

        public override Guid ConfigId
        {
            get { return new Guid("9F4D95E4-8760-4C54-8FAC-BA64ACACC476"); }
        }

        public override bool DoesUserHaveViewAccess(IVRFileDoesUserHaveViewAccessContext context)
        {
            return true;
        }
    }
}
