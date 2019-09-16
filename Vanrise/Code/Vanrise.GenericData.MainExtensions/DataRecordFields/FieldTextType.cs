using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public enum TextType { RichText = 0, MultipleText = 1, Email = 2, LabeledEmail = 3, IPv4 = 4, IPv6 = 5, FileName = 6, Password = 7 }

    public class FieldTextType : DataRecordFieldType
    {

        RecordFilterManager _recordFilterManager = new RecordFilterManager();
        public override Guid ConfigId { get { return new Guid("3f29315e-b542-43b8-9618-7de216cd9653"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-fieldtype-text-runtimeeditor"; } }

        public override string ViewerEditor { get { return "vr-genericdata-fieldtype-text-viewereditor"; } }

        public override DataRecordFieldOrderType OrderType { get { return DataRecordFieldOrderType.ByFieldDescription; } }

        public override Type GetRuntimeType() { return GetNonNullableRuntimeType(); }

        public string Hint { get; set; }
        public TextType? TextType { get; set; }


        public override RDBDataRecordFieldAttribute GetDefaultRDBFieldAttribute(IDataRecordFieldTypeDefaultRDBFieldAttributeContext context)
        {
            return new RDBDataRecordFieldAttribute
            {
                RdbDataType = RDBDataType.NVarchar,
                Size = 255
            };
        }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            if (newValue == null && oldValue == null)
                return true;

            if (newValue == null || oldValue == null)
                return false;

            string newLongValue = (string)newValue;
            string oldLongValue = (string)oldValue;
            return newLongValue.Equals(oldLongValue);
        }

        Type _nonNullableRuntimeType = typeof(string);
        public override Type GetNonNullableRuntimeType()
        {
            return _nonNullableRuntimeType;
        }
        public override string GenerateValueCode(object value)
        {
            if (value == null)
                return "null";
            return string.Concat('"', value.ToString(), '"');
        }
        public override string GetDescription(Object value)
        {
            if (value == null)
                return null;

            IEnumerable<string> textValues = FieldTypeHelper.ConvertFieldValueToList<string>(value);

            if (textValues == null)
                return value.ToString();

            var descriptions = new List<string>();

            foreach (var textValue in textValues)
            {
                descriptions.Add(textValue.ToString());
            }

            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            if (fieldValue != null && filterValue != null)
            {
                var fieldValueObjList = fieldValue as List<object>;
                fieldValueObjList = (fieldValueObjList == null) ? new List<object>() { fieldValue } : fieldValueObjList;

                var fieldValueStringList = fieldValueObjList.MapRecords(itm => Convert.ToString(itm).ToUpper());
                var filterValueString = filterValue.ToString().ToUpper();

                foreach (var fieldValueStringItem in fieldValueStringList)
                {
                    if (fieldValueStringItem.Contains(filterValueString))
                        return true;
                }

                return false;
            }

            return true;
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            if (fieldValue == null)
                return false;

            StringRecordFilter stringRecordFilter = recordFilter as StringRecordFilter;
            if (stringRecordFilter == null)
                throw new NullReferenceException("stringRecordFilter");

            string valueAsString = fieldValue as string;
            if (valueAsString == null)
                throw new NullReferenceException("valueAsString");

            return _recordFilterManager.IsFieldValueMatched(valueAsString, stringRecordFilter);
        }

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", NumberPrecision = "NoDecimal", Field = context != null ? context.DescriptionFieldPath : null };
        }

        public override RecordFilter ConvertToRecordFilter(IDataRecordFieldTypeConvertToRecordFilterContext context)
        {
            if (context.FilterValues == null || context.FilterValues.Count == 0)
                return null;

            var values = context.FilterValues.Select(x => x.ToString()).ToList();
            List<RecordFilter> recordFilters = new List<RecordFilter>();

            foreach (var value in values)
            {
                recordFilters.Add(new StringRecordFilter
                {
                    CompareOperator = context.StrictEqual ? StringRecordFilterOperator.Equals : StringRecordFilterOperator.Contains,
                    Value = value,
                    FieldName = context.FieldName
                });
            }
            return recordFilters.Count > 1 ? new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.Or, Filters = recordFilters } : recordFilters.First();
        }

        protected override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue;
        }
        public override void SetExcelCellType(IDataRecordFieldTypeSetExcelCellTypeContext context)
        {
            context.HeaderCell.ThrowIfNull("context.HeaderCell");
            var headerCell = context.HeaderCell;
            headerCell.CellType = ExcelCellType.Text;
        }

        public override void GetValueByDescription(IGetValueByDescriptionContext context)
        {
            if (context.FieldDescription == null)
                return;

            context.FieldValue = context.FieldDescription.ToString().Trim();
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Text";
        }

        public override bool IsCompatibleWithFieldType(DataRecordFieldType fieldType)
        {
            FieldTextType fieldTypeAsTextType = fieldType as FieldTextType;
            return fieldTypeAsTextType != null;
        }
    }
}