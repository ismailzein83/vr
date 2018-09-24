using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;

namespace Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas
{
    public class SwitchCaseFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("93DD84F4-A750-4577-8173-36E7A653B920"); } }

        public string TargetFieldName { get; set; }

        public Dictionary<string, SwitchCaseMapping> SwitchCaseMappings { get; set; }


        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            if (SwitchCaseMappings == null || SwitchCaseMappings.Count == 0)
                throw new NullReferenceException("SwitchCaseMappings");

            List<string> dependentFields = new List<string>() { this.TargetFieldName };
            dependentFields.AddRange(SwitchCaseMappings.Values.Select(itm => itm.MappingFieldName));

            return dependentFields;
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            if (SwitchCaseMappings == null || SwitchCaseMappings.Count == 0)
                throw new NullReferenceException("SwitchCaseMappings");

            dynamic switchTargetFieldName = context.GetFieldValue(this.TargetFieldName);
            if (switchTargetFieldName == null)
                return null;

            SwitchCaseMapping switchCaseMapping;
            if (!this.SwitchCaseMappings.TryGetValue(Convert.ToString(switchTargetFieldName), out switchCaseMapping))
                return null;

            return context.GetFieldValue(switchCaseMapping.MappingFieldName);
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            if (SwitchCaseMappings == null || SwitchCaseMappings.Count == 0)
                throw new NullReferenceException("SwitchCaseMappings");

            RecordFilterGroup recordFilterGroup = new RecordFilterGroup();
            recordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.Or;
            recordFilterGroup.Filters = new List<RecordFilter>();

            DataRecordFieldType targetDataRecordFieldType;
            GetFieldType(context, this.TargetFieldName, out targetDataRecordFieldType);

            #region ObjectListRecordFilter

            ObjectListRecordFilter objectListFilter = context.InitialFilter as ObjectListRecordFilter;
            if (objectListFilter != null)
            {
                foreach (var kvp in SwitchCaseMappings)
                {
                    ObjectListRecordFilter targetFieldObjectListRecordFilter = BuildObjectListRecordFilter(ListRecordFilterOperator.In, new List<object>() { kvp.Key });
                    RecordFilter targetFieldRecordFilter = ConvertToRecordFilter(this.TargetFieldName, targetDataRecordFieldType, targetFieldObjectListRecordFilter);

                    DataRecordFieldType mappedFieldBEFieldType;
                    GetFieldType(context, kvp.Value.MappingFieldName, out mappedFieldBEFieldType);
                    ObjectListRecordFilter mappedFieldObjectListRecordFilter = BuildObjectListRecordFilter(objectListFilter.CompareOperator, objectListFilter.Values);
                    RecordFilter mappedFieldRecordFilter = ConvertToRecordFilter(kvp.Value.MappingFieldName, mappedFieldBEFieldType, mappedFieldObjectListRecordFilter);

                    RecordFilterGroup currentRecordFilterGroup = new RecordFilterGroup();
                    currentRecordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.And;
                    currentRecordFilterGroup.Filters = new List<RecordFilter>() { targetFieldRecordFilter, mappedFieldRecordFilter };

                    recordFilterGroup.Filters.Add(currentRecordFilterGroup);
                }

                return recordFilterGroup;
            }

            #endregion

            #region StringRecordFilter

            StringRecordFilter stringRecordFilter = context.InitialFilter as StringRecordFilter;
            if (stringRecordFilter != null)
            {
                foreach (var kvp in SwitchCaseMappings)
                {
                    ObjectListRecordFilter targetFieldObjectListRecordFilter = BuildObjectListRecordFilter(ListRecordFilterOperator.In, new List<object>() { kvp.Key });
                    RecordFilter targetFieldRecordFilter = ConvertToRecordFilter(this.TargetFieldName, targetDataRecordFieldType, targetFieldObjectListRecordFilter);

                    DataRecordFieldType mappedFieldBEFieldType;
                    GetFieldType(context, kvp.Value.MappingFieldName, out mappedFieldBEFieldType);
                    StringRecordFilter mappedFieldStringRecordFilter = BuildStringRecordFilter(stringRecordFilter.CompareOperator, stringRecordFilter.Value);
                    RecordFilter mappedFieldRecordFilter = ConvertToRecordFilter(kvp.Value.MappingFieldName, mappedFieldBEFieldType, mappedFieldStringRecordFilter);

                    RecordFilterGroup currentRecordFilterGroup = new RecordFilterGroup();
                    currentRecordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.And;
                    currentRecordFilterGroup.Filters = new List<RecordFilter>() { targetFieldRecordFilter, mappedFieldRecordFilter };

                    recordFilterGroup.Filters.Add(currentRecordFilterGroup);
                }

                return recordFilterGroup;
            }

            #endregion

            #region EmptyRecordFilter

            EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            if (emptyFilter != null)
            {
                foreach (var kvp in SwitchCaseMappings)
                {
                    ObjectListRecordFilter targetFieldObjectListRecordFilter = BuildObjectListRecordFilter(ListRecordFilterOperator.In, new List<object>() { kvp.Key });
                    RecordFilter targetFieldRecordFilter = ConvertToRecordFilter(this.TargetFieldName, targetDataRecordFieldType, targetFieldObjectListRecordFilter);

                    EmptyRecordFilter mappedFieldEmptyRecordFilter = new EmptyRecordFilter { FieldName = kvp.Value.MappingFieldName };

                    RecordFilterGroup currentRecordFilterGroup = new RecordFilterGroup();
                    currentRecordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.And;
                    currentRecordFilterGroup.Filters = new List<RecordFilter>() { targetFieldRecordFilter, mappedFieldEmptyRecordFilter };

                    recordFilterGroup.Filters.Add(currentRecordFilterGroup);
                }

                return recordFilterGroup;
            }

            #endregion

            #region NonEmptyRecordFilter

            NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
            {
                foreach (var kvp in SwitchCaseMappings)
                {
                    ObjectListRecordFilter targetFieldObjectListRecordFilter = BuildObjectListRecordFilter(ListRecordFilterOperator.In, new List<object>() { kvp.Key });
                    RecordFilter targetFieldRecordFilter = ConvertToRecordFilter(this.TargetFieldName, targetDataRecordFieldType, targetFieldObjectListRecordFilter);

                    NonEmptyRecordFilter mappedFieldNonEmptyRecordFilter = new NonEmptyRecordFilter { FieldName = kvp.Value.MappingFieldName };

                    RecordFilterGroup currentRecordFilterGroup = new RecordFilterGroup();
                    currentRecordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.And;
                    currentRecordFilterGroup.Filters = new List<RecordFilter>() { targetFieldRecordFilter, mappedFieldNonEmptyRecordFilter };

                    recordFilterGroup.Filters.Add(currentRecordFilterGroup);
                }

                return recordFilterGroup;
            }

            #endregion

            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }


        private void GetFieldType(IDataRecordFieldFormulaConvertFilterContext context, string fieldName, out DataRecordFieldType dataRecordFieldType)
        {
            dataRecordFieldType = context.GetFieldType(fieldName) as DataRecordFieldType;
            if (dataRecordFieldType == null)
                throw new NullReferenceException(String.Format("dataRecordFieldType '{0}'", fieldName));
        }

        private ObjectListRecordFilter BuildObjectListRecordFilter(ListRecordFilterOperator listRecordFilterOperator, List<object> values)
        {
            return new ObjectListRecordFilter()
            {
                CompareOperator = listRecordFilterOperator,
                Values = values
            };
        }

        private StringRecordFilter BuildStringRecordFilter(StringRecordFilterOperator stringRecordFilterOperator, string value)
        {
            return new StringRecordFilter()
            {
                CompareOperator = stringRecordFilterOperator,
                Value = value
            };
        }

        private RecordFilter ConvertToRecordFilter(string fieldName, DataRecordFieldType dataRecordFieldType, ObjectListRecordFilter objectListRecordFilter)
        {
            var recordFilter = dataRecordFieldType.ConvertToRecordFilter(new DataRecordFieldTypeConvertToRecordFilterContext { FieldName = fieldName, FilterValues = objectListRecordFilter.Values ,StrictEqual = true});
            if (recordFilter is ObjectListRecordFilter)
                ((ObjectListRecordFilter)recordFilter).CompareOperator = objectListRecordFilter.CompareOperator;
            return recordFilter;
        }

        private RecordFilter ConvertToRecordFilter(string fieldName, DataRecordFieldType dataRecordFieldType, StringRecordFilter stringRecordFilter)
        {
            var recordFilter = dataRecordFieldType.ConvertToRecordFilter(new DataRecordFieldTypeConvertToRecordFilterContext { FieldName = fieldName, FilterValues = new List<object>() { stringRecordFilter.Value } ,StrictEqual =true});
            if (recordFilter is StringRecordFilter)
                ((StringRecordFilter)recordFilter).CompareOperator = stringRecordFilter.CompareOperator;
            return recordFilter;
        }
    }

    public class SwitchCaseMapping
    {
        public string MappingFieldName { get; set; }
    }
}
