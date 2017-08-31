using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas
{
    public enum MathOperator { Multiplication = 0, Division = 1 }

    public class SingleMathOperationFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("5B8471BA-3D9A-412D-A158-27BDEC9C4094"); } }

        public string FieldName { get; set; }

        public MathOperator MathOperator { get; set; }

        public decimal Value { get; set; }

        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { this.FieldName };
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            if (Value == 0)
                throw new Exception("Value should be different than zero");

            dynamic fieldValue = context.GetFieldValue(this.FieldName);

            switch (this.MathOperator)
            {
                case MathOperator.Multiplication:
                    return fieldValue * this.Value;

                case MathOperator.Division:
                    return fieldValue / this.Value;

                default: throw new NotSupportedException(string.Format("Mathematical Operator '{0}'", this.MathOperator));
            }
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            if (Value == 0)
                throw new Exception("Value should be different than zero");

            if (context.InitialFilter == null)
                throw new ArgumentNullException("context.InitialFilter");

            NumberRecordFilter numberRecordFilter = context.InitialFilter as NumberRecordFilter;
            if (numberRecordFilter != null)
            {
                switch (this.MathOperator)
                {
                    case MathOperator.Multiplication:
                        numberRecordFilter.FieldName = this.FieldName;
                        numberRecordFilter.Value = numberRecordFilter.Value / this.Value;
                        return numberRecordFilter;

                    case MathOperator.Division:
                        numberRecordFilter.FieldName = this.FieldName;
                        numberRecordFilter.Value = numberRecordFilter.Value * this.Value;
                        return numberRecordFilter;

                    default: throw new NotSupportedException(string.Format("Mathematical Operator '{0}'", this.MathOperator));
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
}