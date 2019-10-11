using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.CustomObject
{
    public class RecordFilterCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("628115B1-E669-454F-B581-010A02C4E765"); } }

        public Guid DataRecordTypeID { get; set; }

        public override string SelectorUIControl { get { return "vr-genericdata-customobject-recordfilter-runtime"; } }

        public override bool AreEqual(object newValue, object oldValue)
        {
            string newRecordFilterGroupExpression = null;
            if (newValue != null)
                newRecordFilterGroupExpression = BuildRecordFilterGroupExpression(newValue as RecordFilterGroup);

            string oldRecordFilterGroupExpression = null;
            if (oldValue != null)
                oldRecordFilterGroupExpression = BuildRecordFilterGroupExpression(oldValue as RecordFilterGroup);

            return newRecordFilterGroupExpression == oldRecordFilterGroupExpression;
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var recordFilterGroup = context.FieldValue as RecordFilterGroup;
            if (recordFilterGroup == null)
                return null;

            return BuildRecordFilterGroupExpression(recordFilterGroup);
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(RecordFilterGroup);
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Record Filter Custom Object";
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as RecordFilterGroup;
        }

        private string BuildRecordFilterGroupExpression(RecordFilterGroup recordFilterGroup)
        {
            Dictionary<string, RecordFilterFieldInfo> recordFilterFieldsInfo = null;

            var dataRecordTypeFields = new DataRecordTypeManager().GetDataRecordTypeFields(this.DataRecordTypeID);
            if (dataRecordTypeFields != null)
            {
                recordFilterFieldsInfo = dataRecordTypeFields.ToDictionary(x => x.Value.Name, x => new RecordFilterFieldInfo()
                {
                    Name = x.Value.Name,
                    Title = x.Value.Title,
                    Type = x.Value.Type
                });
            }

            return new RecordFilterManager().BuildRecordFilterGroupExpression(recordFilterGroup, recordFilterFieldsInfo);
        }
    }
}