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

        public abstract Type GetRuntimeType();

        public abstract Type GetNonNullableRuntimeType();

        public abstract string GetDescription(Object value);

        public abstract bool IsMatched(Object fieldValue, Object filterValue);

        public abstract bool IsMatched(Object fieldValue, RecordFilter recordFilter);

        public abstract GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context);

        public abstract RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues);

        public virtual void SetExcelCellType(IDataRecordFieldTypeSetExcelCellTypeContext context)
        {

        }

        public virtual string DetailViewerEditor { get { return "vr-genericdata-datarecordfield-defaultdetailviewer"; } }

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

        public string GetValueMethod { get; set; }

        public string ConvertFilterMethod { get; set; }

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
}
