using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldCustomObjectType : DataRecordFieldType
    {
        public override Guid ConfigId { get { return new Guid("28411D23-EA66-47AC-A323-106BE0B9DA7E"); } }

        public override string ViewerEditor { get { return "vr-genericdata-fieldtype-customobject-viewereditor"; } }

        public override RDBDataRecordFieldAttribute GetDefaultRDBFieldAttribute(IDataRecordFieldTypeDefaultRDBFieldAttributeContext context)
        {
            return new RDBDataRecordFieldAttribute
            {
                RdbDataType = RDBDataType.NVarchar
            };
        }

        public bool IsNullable { get; set; }

        public FieldCustomObjectTypeSettings Settings { get; set; }

        public override string RuntimeEditor { get { return null; } }

        public override bool StoreValueSerialized { get { return true; } }

        Type _runtimeType;
        public override Type GetRuntimeType()
        {
            if (_runtimeType == null)
            {
                var type = GetNonNullableRuntimeType();
                lock (this)
                {
                    if (_runtimeType == null)
                        _runtimeType = (IsNullable) ? GetNullableType(type) : type;
                }
            }
            return _runtimeType;
        }

        Type _nonNullableRuntimeType;
        public override Type GetNonNullableRuntimeType()
        {
            if (_nonNullableRuntimeType == null)
            {
                lock (this)
                {
                    if (_nonNullableRuntimeType == null)
                    {
                        this.Settings.ThrowIfNull("Settings");
                        _nonNullableRuntimeType = this.Settings.GetNonNullableRuntimeType();
                    }
                }

            }
            return _nonNullableRuntimeType;
        }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            if (this.Settings != null)
                return Settings.AreEqual(newValue, oldValue);
            return base.AreEqual(newValue, oldValue);
        }

        public override string GetDescription(object value)
        {
            if (this.Settings != null)
                return this.Settings.GetDescription(new FieldCustomObjectTypeSettingsContext
                {
                    FieldValue = value
                });
            return null;
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            throw new NotImplementedException();
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            if (fieldValue == null)
                return false;

            if (this.Settings != null)
            {
                return this.Settings.IsMatched(new FieldCustomObjectTypeSettingsContext
                {
                    FieldValue = fieldValue,
                    RecordFilter = recordFilter
                });
            }
            return false;
        }

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", NumberPrecision = "NoDecimal", Field = context != null ? context.DescriptionFieldPath : null };
        }

        public override RecordFilter ConvertToRecordFilter(IDataRecordFieldTypeConvertToRecordFilterContext context)
        {
            if (context.FilterValues == null || context.FilterValues.Count == 0)
                return null;
            var value = context.FilterValues[0];

            if (value != null)
            {
                var recordFilter= value.CastWithValidate<RecordFilter>("context.FilterValues[0]");
                recordFilter.FieldName = context.FieldName;
                return recordFilter;
            }
            else
            {
                return new EmptyRecordFilter { FieldName = context.FieldName };
            }
        }

        protected override dynamic ParseNonNullValueToFieldType(Object originalValue)
        {
            return this.Settings.ParseNonNullValueToFieldType(originalValue);
        }

        public override string GetRuntimeTypeDescription()
        {
            return Settings.GetRuntimeTypeDescription();
        }
    }

}