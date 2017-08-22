using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas
{
    public class NullToBooleanFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("458EBD88-4537-4B57-B2AB-14AFAB2EF9B6"); } }

        public string NullableFieldFieldName { get; set; }

        public bool NullIsFalse { get; set; }

        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { NullableFieldFieldName };
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            dynamic nullableField = context.GetFieldValue(NullableFieldFieldName);

            if (NullIsFalse)
                return nullableField == null ? false : true;
            else
                return nullableField == null ? true : false;
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            if (context.InitialFilter == null)
                throw new ArgumentNullException("context.InitialFilter");

            ObjectListRecordFilter objectListFilter = context.InitialFilter as ObjectListRecordFilter;
            if (objectListFilter != null)
            {
                bool booleanFieldValue = Convert.ToBoolean(objectListFilter.Values.First());

                if((NullIsFalse && !booleanFieldValue) || (!NullIsFalse && booleanFieldValue))
                    return new EmptyRecordFilter() { FieldName = NullableFieldFieldName };
                else
                    return new NonEmptyRecordFilter() { FieldName = NullableFieldFieldName }; 
            }

            //EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            //if (emptyFilter != null)
            //    return new AlwaysFalseRecordFilter() { };

            NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
                return null;

            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }
    }
}
