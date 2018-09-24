using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas
{
    public class ParentBusinessEntityFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("fc4b69f0-d577-4319-8d10-ed8f95e07441"); } }

        public string ChildFieldName { get; set; }

        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { ChildFieldName };
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            DataRecordFields.FieldBusinessEntityType currentBEFieldType;
            DataRecordFields.FieldBusinessEntityType childBEFieldType;
            GetFieldTypes(context, out currentBEFieldType, out childBEFieldType);
            
            dynamic entityId = context.GetFieldValue(this.ChildFieldName);
            if (entityId == null)
                return null;

            return new BusinessEntityManager().GetParentEntityId(childBEFieldType.BusinessEntityDefinitionId, currentBEFieldType.BusinessEntityDefinitionId, entityId);
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            if (context.InitialFilter == null)
                throw new ArgumentNullException("context.InitialFilter");

            ObjectListRecordFilter objectListFilter = context.InitialFilter as ObjectListRecordFilter;
            if (objectListFilter != null)
            {
                DataRecordFields.FieldBusinessEntityType currentBEFieldType;
                DataRecordFields.FieldBusinessEntityType childBEFieldType;
                GetFieldTypes(context, out currentBEFieldType, out childBEFieldType);

                BusinessEntityManager beManager = new BusinessEntityManager();
                IEnumerable<dynamic> childValues = beManager.GetChildEntitiesIds(currentBEFieldType.BusinessEntityDefinitionId, childBEFieldType.BusinessEntityDefinitionId, objectListFilter.Values.Cast<dynamic>());

                var childFilter = childBEFieldType.ConvertToRecordFilter(new DataRecordFieldTypeConvertToRecordFilterContext { FieldName = this.ChildFieldName, FilterValues = childValues.Cast<Object>().ToList() });
                if (childFilter is ObjectListRecordFilter)
                    ((ObjectListRecordFilter)childFilter).CompareOperator = objectListFilter.CompareOperator;
                return childFilter;
            }

            EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            if (emptyFilter != null)
                return new EmptyRecordFilter { FieldName = this.ChildFieldName };

            NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
                return new NonEmptyRecordFilter { FieldName = this.ChildFieldName };

            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }

        private void GetFieldTypes(IDataRecordFieldFormulaContext context, out DataRecordFields.FieldBusinessEntityType currentBEFieldType, out DataRecordFields.FieldBusinessEntityType childBEFieldType)
        {
            currentBEFieldType = context.FieldType as DataRecordFields.FieldBusinessEntityType;
            if (currentBEFieldType == null)
                throw new NullReferenceException("currentBEFieldType");
            childBEFieldType = context.GetFieldType(this.ChildFieldName) as DataRecordFields.FieldBusinessEntityType;
            if (childBEFieldType == null)
                throw new NullReferenceException(String.Format("childBRFieldType '{0}'", this.ChildFieldName));
        }
    }
}
