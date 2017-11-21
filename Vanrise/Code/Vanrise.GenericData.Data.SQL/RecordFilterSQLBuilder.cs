using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class RecordFilterSQLBuilder
    {
        Func<string, string> _getSQLExpressionFromFieldName;
        public RecordFilterSQLBuilder(Func<string, string> getSQLExpressionFromFieldName)
        {
            if (getSQLExpressionFromFieldName == null)
                throw new ArgumentNullException("getSQLExpressionFromFieldName");
            _getSQLExpressionFromFieldName = getSQLExpressionFromFieldName;
        }

        public string BuildRecordFilter(RecordFilterGroup filterGroup, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            if (filterGroup == null || filterGroup.Filters == null)
                return null;

            StringBuilder builder = new StringBuilder();
            foreach (var filter in filterGroup.Filters)
            {
                if (builder.Length > 0)
                    builder.AppendFormat(" {0} ", filterGroup.LogicalOperator);

                RecordFilterGroup childFilterGroup = filter as RecordFilterGroup;
                if (childFilterGroup != null)
                {
                    builder.Append(BuildRecordFilter(childFilterGroup, ref parameterIndex, parameterValues));
                    continue;
                }

                EmptyRecordFilter emptyFilter = filter as EmptyRecordFilter;
                if (emptyFilter != null)
                {
                    builder.Append(BuildRecordFilter(emptyFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                NonEmptyRecordFilter nonEmptyFilter = filter as NonEmptyRecordFilter;
                if (nonEmptyFilter != null)
                {
                    builder.Append(BuildRecordFilter(nonEmptyFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                StringRecordFilter stringFilter = filter as StringRecordFilter;
                if (stringFilter != null)
                {
                    builder.Append(BuildRecordFilter(stringFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                NumberRecordFilter numberFilter = filter as NumberRecordFilter;
                if (numberFilter != null)
                {
                    builder.Append(BuildRecordFilter(numberFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                DateTimeRecordFilter dateTimeFilter = filter as DateTimeRecordFilter;
                if (dateTimeFilter != null)
                {
                    builder.Append(BuildRecordFilter(dateTimeFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                BooleanRecordFilter booleanFilter = filter as BooleanRecordFilter;
                if (booleanFilter != null)
                {
                    builder.Append(BuildRecordFilter(booleanFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                NumberListRecordFilter numberListFilter = filter as NumberListRecordFilter;
                if (numberListFilter != null)
                {
                    builder.Append(BuildRecordFilter(numberListFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                StringListRecordFilter stringListRecordFilter = filter as StringListRecordFilter;
                if (stringListRecordFilter != null)
                {
                    builder.Append(BuildRecordFilter(stringListRecordFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                ObjectListRecordFilter objectListRecordFilter = filter as ObjectListRecordFilter;
                if (objectListRecordFilter != null)
                {
                    builder.Append(BuildRecordFilter(objectListRecordFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                AlwaysFalseRecordFilter alwaysFalseFilter = filter as AlwaysFalseRecordFilter;
                if (alwaysFalseFilter != null)
                {
                    builder.Append(BuildRecordFilter(alwaysFalseFilter, ref parameterIndex, parameterValues));
                    continue;
                }
            }

            return String.Format("({0})", builder);
        }

        private string BuildRecordFilter(EmptyRecordFilter emptyFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            return string.Format("{0} IS NULL", GetSQLExpression(emptyFilter));
        }

        private string BuildRecordFilter(NonEmptyRecordFilter nonEmptyFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            return string.Format("{0} IS NOT NULL", GetSQLExpression(nonEmptyFilter));
        }

        private string BuildRecordFilter(StringRecordFilter stringFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            string parameterName = GenerateParameterName(ref parameterIndex);
            string modifiedParameterName = parameterName;
            string compareOperator = null;

            switch (stringFilter.CompareOperator)
            {
                case StringRecordFilterOperator.Equals: compareOperator = "="; break;
                case StringRecordFilterOperator.NotEquals: compareOperator = "<>"; break;
                case StringRecordFilterOperator.Contains:
                    compareOperator = "LIKE";
                    modifiedParameterName = string.Format("'%' + {0} + '%'", parameterName);
                    break;
                case StringRecordFilterOperator.NotContains:
                    compareOperator = "NOT LIKE";
                    modifiedParameterName = string.Format("'%' + {0} + '%'", parameterName);
                    break;
                case StringRecordFilterOperator.StartsWith:
                    compareOperator = "LIKE";
                    modifiedParameterName = string.Format("{0} + '%'", parameterName);
                    break;
                case StringRecordFilterOperator.NotStartsWith:
                    compareOperator = "NOT LIKE";
                    modifiedParameterName = string.Format("{0} + '%'", parameterName);
                    break;
                case StringRecordFilterOperator.EndsWith:
                    compareOperator = "LIKE";
                    modifiedParameterName = string.Format("'%' + {0}", parameterName);
                    break;
                case StringRecordFilterOperator.NotEndsWith:
                    compareOperator = "NOT LIKE";
                    modifiedParameterName = string.Format("'%' + {0}", parameterName);
                    break;
            }
            parameterValues.Add(parameterName, stringFilter.Value);
            return string.Format("{0} {1} {2}", GetSQLExpression(stringFilter), compareOperator, modifiedParameterName);
        }

        private string BuildRecordFilter(NumberRecordFilter numberFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            string parameterName = GenerateParameterName(ref parameterIndex);
            string compareOperator = null;

            switch (numberFilter.CompareOperator)
            {
                case NumberRecordFilterOperator.Equals: compareOperator = "="; break;
                case NumberRecordFilterOperator.NotEquals: compareOperator = "<>"; break;
                case NumberRecordFilterOperator.Greater: compareOperator = ">"; break;
                case NumberRecordFilterOperator.GreaterOrEquals: compareOperator = ">="; break;
                case NumberRecordFilterOperator.Less: compareOperator = "<"; break;
                case NumberRecordFilterOperator.LessOrEquals: compareOperator = "<="; break;
            }
            parameterValues.Add(parameterName, numberFilter.Value);
            return string.Format("{0} {1} {2}", GetSQLExpression(numberFilter), compareOperator, parameterName);
        }


        private string BuildRecordFilter(DateTimeRecordFilter dateTimeFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            string columnName = GetSQLExpression(dateTimeFilter);
            string castedColumnName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, columnName);

            DateTime firstDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value);
            string startDateParameterName = GenerateParameterName(ref parameterIndex);
            parameterValues.Add(startDateParameterName, firstDateTimeValue);
            string castedStartDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, startDateParameterName);

            switch (dateTimeFilter.ComparisonPart)
            {
                case DateTimeRecordFilterComparisonPart.DateTime:
                case DateTimeRecordFilterComparisonPart.DateOnly:
                case DateTimeRecordFilterComparisonPart.TimeOnly:
                    return GetDateTimeRecordFilterQuery(dateTimeFilter, ref parameterIndex, parameterValues, castedColumnName, castedStartDateParameterName);

                case DateTimeRecordFilterComparisonPart.YearMonth:
                    return GetYearMonthRecordFilterQuery(dateTimeFilter, ref parameterIndex, parameterValues, castedColumnName, castedStartDateParameterName, firstDateTimeValue);

                case DateTimeRecordFilterComparisonPart.YearWeek:
                    return GetYearWeekRecordFilterQuery(dateTimeFilter, ref parameterIndex, parameterValues, castedColumnName, castedStartDateParameterName, firstDateTimeValue);

                case DateTimeRecordFilterComparisonPart.Hour:
                    return GetHourRecordFilterQuery(dateTimeFilter, ref parameterIndex, parameterValues, castedColumnName, castedStartDateParameterName, firstDateTimeValue);

                default: throw new NotSupportedException(string.Format("dateTimeFilter.ComparisonPart '{0}'", dateTimeFilter.ComparisonPart));
            }
        }

        private string GetDateTimeRecordFilterQuery(DateTimeRecordFilter dateTimeFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues,
            string castedColumnName, string castedStartDateParameterName)
        {
            string compareOperator;
            string endDateParameterName;
            string castedEndDateParameterName;
            string secondDateTimeValueQuery = string.Empty;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals: compareOperator = "="; break;
                case DateTimeRecordFilterOperator.NotEquals: compareOperator = "<>"; break;
                case DateTimeRecordFilterOperator.Greater: compareOperator = ">"; break;
                case DateTimeRecordFilterOperator.GreaterOrEquals: compareOperator = ">="; break;
                case DateTimeRecordFilterOperator.Less: compareOperator = "<"; break;
                case DateTimeRecordFilterOperator.LessOrEquals: compareOperator = "<="; break;

                case DateTimeRecordFilterOperator.Between:
                    compareOperator = ">=";
                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2));
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    secondDateTimeValueQuery = string.Format(" and {0} <{1} {2}", castedColumnName, dateTimeFilter.ExcludeValue2 ? "" : "=", castedEndDateParameterName);
                    break;

                case DateTimeRecordFilterOperator.NotBetween:
                    compareOperator = "<";
                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2));
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    secondDateTimeValueQuery = string.Format(" or {0} >{1} {2}", castedColumnName, dateTimeFilter.ExcludeValue2 ? "=" : "", castedEndDateParameterName);
                    break;

                default: throw new NotSupportedException(string.Format("dateTimeFilter.CompareOperator '{0}'", dateTimeFilter.CompareOperator));
            }

            return string.Format("({0} {1} {2} {3})", castedColumnName, compareOperator, castedStartDateParameterName, secondDateTimeValueQuery);
        }

        private string GetYearMonthRecordFilterQuery(DateTimeRecordFilter dateTimeFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues,
            string castedColumnName, string castedStartDateParameterName, DateTime firstDateTimeValue)
        {
            DateTime secondDateTimeValue;

            DateTime endDate;
            string endDateParameterName;
            string castedEndDateParameterName;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} >= {1} and {0} < {2})", castedColumnName, castedStartDateParameterName, castedEndDateParameterName);

                case DateTimeRecordFilterOperator.NotEquals:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} < {1} or {0} >= {2})", castedColumnName, castedStartDateParameterName, castedEndDateParameterName);

                case DateTimeRecordFilterOperator.Greater:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} >= {1})", castedColumnName, castedEndDateParameterName);

                case DateTimeRecordFilterOperator.GreaterOrEquals:
                    return string.Format("({0} >= {1})", castedColumnName, castedStartDateParameterName);

                case DateTimeRecordFilterOperator.Less:
                    return string.Format("({0} < {1})", castedColumnName, castedStartDateParameterName);

                case DateTimeRecordFilterOperator.LessOrEquals:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} < {1})", castedColumnName, castedEndDateParameterName);

                case DateTimeRecordFilterOperator.Between:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = GetFirstDayOfNextMonth(secondDateTimeValue);
                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} >= {1} and {0} < {2})", castedColumnName, castedStartDateParameterName, castedEndDateParameterName);

                case DateTimeRecordFilterOperator.NotBetween:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = GetFirstDayOfNextMonth(secondDateTimeValue);
                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} < {1} or {0} >= {2})", castedColumnName, castedStartDateParameterName, castedEndDateParameterName);

                default: throw new NotSupportedException(string.Format("dateTimeFilter.CompareOperator '{0}'", dateTimeFilter.CompareOperator));
            }
        }

        private string GetYearWeekRecordFilterQuery(DateTimeRecordFilter dateTimeFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues,
            string castedColumnName, string castedStartDateParameterName, DateTime firstDateTimeValue)
        {
            DateTime secondDateTimeValue;

            DateTime endDate;
            string endDateParameterName;
            string castedEndDateParameterName;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} >= {1} and {0} < {2})", castedColumnName, castedStartDateParameterName, castedEndDateParameterName);

                case DateTimeRecordFilterOperator.NotEquals:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} < {1} or {0} >= {2})", castedColumnName, castedStartDateParameterName, castedEndDateParameterName);

                case DateTimeRecordFilterOperator.Greater:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} >= {1})", castedColumnName, castedEndDateParameterName);

                case DateTimeRecordFilterOperator.GreaterOrEquals:
                    return string.Format("({0} >= {1})", castedColumnName, castedStartDateParameterName);

                case DateTimeRecordFilterOperator.Less:
                    return string.Format("({0} < {1})", castedColumnName, castedStartDateParameterName);

                case DateTimeRecordFilterOperator.LessOrEquals:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} < {1})", castedColumnName, castedEndDateParameterName);

                case DateTimeRecordFilterOperator.Between:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddDays(7);
                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} >= {1} and {0} < {2})", castedColumnName, castedStartDateParameterName, castedEndDateParameterName);

                case DateTimeRecordFilterOperator.NotBetween:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddDays(7);
                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} < {1} or {0} >= {2})", castedColumnName, castedStartDateParameterName, castedEndDateParameterName);

                default: throw new NotSupportedException(string.Format("dateTimeFilter.CompareOperator '{0}'", dateTimeFilter.CompareOperator));
            }
        }

        private string GetHourRecordFilterQuery(DateTimeRecordFilter dateTimeFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues,
            string castedColumnName, string castedStartDateParameterName, DateTime firstDateTimeValue)
        {
            DateTime secondDateTimeValue;

            DateTime endDate;
            string endDateParameterName;
            string castedEndDateParameterName;

            bool isMidnight;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} >= {1} and {0} <{2} {3})", castedColumnName, castedStartDateParameterName, isMidnight ? "=" : "", castedEndDateParameterName);

                case DateTimeRecordFilterOperator.NotEquals:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} < {1} or {0} >{2} {3})", castedColumnName, castedStartDateParameterName, isMidnight ? "" : "=", castedEndDateParameterName);

                case DateTimeRecordFilterOperator.Greater:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} >{1} {2})", castedColumnName, isMidnight ? "" : "=", castedEndDateParameterName);

                case DateTimeRecordFilterOperator.GreaterOrEquals:
                    return string.Format("({0} >= {1})", castedColumnName, castedStartDateParameterName);

                case DateTimeRecordFilterOperator.Less:
                    return string.Format("({0} < {1})", castedColumnName, castedStartDateParameterName);

                case DateTimeRecordFilterOperator.LessOrEquals:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} <{1} {2})", castedColumnName, isMidnight ? "=" : "", castedEndDateParameterName);

                case DateTimeRecordFilterOperator.Between:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(secondDateTimeValue.Year, secondDateTimeValue.Month, secondDateTimeValue.Day, 23, 59, 59, 998);

                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} >= {1} and {0} <{2} {3})", castedColumnName, castedStartDateParameterName, isMidnight ? "=" : "", castedEndDateParameterName);

                case DateTimeRecordFilterOperator.NotBetween:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(secondDateTimeValue.Year, secondDateTimeValue.Month, secondDateTimeValue.Day, 23, 59, 59, 998);

                    endDateParameterName = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(endDateParameterName, endDate);
                    castedEndDateParameterName = CastAsComparisonPart(dateTimeFilter.ComparisonPart, endDateParameterName);
                    return string.Format("({0} < {1} or {0} >{2} {3})", castedColumnName, castedStartDateParameterName, isMidnight ? "" : "=", castedEndDateParameterName);

                default: throw new NotSupportedException(string.Format("dateTimeFilter.CompareOperator '{0}'", dateTimeFilter.CompareOperator));
            }
        }

        private DateTime GetDateTimeValue(DateTimeRecordFilterComparisonPart comparisonPart, object value)
        {
            switch (comparisonPart)
            {
                case DateTimeRecordFilterComparisonPart.DateTime:
                case DateTimeRecordFilterComparisonPart.DateOnly:
                case DateTimeRecordFilterComparisonPart.YearMonth:
                    return (DateTime)value;

                case DateTimeRecordFilterComparisonPart.TimeOnly:
                    return Vanrise.Common.Utilities.AppendTimeToDateTime((Vanrise.Entities.Time)value, DateTime.Now);

                case DateTimeRecordFilterComparisonPart.Hour:
                    return Vanrise.Common.Utilities.AppendTimeToDateTime((Vanrise.Entities.Time)value, DateTime.Now);

                case DateTimeRecordFilterComparisonPart.YearWeek:
                    return Vanrise.Common.Utilities.GetMonday((DateTime)value);

                default: throw new NotSupportedException(string.Format("ComparisonPart '{0}'", comparisonPart));
            }
        }

        private string CastAsComparisonPart(DateTimeRecordFilterComparisonPart comparisonPart, string columnName)
        {
            switch (comparisonPart)
            {
                case DateTimeRecordFilterComparisonPart.DateTime: return columnName;
                case DateTimeRecordFilterComparisonPart.DateOnly: return string.Format("Cast({0} as date)", columnName);
                case DateTimeRecordFilterComparisonPart.TimeOnly: return string.Format("Cast({0} as time)", columnName);
                case DateTimeRecordFilterComparisonPart.YearMonth: return columnName; //string.Format("DATEADD(month, DATEDIFF(month, 0, {0}), 0)", columnName);
                case DateTimeRecordFilterComparisonPart.YearWeek: return string.Format("Cast({0} as date)", columnName);
                case DateTimeRecordFilterComparisonPart.Hour: return string.Format("Cast({0} as time)", columnName);
                default: throw new NotSupportedException(string.Format("comparisonPart '{0}'", comparisonPart));
            }
        }

        private DateTime GetFirstDayOfNextMonth(DateTime dateTime)
        {
            DateTime dateTimeNextMonth = dateTime.AddMonths(1);
            return new DateTime(dateTimeNextMonth.Year, dateTimeNextMonth.Month, 1);
        }

        private bool CheckMidnight(DateTime dateTime)
        {
            if (dateTime.Hour == 0 && dateTime.Minute == 0 && dateTime.Second == 0 && dateTime.Millisecond == 0)
                return true;
            return false;
        }


        private string BuildRecordFilter(BooleanRecordFilter booleanFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            return string.Format("{0} = {1}", GetSQLExpression(booleanFilter), booleanFilter.IsTrue ? "1" : "0");
        }

        private string BuildRecordFilter(NumberListRecordFilter numberListFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            return BuildRecordFilter<Decimal>(numberListFilter, ref parameterIndex, parameterValues);
        }

        private string BuildRecordFilter(StringListRecordFilter stringListRecordFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            return BuildRecordFilter<String>(stringListRecordFilter, ref parameterIndex, parameterValues);
        }

        private string BuildRecordFilter(ObjectListRecordFilter stringListRecordFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            return BuildRecordFilter<Object>(stringListRecordFilter, ref parameterIndex, parameterValues);
        }

        private string BuildRecordFilter(AlwaysFalseRecordFilter alwaysFalseFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            return string.Format("0 = 1");
        }

        string BuildRecordFilter<T>(ListRecordFilter<T> listFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            StringBuilder valuesBuilder = new StringBuilder();
            foreach (var value in listFilter.Values)
            {
                if (valuesBuilder.Length > 0)
                    valuesBuilder.Append(", ");
                string parameterName = GenerateParameterName(ref parameterIndex);
                parameterValues.Add(parameterName, value);
                valuesBuilder.Append(parameterName);
            }
            string compareOperator = listFilter.CompareOperator == ListRecordFilterOperator.In ? "IN" : "NOT IN";
            return string.Format("{0} {1} ({2})", GetSQLExpression(listFilter), compareOperator, valuesBuilder);
        }

        string GenerateParameterName(ref int parameterIndex)
        {
            return String.Format("@Prm_{0}", parameterIndex++);
        }

        string GetSQLExpression(RecordFilter recordFilter)
        {
            return _getSQLExpressionFromFieldName(recordFilter.FieldName);
        }
    }
}
