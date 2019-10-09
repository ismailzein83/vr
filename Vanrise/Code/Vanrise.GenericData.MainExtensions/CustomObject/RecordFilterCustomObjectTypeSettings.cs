using System;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.CustomObject
{
    public class RecordFilterCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("628115B1-E669-454F-B581-010A02C4E765"); } }

        public Guid DataRecordTypeID { get; set; }

        public override bool AreEqual(object newValue, object oldValue)
        {
            throw new NotImplementedException();
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            throw new NotImplementedException();
        }

        public override Type GetNonNullableRuntimeType()
        {
            throw new NotImplementedException();
        }

        public override string GetRuntimeTypeDescription()
        {
            throw new NotImplementedException();
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            throw new NotImplementedException();
        }
    }
}