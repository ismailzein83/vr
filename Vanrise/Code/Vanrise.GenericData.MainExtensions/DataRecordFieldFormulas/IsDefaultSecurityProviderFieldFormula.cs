using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;

namespace Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas
{
    public class IsDefaultSecurityProviderFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("ECBC9B85-98F1-4828-8FA3-161B9E639F1C"); } }

        public string FieldName { get; set; }

        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { FieldName };
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            Guid defaultSecurityProviderId = new ConfigManager().GetDefaultSecurityProviderId();
            var securityProviderId = context.GetFieldValue(FieldName);
            return securityProviderId == defaultSecurityProviderId;
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            if (context.InitialFilter == null)
                throw new ArgumentNullException("context.InitialFilter");

            EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            if (emptyFilter != null)
                return new AlwaysFalseRecordFilter() { };

            NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
                return null;
            BooleanRecordFilter booleanRecordFilter = context.InitialFilter as BooleanRecordFilter;
            if (booleanRecordFilter != null)
            {
                Guid defaultSecurityProviderId = new ConfigManager().GetDefaultSecurityProviderId();
                return new StringRecordFilter()
                {
                    FieldName = FieldName,
                    Value = defaultSecurityProviderId.ToString(),
                    CompareOperator = booleanRecordFilter.IsTrue ? StringRecordFilterOperator.Equals : StringRecordFilterOperator.NotEquals
                };
            }
            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }
    }
}
