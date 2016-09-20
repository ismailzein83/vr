using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas
{
    public class ParentBusinessEntityFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("fc4b69f0-d577-4319-8d10-ed8f95e07441"); } }

        public string ChildFieldName { get; set; }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            DataRecordFields.FieldBusinessEntityType currentBEFieldType;
            DataRecordFields.FieldBusinessEntityType childBEFieldType;
            GetFieldTypes(context, out currentBEFieldType, out childBEFieldType);
            BusinessEntityManager beManager = new BusinessEntityManager();
            return beManager.GetParentEntityId(childBEFieldType.BusinessEntityDefinitionId, currentBEFieldType.BusinessEntityDefinitionId, context.GetFieldValue(this.ChildFieldName));
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            if (context.InitialFilter == null)
                throw new ArgumentNullException("context.InitialFilter");

            NumberListRecordFilter numberListFilter = context.InitialFilter as NumberListRecordFilter;
            if (numberListFilter != null)
            {
                DataRecordFields.FieldBusinessEntityType currentBEFieldType;
                DataRecordFields.FieldBusinessEntityType childBEFieldType;
                GetFieldTypes(context, out currentBEFieldType, out childBEFieldType);
                
                BusinessEntityManager beManager = new BusinessEntityManager();
                IEnumerable<dynamic> childValues = beManager.GetChildEntitiesIds(currentBEFieldType.BusinessEntityDefinitionId, childBEFieldType.BusinessEntityDefinitionId, numberListFilter.Values.Cast<dynamic>());

                var childFilter = childBEFieldType.ConvertToRecordFilter(this.ChildFieldName, childValues.Cast<Object>().ToList());
                if (childFilter is NumberListRecordFilter)
                    ((NumberListRecordFilter)childFilter).CompareOperator = numberListFilter.CompareOperator;
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
