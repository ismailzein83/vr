using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas
{
    public class CompareToConstantFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("F5B55B95-503D-4B7B-A1BB-7A7BCB52C5DF"); } }

        public string FieldName { get; set; }

        public CompareToConstantOperator Operator { get; set; }

        public decimal Value { get; set; }

        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { this.FieldName };
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            dynamic fieldValue = context.GetFieldValue(this.FieldName);
            if (fieldValue == null)
                return null;

            switch (this.Operator)
            {
                case CompareToConstantOperator.Greater:
                    return fieldValue > this.Value;

                case CompareToConstantOperator.GreaterOrEquals:
                    return fieldValue >= this.Value;

                case CompareToConstantOperator.Less:
                    return fieldValue < this.Value;

                case CompareToConstantOperator.LessOrEquals:
                    return fieldValue <= this.Value;

                default: throw new NotSupportedException(string.Format("Compare to constant Operator '{0}'", this.Operator));
            }
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            if (context.InitialFilter == null)
                throw new ArgumentNullException("context.InitialFilter");

            BooleanRecordFilter booleanRecordFilter = context.InitialFilter as BooleanRecordFilter;
            if (booleanRecordFilter != null)
            {
                NumberRecordFilter numberRecordFilter = new NumberRecordFilter();
                numberRecordFilter.FieldName = this.FieldName;
                numberRecordFilter.Value = this.Value;

                switch (this.Operator)
                {
                    case CompareToConstantOperator.Greater:
                        if (booleanRecordFilter.IsTrue)
                            numberRecordFilter.CompareOperator = NumberRecordFilterOperator.Greater;
                        else
                            numberRecordFilter.CompareOperator = NumberRecordFilterOperator.LessOrEquals;
                        return numberRecordFilter;

                    case CompareToConstantOperator.GreaterOrEquals:
                        if (booleanRecordFilter.IsTrue)
                            numberRecordFilter.CompareOperator = NumberRecordFilterOperator.GreaterOrEquals;
                        else
                            numberRecordFilter.CompareOperator = NumberRecordFilterOperator.Less;
                        return numberRecordFilter;

                    case CompareToConstantOperator.Less:
                        if (booleanRecordFilter.IsTrue)
                            numberRecordFilter.CompareOperator = NumberRecordFilterOperator.Less;
                        else
                            numberRecordFilter.CompareOperator = NumberRecordFilterOperator.GreaterOrEquals;
                        return numberRecordFilter;

                    case CompareToConstantOperator.LessOrEquals:
                        if (booleanRecordFilter.IsTrue)
                            numberRecordFilter.CompareOperator = NumberRecordFilterOperator.LessOrEquals;
                        else
                            numberRecordFilter.CompareOperator = NumberRecordFilterOperator.Greater;
                        return numberRecordFilter;

                    default: throw new NotSupportedException(string.Format("Compare to constant Operator '{0}'", this.Operator));
                }
            }

            EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            if (emptyFilter != null)
                return new EmptyRecordFilter() { FieldName = this.FieldName };

            NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
                return new NonEmptyRecordFilter { FieldName = this.FieldName };

            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }
    }

    public enum CompareToConstantOperator
    {
        [Description(" > ")]
        Greater = 0,
        [Description(" >= ")]
        GreaterOrEquals = 1,
        [Description(" < ")]
        Less = 2,
        [Description(" <= ")]
        LessOrEquals = 3
    }
}
