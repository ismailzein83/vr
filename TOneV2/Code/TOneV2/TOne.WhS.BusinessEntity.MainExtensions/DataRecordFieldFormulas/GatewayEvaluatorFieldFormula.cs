using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.DataRecordFieldFormulas
{
    public class GatewayEvaluatorFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("395B97B4-E304-4040-A53D-16A145B4C42E"); } }
        public string PortFieldFormula { get; set; }

        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { PortFieldFormula };
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            dynamic port = context.GetFieldValue(this.PortFieldFormula);
            if (!String.IsNullOrEmpty(port))
            {
                var switchConnectivity = (new TOne.WhS.BusinessEntity.Business.SwitchConnectivityManager()).GetMatchConnectivity(port);
                if (switchConnectivity != null)
                    return switchConnectivity.SwitchConnectivityId;
            }
            return null;
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            //if (context.InitialFilter == null)
            //    throw new ArgumentNullException("context.InitialFilter");

            //ObjectListRecordFilter objectListFilter = context.InitialFilter as ObjectListRecordFilter;
            //if (objectListFilter != null)
            //{
            //    DataRecordFields.FieldBusinessEntityType currentBEFieldType;
            //    DataRecordFields.FieldBusinessEntityType childBEFieldType;
            //    GetFieldTypes(context, out currentBEFieldType, out childBEFieldType);

            //    BusinessEntityManager beManager = new BusinessEntityManager();
            //    IEnumerable<dynamic> childValues = beManager.GetChildEntitiesIds(currentBEFieldType.BusinessEntityDefinitionId, childBEFieldType.BusinessEntityDefinitionId, objectListFilter.Values.Cast<dynamic>());

            //    var childFilter = childBEFieldType.ConvertToRecordFilter(this.ChildFieldName, childValues.Cast<Object>().ToList());
            //    if (childFilter is ObjectListRecordFilter)
            //        ((ObjectListRecordFilter)childFilter).CompareOperator = objectListFilter.CompareOperator;
            //    return childFilter;
            //}

            //EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            //if (emptyFilter != null)
            //    return new EmptyRecordFilter { FieldName = this.ChildFieldName };

            //NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            //if (nonEmptyFilter != null)
            //    return new NonEmptyRecordFilter { FieldName = this.ChildFieldName };

            //throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));

            return null;
        }
    }
}
