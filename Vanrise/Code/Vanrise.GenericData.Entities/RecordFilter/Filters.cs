using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class EmptyRecordFilter : RecordFilter
    {
        public override string GetDescription(IRecordFilterGetDescriptionContext context)
        {
            return string.Format(" {0} Is Empty ", context.GetFieldTitle(FieldName));
        }
    }

    public class NonEmptyRecordFilter : RecordFilter
    {
        public override string GetDescription(IRecordFilterGetDescriptionContext context)
        {
            return string.Format(" {0} Is Non Empty ", context.GetFieldTitle(FieldName));
        }
    }

    public enum StringRecordFilterOperator
    {
        [Description(" = ")]
        Equals = 0,
        [Description(" <> ")]
        NotEquals = 1,
        [Description(" Starts With ")]
        StartsWith = 2,
        [Description(" Not Starts With ")]
        NotStartsWith = 3,
        [Description(" Ends With ")]
        EndsWith = 4,
        [Description(" Not Ends With ")]
        NotEndsWith = 5,
        [Description(" Contains ")]
        Contains = 6,
        [Description(" Not Contains ")]
        NotContains = 7
    }
    public class StringRecordFilter : RecordFilter
    {
        public StringRecordFilterOperator CompareOperator { get; set; }

        public string Value { get; set; }

        public Guid? ValueParameterId { get; set; }

        public override string GetDescription(IRecordFilterGetDescriptionContext context)
        {
            return string.Format(" {0} {1} {2} ", context.GetFieldTitle(FieldName), Utilities.GetEnumDescription(CompareOperator), context.GetFieldValueDescription(FieldName, Value));
        }

        public override void SetValueFromParameters(IRecordFilterSetValueFromParametersContext context)
        {
            if (ValueParameterId.HasValue)
            {
                dynamic parameterValue;
                if (context.TryGetParameterValue(this.ValueParameterId.Value, out parameterValue))
                    this.Value = parameterValue as string;
            }
        }
    }

    public enum NumberRecordFilterOperator
    {
        [Description(" = ")]
        Equals = 0,
        [Description(" <> ")]
        NotEquals = 1,
        [Description(" > ")]
        Greater = 2,
        [Description(" >= ")]
        GreaterOrEquals = 3,
        [Description(" < ")]
        Less = 4,
        [Description(" <= ")]
        LessOrEquals = 5
    }
    public class NumberRecordFilter : RecordFilter
    {
        public NumberRecordFilterOperator CompareOperator { get; set; }

        public Decimal Value { get; set; }

        public Guid? ValueParameterId { get; set; }

        public override string GetDescription(IRecordFilterGetDescriptionContext context)
        {
            return string.Format(" {0} {1} {2} ", context.GetFieldTitle(FieldName), Utilities.GetEnumDescription(CompareOperator), context.GetFieldValueDescription(FieldName, Value));
        }

        public override void SetValueFromParameters(IRecordFilterSetValueFromParametersContext context)
        {
            if (this.ValueParameterId.HasValue)
            {
                dynamic parameterValue;
                if (context.TryGetParameterValue(this.ValueParameterId.Value, out parameterValue))
                    this.Value = (Decimal)parameterValue;
            }
        }
    }

    public enum DateTimeRecordFilterComparisonPart { DateTime = 0, TimeOnly = 1, DateOnly = 2, YearMonth = 3, YearWeek = 4, Hour = 5 }
    public enum DateTimeRecordFilterOperator
    {
        [Description(" = "), DateTimeRecordFilterOperatorAttribute(false)]
        Equals = 0,
        [Description(" <> "), DateTimeRecordFilterOperatorAttribute(false)]
        NotEquals = 1,
        [Description(" > "), DateTimeRecordFilterOperatorAttribute(false)]
        Greater = 2,
        [Description(" >= "), DateTimeRecordFilterOperatorAttribute(false)]
        GreaterOrEquals = 3,
        [Description(" < "), DateTimeRecordFilterOperatorAttribute(false)]
        Less = 4,
        [Description(" <= "), DateTimeRecordFilterOperatorAttribute(false)]
        LessOrEquals = 5,
        [Description(" Between "), DateTimeRecordFilterOperatorAttribute(true)]
        Between = 6,
        [Description(" Not Between "), DateTimeRecordFilterOperatorAttribute(true)]
        NotBetween = 7
    }
    public class DateTimeRecordFilterOperatorAttribute : Attribute
    {
        public bool HasSecondValue { get; set; }

        public DateTimeRecordFilterOperatorAttribute(bool hasSecondValue)
        {
            this.HasSecondValue = hasSecondValue;
        }
    }
    public class DateTimeRecordFilter : RecordFilter
    {
        public DateTimeRecordFilterComparisonPart ComparisonPart { get; set; }

        public DateTimeRecordFilterOperator CompareOperator { get; set; }

        public object Value { get; set; }

        public object Value2 { get; set; }

        public bool ExcludeValue2 { get; set; }

        public Guid? ValueParameterId { get; set; }

        public override string GetDescription(IRecordFilterGetDescriptionContext context)
        {
            string fieldTitle = context.GetFieldTitle(FieldName);
            string compareOperatorDescription = Utilities.GetEnumDescription(CompareOperator);
            string valueDescription = GetValueDescription(this.Value);

            bool hasSecondValue = Vanrise.Common.Utilities.GetEnumAttribute<DateTimeRecordFilterOperator, DateTimeRecordFilterOperatorAttribute>(this.CompareOperator).HasSecondValue;
            if (hasSecondValue)
                return string.Format(" {0} {1} {2} and {3} ", fieldTitle, compareOperatorDescription, valueDescription, GetValueDescription(this.Value2));
            else
                return string.Format(" {0} {1} {2} ", fieldTitle, compareOperatorDescription, valueDescription);
        }

        public override void SetValueFromParameters(IRecordFilterSetValueFromParametersContext context)
        {
            if (this.ValueParameterId.HasValue)
            {
                dynamic parameterValue;
                if (context.TryGetParameterValue(this.ValueParameterId.Value, out parameterValue))
                    this.Value = (DateTime)parameterValue;
            }
        }

        private string GetValueDescription(object value)
        {
            switch (this.ComparisonPart)
            {
                case DateTimeRecordFilterComparisonPart.DateTime:
                    if (this.Value is DateTime)
                        return Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                    else
                        throw new DataIntegrityValidationException("DateTimeRecordFilter.Value should be of type DateTime");

                case DateTimeRecordFilterComparisonPart.DateOnly:
                    if (this.Value is DateTime)
                        return Convert.ToDateTime(value).ToString("yyyy-MM-dd");
                    else
                        throw new DataIntegrityValidationException("DateTimeRecordFilter.Value should be of type DateTime");

                case DateTimeRecordFilterComparisonPart.TimeOnly:
                    if (this.Value is Time)
                        return ((Vanrise.Entities.Time)value).ToShortTimeString();
                    else
                        throw new DataIntegrityValidationException("DateTimeRecordFilter.Value should be of type Time");

                case DateTimeRecordFilterComparisonPart.YearMonth:
                    if (this.Value is DateTime)
                        return Convert.ToDateTime(value).ToString("yyyy-MM");
                    else
                        throw new DataIntegrityValidationException("DateTimeRecordFilter.Value should be of type DateTime");

                case DateTimeRecordFilterComparisonPart.YearWeek:
                    if (this.Value is DateTime)
                        return Vanrise.Common.Utilities.GetWeekOfYearDescription((DateTime)this.Value);
                    else
                        throw new DataIntegrityValidationException("DateTimeRecordFilter.Value should be of type DateTime");

                case DateTimeRecordFilterComparisonPart.Hour:
                    if (this.Value is Time)
                        return ((Vanrise.Entities.Time)value).Hour.ToString();
                    else
                        throw new DataIntegrityValidationException("DateTimeRecordFilter.Value should be of type Time");

                default: throw new NotSupportedException(string.Format("ComparisonPart '{0}'", this.ComparisonPart));
            }
        }
    }

    public class BooleanRecordFilter : RecordFilter
    {
        public bool IsTrue { get; set; }

        public Guid? ValueParameterId { get; set; }

        public override string GetDescription(IRecordFilterGetDescriptionContext context)
        {
            return string.Format(" {0} {1} ", context.GetFieldTitle(FieldName), context.GetFieldValueDescription(FieldName, IsTrue));
        }

        public override void SetValueFromParameters(IRecordFilterSetValueFromParametersContext context)
        {
            if (this.ValueParameterId.HasValue)
            {
                dynamic parameterValue;
                if (context.TryGetParameterValue(this.ValueParameterId.Value, out parameterValue))
                    this.IsTrue = (bool)parameterValue;
            }
        }
    }

    public enum ListRecordFilterOperator
    {
        [Description("Specific")]
        In = 0,
        [Description("All Except")]
        NotIn = 1
    }
    public abstract class ListRecordFilter<T> : RecordFilter
    {
        public ListRecordFilterOperator CompareOperator { get; set; }

        public List<T> Values { get; set; }

        public List<Guid> ValueParameterIds { get; set; }

        public override void SetValueFromParameters(IRecordFilterSetValueFromParametersContext context)
        {
            if (this.ValueParameterIds != null)
            {
                foreach (var valueParameterId in this.ValueParameterIds)
                {
                    dynamic parameterValue;
                    if (context.TryGetParameterValue(valueParameterId, out parameterValue))
                    {
                        if (this.Values == null)
                            this.Values = new List<T>();
                        this.Values.Add(parameterValue);
                    }
                }
            }
            base.SetValueFromParameters(context);
        }
    }
    public class NumberListRecordFilter : ListRecordFilter<Decimal>
    {
        public override string GetDescription(IRecordFilterGetDescriptionContext context)
        {
            return string.Format(" {0} {1} ( {2} ) ", context.GetFieldTitle(FieldName), Utilities.GetEnumDescription(CompareOperator), context.GetFieldValueDescription(FieldName, Values));
        }
    }
    public class StringListRecordFilter : ListRecordFilter<String>
    {
        public override string GetDescription(IRecordFilterGetDescriptionContext context)
        {
            return string.Format(" {0} {1} ( {2} ) ", context.GetFieldTitle(FieldName), Utilities.GetEnumDescription(CompareOperator), context.GetFieldValueDescription(FieldName, Values));
        }
    }
    public class ObjectListRecordFilter : ListRecordFilter<Object>
    {
        public override string GetDescription(IRecordFilterGetDescriptionContext context)
        {
            return string.Format(" {0} {1} ( {2} ) ", context.GetFieldTitle(FieldName), Utilities.GetEnumDescription(CompareOperator), context.GetFieldValueDescription(FieldName, Values.Cast<Object>().ToList()));
        }
    }

    public class AlwaysFalseRecordFilter : RecordFilter
    {
        public override string GetDescription(IRecordFilterGetDescriptionContext context)
        {
            throw new NotImplementedException();
        }
    }
}