﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Entities
{
    public enum DataRecordFieldOrderType { ByFieldValue = 0, ByFieldDescription = 1 }
    public abstract class DataRecordFieldType
    {
        public abstract Guid ConfigId { get; }

        public abstract string RuntimeEditor { get; }

        public virtual string RuntimeViewSettingEditor { get; }

        public virtual string DifferenceEditor { get; set; }

        public virtual string ViewerEditor { get; set; }

        public string GetValueMethod { get; set; }

        public string ConvertFilterMethod { get; set; }

        public virtual string DetailViewerEditor { get { return "vr-genericdata-datarecordfield-defaultdetailviewer"; } }

        public abstract Type GetRuntimeType();

        public abstract Type GetNonNullableRuntimeType();

        public abstract string GetRuntimeTypeDescription();

        public virtual bool AreEqual(Object newValue, Object oldValue)
        {
            return newValue == oldValue;
        }

        public virtual bool TryResolveDifferences(IDataRecordFieldTypeTryResolveDifferencesContext context)
        {
            return false;
        }

        public virtual void GetValueByDescription(IGetValueByDescriptionContext context)
        {
        }

        public virtual void onBeforeSave(IDataRecordFieldTypeOnBeforeSaveContext context)
        {

        }

        public virtual void onAfterSave(IDataRecordFieldTypeOnAfterSaveContext context)
        {

        }

        public virtual string ConvertFlatFileValue(IDataRecordFieldTypeConvertFlatFileValueContext context)
        {
            return GetDescription(context.FieldValue);
        }

        public abstract RDBDataRecordFieldAttribute GetDefaultRDBFieldAttribute(IDataRecordFieldTypeDefaultRDBFieldAttributeContext context);

        public abstract string GetDescription(Object value);

        public abstract bool IsMatched(Object fieldValue, Object filterValue);

        public abstract bool IsMatched(Object fieldValue, RecordFilter recordFilter);

        public abstract GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context);

        public abstract RecordFilter ConvertToRecordFilter(IDataRecordFieldTypeConvertToRecordFilterContext context);

        public virtual bool TryGenerateUniqueIdentifier(out Guid? uniqueIdentifier)
        {
            uniqueIdentifier = null;
            return false;
        }

        public virtual void SetExcelCellType(IDataRecordFieldTypeSetExcelCellTypeContext context)
        {

        }

        protected Type GetNullableType(Type type)
        {
            return (type.IsValueType) ? typeof(Nullable<>).MakeGenericType(type) : type;
        }

        public virtual DataRecordFieldOrderType OrderType
        {
            get
            {
                return DataRecordFieldOrderType.ByFieldValue;
            }
        }

        public virtual bool StoreValueSerialized { get { return false; } }
        public virtual string SerializeValue(ISerializeDataRecordFieldValueContext context)
        {
            context.ThrowIfNull("context");
            if (context.Object == null)
                return string.Empty;

            return Vanrise.Common.Serializer.Serialize(context.Object);
        }
        public virtual Object DeserializeValue(IDeserializeDataRecordFieldValueContext context)
        {
            context.ThrowIfNull("context");
            if (string.IsNullOrEmpty(context.Value))
                return null;

            return Vanrise.Common.Serializer.Deserialize(context.Value);
        }

        public virtual bool CanRoundValue { get { return false; } }
        public virtual dynamic GetRoundedValue(dynamic value)
        {
            throw new NotImplementedException();
        }

        public dynamic ParseValueToFieldType(IDataRecordFieldTypeParseValueToFieldTypeContext context)
        {
            var originalValue = context.Value;
            if (originalValue == null)
            {
                var fieldRuntimeType = GetRuntimeType();

                if (fieldRuntimeType.IsValueType)
                    return Activator.CreateInstance(fieldRuntimeType);
                else
                    return null;
            }
            else
            {
                return ParseNonNullValueToFieldType(originalValue);
            }
        }

        public virtual bool RenderDescriptionByDefault()
        {
            return false;
        }

        protected virtual dynamic ParseNonNullValueToFieldType(Object originalValue)
        {
            return Convert.ChangeType(originalValue, GetNonNullableRuntimeType());
        }

        public virtual bool IsStillAvailable(IDataRecordFieldTypeIsStillAvailableContext context)
        {
            return true;
        }

        public virtual bool IsCompatibleWithFieldType(DataRecordFieldType fieldType)
        {
            return false;
        }

        public virtual void SetDependentFieldValues(ISetDependentFieldValuesContext context)
        {
        }
        public virtual bool TryGetStyleDefinitionId(IDataRecordFieldStyleDefinitionContext context) {
            return false;
        }
    }
    public interface IDataRecordFieldStyleDefinitionContext
    {
        object FieldValue { get; }
        Guid StyleDefinitionId { set; }
    }
    public  class DataRecordFieldStyleDefinitionContext : IDataRecordFieldStyleDefinitionContext
    {
        public object FieldValue { get; set; }
        public Guid StyleDefinitionId { get; set; }

    }
    public interface IDataRecordFieldTypeDefaultRDBFieldAttributeContext
    {

    }

    public class DataRecordFieldTypeDefaultRDBFieldAttributeContext : IDataRecordFieldTypeDefaultRDBFieldAttributeContext
    {

    }

    public interface IDataRecordFieldTypeConvertToRecordFilterContext
    {
        string FieldName { get; }
        List<Object> FilterValues { get; }
        bool StrictEqual { get; }
    }

    public class DataRecordFieldTypeConvertToRecordFilterContext : IDataRecordFieldTypeConvertToRecordFilterContext
    {
        public string FieldName { get; set; }
        public List<object> FilterValues { get; set; }
        public bool StrictEqual { get; set; }
    }

    public interface IDataRecordFieldTypeParseValueToFieldTypeContext
    {
        Object Value { get; }
    }

    public interface ISerializeDataRecordFieldValueContext
    {
        Object Object { get; }
    }

    public class SerializeDataRecordFieldValueContext : ISerializeDataRecordFieldValueContext
    {
        public Object Object { get; set; }
    }

    public interface IDeserializeDataRecordFieldValueContext
    {
        string Value { get; }
    }

    public class DeserializeDataRecordFieldValueContext : IDeserializeDataRecordFieldValueContext
    {
        public string Value { get; set; }
    }

    public interface IDataRecordFieldTypeSetExcelCellTypeContext
    {
        ExportExcelHeaderCell HeaderCell { get; }
    }

    public interface IDataRecordFieldEvaluator
    {
        dynamic GetFieldValue(string fieldName, IDataRecordFieldGetValueContext context);

        RecordFilter ConvertRecordFilter(string fieldName, RecordFilter recordFilter, IDataRecordFieldConvertFilterContext context);
    }

    public interface IDataRecordFieldGetValueContext
    {
        dynamic GetFieldValue(string fieldName);
    }

    public interface IDataRecordFieldConvertFilterContext
    {

    }

    public class FieldTypeGetGridColumnAttributeContext
    {
        public string ValueFieldPath { get; set; }
        public string DescriptionFieldPath { get; set; }
    }

    public interface IDataRecordFieldTypeTryResolveDifferencesContext
    {
        Object OldValue { get; }
        Object NewValue { get; }
        Object Changes { set; }
    }

    public interface IGetValueByDescriptionContext
    {
        Object FieldDescription { get; }

        List<AdditionalField> AdditionalFields { get; }

        string ErrorMessage { set; }

        Object FieldValue { set; }

        DataRecordFieldType FieldType { get; set; }
    }

    public class DataRecordFieldTypeIsStillAvailableContext : IDataRecordFieldTypeIsStillAvailableContext
    {
        public dynamic EntityId { get; set; }
    }

    public interface IDataRecordFieldTypeIsStillAvailableContext
    {
        dynamic EntityId { get; }
    }

    public interface IDataRecordFieldTypeConvertFlatFileValueContext
    {
        object FieldValue { get; }
    }

    public interface ISetDependentFieldValuesContext
    {
        string FieldName { get; }

        object FieldValue { get; }

        Guid DataRecordTypeId { get; }

        Dictionary<string, DataRecordField> DataRecordFields { get; }

        Dictionary<string, object> DependentFieldValues { get; }
    }

    public class SetDependentFieldValuesContext : ISetDependentFieldValuesContext
    {
        public string FieldName { get; set; }

        public object FieldValue { get; set; }

        public Guid DataRecordTypeId { get; set; }

        public Dictionary<string, DataRecordField> DataRecordFields { get; set; }

        public Dictionary<string, object> DependentFieldValues { get; set; }
    }
}