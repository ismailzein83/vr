using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas
{
    public class NullToBooleanFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("458EBD88-4537-4B57-B2AB-14AFAB2EF9B6"); } }

        public string NullableFieldName { get; set; }

        public bool NullIsFalse { get; set; }

        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { NullableFieldName };
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            dynamic nullableField = context.GetFieldValue(NullableFieldName);

            if (NullIsFalse)
                return nullableField == null ? false : true;
            else
                return nullableField == null ? true : false;
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            if (context.InitialFilter == null)
                throw new ArgumentNullException("context.InitialFilter");

            BooleanRecordFilter booleanRecordFilter = context.InitialFilter as BooleanRecordFilter;
            if (booleanRecordFilter != null)
            {
                if ((NullIsFalse && !booleanRecordFilter.IsTrue) || (!NullIsFalse && booleanRecordFilter.IsTrue))
                    return new EmptyRecordFilter() { FieldName = NullableFieldName };
                else
                    return new NonEmptyRecordFilter() { FieldName = NullableFieldName };
            }

            EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            if (emptyFilter != null)
                return new AlwaysFalseRecordFilter() { };

            NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
                return null;

            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }
    }
}
