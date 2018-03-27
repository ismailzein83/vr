using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldCustomObjectType : DataRecordFieldType
    {
        public override Guid ConfigId
        {
            get { return new Guid("28411D23-EA66-47AC-A323-106BE0B9DA7E"); }
        }
        public bool IsNullable { get; set; }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            if (this.Settings != null)
                return Settings.AreEqual(newValue, oldValue);
            return base.AreEqual(newValue, oldValue);
        }

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

        public override bool StoreValueSerialized { get { return true; } }

        public override string GetDescription(object value)
        {
            if (this.Settings != null)
                return this.Settings.GetDescription(new FieldCustomObjectTypeSettingsContext
                {
                    FieldValue = value
                });
            return null;
        }
        public override string ViewerEditor { get { return "vr-genericdata-fieldtype-customobject-viewereditor"; } }
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

        public FieldCustomObjectTypeSettings Settings { get; set; }

        protected override dynamic ParseNonNullValueToFieldType(Object originalValue)
        {
            return this.Settings.ParseNonNullValueToFieldType(originalValue);
        }
    }
  
    
}
