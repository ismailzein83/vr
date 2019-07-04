using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldListDataRecordType : DataRecordFieldType
    {
        public override Guid ConfigId => new Guid("8C0574CD-8862-4F29-828A-CE398635F2F7");

        public override string RuntimeEditor => "vr-genericdata-fieldtype-datarecordtypelist-runtimeeditor";
        public Guid DataRecordTypeId { get; set; }
        public ListRecordRuntimeViewType RuntimeViewType { get; set; }
        public override RecordFilter ConvertToRecordFilter(IDataRecordFieldTypeConvertToRecordFilterContext context)
        {
            throw new NotImplementedException();
        }
        public override string RuntimeViewSettingEditor => "vr-genericdata-fieldtype-datarecordtypelist-settings";
        public override RDBDataRecordFieldAttribute GetDefaultRDBFieldAttribute(IDataRecordFieldTypeDefaultRDBFieldAttributeContext context)
        {
            return new RDBDataRecordFieldAttribute
            {
                RdbDataType = RDBDataType.NVarchar
            };
        }

        public override string GetDescription(object value)
        {
            return value == null ? null : $"{(value as dynamic).Count} Items";
        }

        public override GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", NumberPrecision = "NoDecimal", Field = context != null ? context.DescriptionFieldPath : null };
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
                        Type dataRecordType = new DataRecordTypeManager().GetDataRecordRuntimeType(DataRecordTypeId);
                        _nonNullableRuntimeType = typeof(List<>).MakeGenericType(dataRecordType);
                    }
                }
            }
            return _nonNullableRuntimeType;
        }
        public override void ApplyTranslation(IDataRecordFieldTypeTranslationContext context)
        {
            var runtimeViewType = context.FieldViewSettings as ListRecordRuntimeViewType;
            if (runtimeViewType != null)
            {
                runtimeViewType.ApplyTranslation(new ListRecordRuntimeViewTypeTranslationContext { DataRecordTypeId=DataRecordTypeId,LanguageId=context.LanguageId });
            }
        }
        public override Type GetRuntimeType()
        {
            return GetNonNullableRuntimeType();
        }

        public override string GetRuntimeTypeDescription()
        {
            return Vanrise.Common.CSharpCompiler.TypeToString(GetRuntimeType());
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            throw new NotImplementedException();
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            throw new NotImplementedException();
        }
        protected override dynamic ParseNonNullValueToFieldType(Object originalValue)
        {
            if (originalValue.GetType() != GetRuntimeType())
            {
                return Serializer.Deserialize(Serializer.Serialize(originalValue, true), GetRuntimeType());
            }
            else
            {
                return originalValue;
            }
        }

        public override bool StoreValueSerialized => true;
        public override string SerializeValue(ISerializeDataRecordFieldValueContext context)
        {
            context.ThrowIfNull("context");
            if (context.Object == null)
                return string.Empty;

            return Vanrise.Common.Serializer.Serialize(context.Object, true);
        }

        public override object DeserializeValue(IDeserializeDataRecordFieldValueContext context)
        {
            context.ThrowIfNull("context");
            if (string.IsNullOrEmpty(context.Value))
                return null;
            return Vanrise.Common.Serializer.Deserialize(context.Value, GetRuntimeType());
        }

        public override bool IsCompatibleWithFieldType(DataRecordFieldType fieldType)
        {
            FieldListDataRecordType fieldTypeAsListDataRecordType = fieldType as FieldListDataRecordType;
            if (fieldTypeAsListDataRecordType == null)
                return false;
            return fieldTypeAsListDataRecordType.DataRecordTypeId == this.DataRecordTypeId;
        }

    }

    public class GridViewListRecordRuntimeViewType : ListRecordRuntimeViewType
    {
        public override Guid ConfigId => new Guid("661E02F1-7A44-4D56-A73F-7912EF3017B1");
        public override string RuntimeEditor => "vr-genericdata-fieldtype-datarecordtypelist-gridview-runtime";
        public bool HideAddButton { get; set; }
        public bool HideSection { get; set; }
        public bool EnableDraggableRow { get; set; }
        public List<GridViewListAvailableFields> AvailableFields { get; set; }
    }
    public class GridViewListAvailableFields
    {
        public string FieldName { get; set; }
        public bool IsRequired { get; set; }
        public bool IsDisabled { get; set; }
        public bool ShowAsLabel { get; set; }
        public GridColumnSettings GridColumnSettings { get; set; }
    }
    public class GridEditorViewListRecordRuntimeViewType : ListRecordRuntimeViewType
    {
        public override Guid ConfigId => new Guid("03925E9D-6A0F-4D4F-A4A4-36F5757D71EB");
        public override string RuntimeEditor => "vr-genericdata-fieldtype-datarecordtypelist-grideditorview-runtime";
        public VRGenericEditorDefinitionSetting Settings { get; set; }
        public List<string> AvailableColumns { get; set; }
        public bool HideSection { get; set; }
        public override void ApplyTranslation(IListRecordRuntimeViewTypeTranslationContext context)
        {
            if (Settings != null)
                Settings.ApplyTranslation(new GenericEditorTranslationContext(context.DataRecordTypeId, context.LanguageId));
        }
        public override Dictionary<string, GridColumnAttribute> GetGenericEditorColumnsInfo(IListRecordRuntimeViewTypeColumnsInfoContext context)
        {
            var columnsInfo = Settings.GetGridColumnsAttributes(new GetGenericEditorColumnsInfoContext { DataRecordTypeId = context.DataRecordTypeId });
            if (columnsInfo == null || AvailableColumns == null || AvailableColumns.Count == 0)
                return columnsInfo;
            Dictionary<string, GridColumnAttribute> availableColumnsInfo = new Dictionary<string, GridColumnAttribute>();

            foreach (var columnInfo in columnsInfo)
            {
                if (AvailableColumns.Contains(columnInfo.Key))
                    availableColumnsInfo.Add(columnInfo.Key, columnInfo.Value);
            }
            return availableColumnsInfo;
        }
    }
    public class FieldViewListRecordRuntimeViewType : ListRecordRuntimeViewType
    {
        public override Guid ConfigId => new Guid("6A8E0D5E-318C-4C6E-A99B-991EAE562B7C");
        public override string RuntimeEditor => "vr-genericdata-fieldtype-datarecordtypelist-fieldview-runtime";
        public DataRecordField RecordField { get; set; }
    }
   
    public class ListRecordRuntimeViewTypeTranslationContext : IListRecordRuntimeViewTypeTranslationContext
    {
        public Guid LanguageId { get; set; }
        public Guid DataRecordTypeId { get; set; }
    }

    public class GridGenericEditorViewListRecordRuntimeViewType : ListRecordRuntimeViewType
    {
        public override Guid ConfigId => new Guid("CCB477B6-B20F-4FCE-A8AD-27BD12EE60ED");
        public override string RuntimeEditor => "vr-genericdata-fieldtype-datarecordtypelist-gridgenericeditorview-runtime";
        public bool HideAddButton { get; set; }
        public bool HideSection { get; set; }
        public bool EnableDraggableRow { get; set; }
        public List<GridGenericEditorField> Fields { get; set; }
        public GridGenericEditorDrillDownSettings DrillDownSettings { get; set; }
    }
    public class GridGenericEditorField
    {
        public Guid GridGenericEditorFieldId { get; set; }
        public string HeaderText { get; set; }
        public string Title { get; set; }
        public VRGenericEditorDefinitionSetting Settings { get; set; }
        public GridColumnSettings GridColumnSettings { get; set; }
    }
    public class GridGenericEditorDrillDownSettings
    {
        public  bool EnableDrillDown { get; set; }
        public bool AutoExpand { get; set; }
        public VRGenericEditorDefinitionSetting DrillDownSettings { get; set; }
    }
}

