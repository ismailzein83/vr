using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.MainExtensions.DataRecordTypeExtraFields
{
    class MultipleParentDataRecordTypeExtraFields : DataRecordTypeExtraField
    {
        public override Guid ConfigId { get { return new Guid("466EECEE-F3AC-4880-8818-8FEDD8D92BA8"); } }

        public List<DataRecordTypeExtraField> DataRecordTypeExtraFields { get; set; }

        public override List<DataRecordField> GetFields(IDataRecordExtraFieldContext context)
        {
            DataRecordTypeExtraFields.ThrowIfNull("DataRecordTypeExtraFields");

            var dataRecordFields = new List<DataRecordField>();
            List<string> fieldNames = new List<string>();

            foreach (var dataRecordTypeExtraField in this.DataRecordTypeExtraFields)
            {
                var dataRecordTypeExtraFields = dataRecordTypeExtraField.GetFields(context);
                foreach (var field in dataRecordTypeExtraFields)
                {
                    if (fieldNames.Contains(field.Name))
                        continue;

                    dataRecordFields.Add(field);
                    fieldNames.Add(field.Name);
                }
            }

            return dataRecordFields;
        }
    }
}