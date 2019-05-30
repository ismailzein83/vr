using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldTextResourceType : DataRecordFieldType
    {
        public override Guid ConfigId { get { return new Guid("DA84E13B-6AAC-49EE-AA09-C254A901FB4A"); } }
        public override RDBDataRecordFieldAttribute GetDefaultRDBFieldAttribute(IDataRecordFieldTypeDefaultRDBFieldAttributeContext context)
        {
            return new RDBDataRecordFieldAttribute
            {
                RdbDataType = RDBDataType.NVarchar
            };
        }
        public override Type GetRuntimeType()
        {
            return GetNonNullableRuntimeType();
        }
        public override bool TryResolveDifferences(IDataRecordFieldTypeTryResolveDifferencesContext context)
        {
            return true;
        }
        public override bool AreEqual(Object newValue, Object oldValue)
        {
            if (newValue == null && oldValue == null)
                return true;

            if (newValue == null || oldValue == null)
                return false;

            var newValueObject = newValue as TextResourceFieldTypeEntity;
            var oldValueObject = oldValue as TextResourceFieldTypeEntity;

            if (newValueObject != null && newValueObject.TranslatedValues != null && oldValueObject != null && oldValueObject.TranslatedValues != null)
            {
                foreach (var newAttachmentField in newValueObject.TranslatedValues)
                {
                    var objectEntity = oldValueObject.TranslatedValues.GetRecord(newAttachmentField.Key);
                    if (objectEntity == null || objectEntity != newAttachmentField.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        Type _nonNullableRuntimeType = typeof(TextResourceFieldTypeEntity);
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

        public override RecordFilter ConvertToRecordFilter(IDataRecordFieldTypeConvertToRecordFilterContext context)
        {
            throw new NotImplementedException();
        }

        public override string RuntimeEditor
        {
            get { return null; }
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Text Resource";
        }
        protected override dynamic ParseNonNullValueToFieldType(Object originalValue)
        {
            return originalValue;
        }
        public override bool IsCompatibleWithFieldType(DataRecordFieldType fieldType)
        {
            FieldTextResourceType fieldTextResourceType = fieldType as FieldTextResourceType;
            return fieldTextResourceType != null;
        }
    }
}
