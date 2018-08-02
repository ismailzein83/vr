using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Data.RDB
{
    public class RecordFilterRDBBuilder
    {
        Action<string, RDBExpressionContext> _setExpressionFromField;

        public RecordFilterRDBBuilder(Action<string, RDBExpressionContext> setExpressionFromField)
        {
            _setExpressionFromField = setExpressionFromField;
        }

        public void RecordFilterGroupCondition(RDBConditionContext conditionContext, RecordFilterGroup filterGroup)
        {
            if (filterGroup == null || filterGroup.Filters == null)
                return;

            var groupConditionContext = conditionContext.ChildConditionGroup(filterGroup.LogicalOperator == RecordQueryLogicalOperator.And ? RDBConditionGroupOperator.AND : RDBConditionGroupOperator.OR);

            foreach (var filter in filterGroup.Filters)
            {
                RecordFilterGroup childFilterGroup = filter as RecordFilterGroup;
                if (childFilterGroup != null)
                {
                    RecordFilterGroupCondition(groupConditionContext, childFilterGroup);
                    continue;
                }

                EmptyRecordFilter emptyFilter = filter as EmptyRecordFilter;
                if (emptyFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, emptyFilter);
                    continue;
                }

                NonEmptyRecordFilter nonEmptyFilter = filter as NonEmptyRecordFilter;
                if (nonEmptyFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, nonEmptyFilter);
                    continue;
                }

                StringRecordFilter stringFilter = filter as StringRecordFilter;
                if (stringFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, stringFilter);
                    continue;
                }

                NumberRecordFilter numberFilter = filter as NumberRecordFilter;
                if (numberFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, numberFilter);
                    continue;
                }

                DateTimeRecordFilter dateTimeFilter = filter as DateTimeRecordFilter;
                if (dateTimeFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, dateTimeFilter);
                    continue;
                }

                BooleanRecordFilter booleanFilter = filter as BooleanRecordFilter;
                if (booleanFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, booleanFilter);
                    continue;
                }

                NumberListRecordFilter numberListFilter = filter as NumberListRecordFilter;
                if (numberListFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, numberListFilter);
                    continue;
                }

                StringListRecordFilter stringListRecordFilter = filter as StringListRecordFilter;
                if (stringListRecordFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, stringListRecordFilter);
                    continue;
                }

                ObjectListRecordFilter objectListRecordFilter = filter as ObjectListRecordFilter;
                if (objectListRecordFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, objectListRecordFilter);
                    continue;
                }

                AlwaysFalseRecordFilter alwaysFalseFilter = filter as AlwaysFalseRecordFilter;
                if (alwaysFalseFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, alwaysFalseFilter);
                    continue;
                }

                AlwaysTrueRecordFilter alwaysTrueFilter = filter as AlwaysTrueRecordFilter;
                if (alwaysTrueFilter != null)
                {
                    RecordFilterCondition(groupConditionContext, alwaysTrueFilter);
                    continue;
                }
            }
        }

        private void RecordFilterCondition(RDBConditionContext conditionContext, EmptyRecordFilter emptyFilter)
        {
            _setExpressionFromField(emptyFilter.FieldName, conditionContext.NullCondition());
        }

        private void RecordFilterCondition(RDBConditionContext conditionContext, NonEmptyRecordFilter nonEmptyFilter)
        {
            _setExpressionFromField(nonEmptyFilter.FieldName, conditionContext.NotNullCondition());
        }

        private void RecordFilterCondition(RDBConditionContext conditionContext, StringRecordFilter filter)
        {
            RDBCompareConditionOperator compareOperator;
            switch (filter.CompareOperator)
            {
                case StringRecordFilterOperator.Equals: compareOperator = RDBCompareConditionOperator.Eq; break;
                case StringRecordFilterOperator.NotEquals: compareOperator = RDBCompareConditionOperator.NEq; break;
                case StringRecordFilterOperator.Contains: compareOperator = RDBCompareConditionOperator.Contains; break;
                case StringRecordFilterOperator.NotContains: compareOperator = RDBCompareConditionOperator.NotContains; break;
                case StringRecordFilterOperator.StartsWith: compareOperator = RDBCompareConditionOperator.StartWith; break;
                case StringRecordFilterOperator.NotStartsWith: compareOperator = RDBCompareConditionOperator.NotStartWith; break;
                case StringRecordFilterOperator.EndsWith: compareOperator = RDBCompareConditionOperator.EndWith; break;
                case StringRecordFilterOperator.NotEndsWith: compareOperator = RDBCompareConditionOperator.NotEndWith; break;
                default: throw new NotSupportedException(string.Format("stringFilter.CompareOperator '{0}'", filter.CompareOperator.ToString()));
            }
            var compareConditionContext = conditionContext.CompareCondition(compareOperator);
            _setExpressionFromField(filter.FieldName, compareConditionContext.Expression1());
            compareConditionContext.Expression2().Value(filter.Value);
        }

        private void RecordFilterCondition(RDBConditionContext conditionContext, NumberRecordFilter filter)
        {
            RDBCompareConditionOperator compareOperator;
            switch (filter.CompareOperator)
            {
                case NumberRecordFilterOperator.Equals: compareOperator = RDBCompareConditionOperator.Eq; break;
                case NumberRecordFilterOperator.NotEquals: compareOperator = RDBCompareConditionOperator.NEq; break;
                case NumberRecordFilterOperator.Greater: compareOperator = RDBCompareConditionOperator.G; break;
                case NumberRecordFilterOperator.GreaterOrEquals: compareOperator = RDBCompareConditionOperator.GEq; break;
                case NumberRecordFilterOperator.Less: compareOperator = RDBCompareConditionOperator.L; break;
                case NumberRecordFilterOperator.LessOrEquals: compareOperator = RDBCompareConditionOperator.LEq; break;
                default: throw new NotSupportedException(string.Format("numberFilter.CompareOperator '{0}'", filter.CompareOperator.ToString()));
            }
            var compareConditionContext = conditionContext.CompareCondition(compareOperator);
            _setExpressionFromField(filter.FieldName, compareConditionContext.Expression1());
            compareConditionContext.Expression2().Value(filter.Value);
        }

        private void RecordFilterCondition(RDBConditionContext conditionContext, DateTimeRecordFilter dateTimeFilter)
        {

            BaseRDBExpression fieldDateExpression = null;
            _setExpressionFromField(dateTimeFilter.FieldName, conditionContext.CreateExpressionContext((expression) => fieldDateExpression = expression));

            BaseRDBExpression fieldDatePartExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, fieldDateExpression);

            DateTime firstDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value);
            BaseRDBExpression firstDateTimeValueExpression = BuildExpressionFromDateTime(conditionContext, firstDateTimeValue);
            BaseRDBExpression startDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, firstDateTimeValueExpression);

            switch (dateTimeFilter.ComparisonPart)
            {
                case DateTimeRecordFilterComparisonPart.DateTime:
                case DateTimeRecordFilterComparisonPart.DateOnly:
                case DateTimeRecordFilterComparisonPart.TimeOnly:
                    GetDateTimeRecordFilterQuery(conditionContext, dateTimeFilter, fieldDatePartExpression, startDateExpression);
                    break;
                case DateTimeRecordFilterComparisonPart.YearMonth:
                    GetYearMonthRecordFilterQuery(conditionContext, dateTimeFilter, fieldDatePartExpression, startDateExpression, firstDateTimeValue);
                    break;
                case DateTimeRecordFilterComparisonPart.YearWeek:
                    GetYearWeekRecordFilterQuery(conditionContext, dateTimeFilter, fieldDatePartExpression, startDateExpression, firstDateTimeValue);
                    break;
                case DateTimeRecordFilterComparisonPart.Hour:
                    GetHourRecordFilterQuery(conditionContext, dateTimeFilter, fieldDatePartExpression, startDateExpression, firstDateTimeValue);
                    break;
                default: throw new NotSupportedException(string.Format("dateTimeFilter.ComparisonPart '{0}'", dateTimeFilter.ComparisonPart));
            }
        }

        private BaseRDBExpression BuildExpressionFromDateTime(RDBConditionContext conditionContext, DateTime value)
        {
            BaseRDBExpression rdbExpression = null;
            conditionContext.CreateExpressionContext((expression) => rdbExpression = expression).Value(value);
            return rdbExpression;
        }

        private void GetDateTimeRecordFilterQuery(RDBConditionContext conditionContext, DateTimeRecordFilter dateTimeFilter,
            BaseRDBExpression fieldDatePartExpression, BaseRDBExpression startDateExpression)
        {
            RDBCompareConditionOperator? compareOperator = null;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals: compareOperator = RDBCompareConditionOperator.Eq; break;
                case DateTimeRecordFilterOperator.NotEquals: compareOperator = RDBCompareConditionOperator.NEq; break;
                case DateTimeRecordFilterOperator.Greater: compareOperator = RDBCompareConditionOperator.G; break;
                case DateTimeRecordFilterOperator.GreaterOrEquals: compareOperator = RDBCompareConditionOperator.GEq; break;
                case DateTimeRecordFilterOperator.Less: compareOperator = RDBCompareConditionOperator.L; break;
                case DateTimeRecordFilterOperator.LessOrEquals: compareOperator = RDBCompareConditionOperator.LEq; break;
            }
            if (compareOperator.HasValue)
            {
                conditionContext.CompareCondition(fieldDatePartExpression, compareOperator.Value, startDateExpression);
                return;
            }
            dateTimeFilter.Value2.ThrowIfNull("dateTimeFilter.Value2", dateTimeFilter.FieldName);
            DateTime secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
            BaseRDBExpression secondDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, secondDateTimeValue));
            if (dateTimeFilter.CompareOperator == DateTimeRecordFilterOperator.Between)
            {
                var andCondition = conditionContext.ChildConditionGroup();
                andCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                andCondition.CompareCondition(fieldDatePartExpression, dateTimeFilter.ExcludeValue2 ? RDBCompareConditionOperator.L : RDBCompareConditionOperator.LEq, secondDateExpression);

                return;
            }

            if (dateTimeFilter.CompareOperator == DateTimeRecordFilterOperator.NotBetween)
            {
                var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                orCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                orCondition.CompareCondition(fieldDatePartExpression, dateTimeFilter.ExcludeValue2 ? RDBCompareConditionOperator.G : RDBCompareConditionOperator.GEq, secondDateExpression);
                return;
            }

            throw new NotSupportedException(string.Format("dateTimeFilter.CompareOperator '{0}'", dateTimeFilter.CompareOperator.ToString()));
        }

        private void GetYearMonthRecordFilterQuery(RDBConditionContext conditionContext, DateTimeRecordFilter dateTimeFilter,
            BaseRDBExpression fieldDatePartExpression, BaseRDBExpression startDateExpression, DateTime firstDateTimeValue)
        {

            DateTime secondDateTimeValue;

            DateTime endDate;
            BaseRDBExpression endDateExpression;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var andCondition = conditionContext.ChildConditionGroup();
                    andCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                    andCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.NotEquals:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                    orCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                    orCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.Greater:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.GreaterOrEquals:
                    conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                    return;
                case DateTimeRecordFilterOperator.Less:
                    conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                    return;
                case DateTimeRecordFilterOperator.LessOrEquals:
                    endDate = GetFirstDayOfNextMonth(firstDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.Between:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = GetFirstDayOfNextMonth(secondDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var andCondition2 = conditionContext.ChildConditionGroup();
                    andCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                    andCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.NotBetween:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = GetFirstDayOfNextMonth(secondDateTimeValue);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var orCondition2 = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                    orCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                    orCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression);
                    return;
                default: throw new NotSupportedException(string.Format("dateTimeFilter.CompareOperator '{0}'", dateTimeFilter.CompareOperator));
            }
        }

        private void GetYearWeekRecordFilterQuery(RDBConditionContext conditionContext, DateTimeRecordFilter dateTimeFilter,
             BaseRDBExpression fieldDatePartExpression, BaseRDBExpression startDateExpression, DateTime firstDateTimeValue)
        {
            DateTime secondDateTimeValue;

            DateTime endDate;
            BaseRDBExpression endDateExpression;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var andCondition = conditionContext.ChildConditionGroup();
                    andCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                    andCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.NotEquals:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                    orCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                    orCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.Greater:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.GreaterOrEquals:
                    conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                    return;
                case DateTimeRecordFilterOperator.Less:
                    conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                    return;
                case DateTimeRecordFilterOperator.LessOrEquals:
                    endDate = firstDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.Between:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var andCondition2 = conditionContext.ChildConditionGroup();
                    andCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                    andCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.NotBetween:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddDays(7);
                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var orCondition2 = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                    orCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                    orCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, endDateExpression);
                    return;
                default: throw new NotSupportedException(string.Format("dateTimeFilter.CompareOperator '{0}'", dateTimeFilter.CompareOperator));
            }
        }

        private void GetHourRecordFilterQuery(RDBConditionContext conditionContext, DateTimeRecordFilter dateTimeFilter,
             BaseRDBExpression fieldDatePartExpression, BaseRDBExpression startDateExpression, DateTime firstDateTimeValue)
        {
            DateTime secondDateTimeValue;

            DateTime endDate;
            BaseRDBExpression endDateExpression;

            bool isMidnight;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var andCondition = conditionContext.ChildConditionGroup();
                    andCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                    andCondition.CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.LEq : RDBCompareConditionOperator.L, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.NotEquals:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                    orCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                    orCondition.CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.G : RDBCompareConditionOperator.GEq, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.Greater:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    conditionContext.CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.G : RDBCompareConditionOperator.GEq, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.GreaterOrEquals:
                    conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                    return;
                case DateTimeRecordFilterOperator.Less:
                    conditionContext.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                    return;
                case DateTimeRecordFilterOperator.LessOrEquals:
                    endDate = firstDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(firstDateTimeValue.Year, firstDateTimeValue.Month, firstDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    conditionContext.CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.LEq : RDBCompareConditionOperator.L, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.Between:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(secondDateTimeValue.Year, secondDateTimeValue.Month, secondDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var andCondition2 = conditionContext.ChildConditionGroup();
                    andCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
                    andCondition2.CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.LEq : RDBCompareConditionOperator.L, endDateExpression);
                    return;
                case DateTimeRecordFilterOperator.NotBetween:
                    secondDateTimeValue = GetDateTimeValue(dateTimeFilter.ComparisonPart, dateTimeFilter.Value2);
                    endDate = secondDateTimeValue.AddHours(1);
                    isMidnight = CheckMidnight(endDate);
                    if (isMidnight)
                        endDate = new DateTime(secondDateTimeValue.Year, secondDateTimeValue.Month, secondDateTimeValue.Day, 23, 59, 59, 998);

                    endDateExpression = GetDateExpressionBasedOnDatePart(conditionContext, dateTimeFilter.ComparisonPart, BuildExpressionFromDateTime(conditionContext, endDate));
                    var orCondition2 = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                    orCondition2.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.L, startDateExpression);
                    orCondition2.CompareCondition(fieldDatePartExpression, isMidnight ? RDBCompareConditionOperator.G : RDBCompareConditionOperator.GEq, endDateExpression);
                    return;
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

        private BaseRDBExpression GetDateExpressionBasedOnDatePart(RDBConditionContext conditionContext, DateTimeRecordFilterComparisonPart comparisonPart, BaseRDBExpression dateExpression)
        {
            if (comparisonPart == DateTimeRecordFilterComparisonPart.DateTime || comparisonPart == DateTimeRecordFilterComparisonPart.YearMonth)
            {
                return dateExpression;
            }
            else
            {
                RDBDateTimePart part;
                switch (comparisonPart)
                {
                    case DateTimeRecordFilterComparisonPart.DateOnly:
                    case DateTimeRecordFilterComparisonPart.YearWeek: part = RDBDateTimePart.DateOnly; break;
                    case DateTimeRecordFilterComparisonPart.TimeOnly:
                    case DateTimeRecordFilterComparisonPart.Hour: part = RDBDateTimePart.TimeOnly; break;
                    default: throw new NotSupportedException(string.Format("comparisonPart '{0}'", comparisonPart));
                }
                BaseRDBExpression dateTimePartExpression = null;
                conditionContext.CreateExpressionContext((expression) => dateTimePartExpression = expression).DateTimePart(part).Expression(dateExpression);
                return dateTimePartExpression;
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



        private void RecordFilterCondition(RDBConditionContext conditionContext, BooleanRecordFilter booleanFilter)
        {
            BaseRDBExpression fieldExpression = null;
            _setExpressionFromField(booleanFilter.FieldName, conditionContext.CreateExpressionContext((expression) => fieldExpression = expression));
            BaseRDBExpression valueExpression = null;
            conditionContext.CreateExpressionContext((exp) => valueExpression = exp).Value(booleanFilter.IsTrue);
            conditionContext.CompareCondition(fieldExpression, RDBCompareConditionOperator.Eq, valueExpression);
        }

        private void RecordFilterCondition(RDBConditionContext conditionContext, NumberListRecordFilter numberListFilter)
        {
            List<BaseRDBExpression> valueExpressions = null;
            if (numberListFilter.Values != null)
            {
                valueExpressions = new List<BaseRDBExpression>();
                foreach (var value in numberListFilter.Values)
                {
                    conditionContext.CreateExpressionContext((expression) => valueExpressions.Add(expression)).Value(value);
                }
            }
            RecordFilterCondition(conditionContext, numberListFilter.FieldName, numberListFilter.CompareOperator, valueExpressions);

        }

        private void RecordFilterCondition(RDBConditionContext conditionContext, StringListRecordFilter stringListRecordFilter)
        {
            List<BaseRDBExpression> valueExpressions = null;
            if (stringListRecordFilter.Values != null)
            {
                valueExpressions = new List<BaseRDBExpression>();
                foreach (var value in stringListRecordFilter.Values)
                {
                    conditionContext.CreateExpressionContext((expression) => valueExpressions.Add(expression)).Value(value);
                }
            }
            RecordFilterCondition(conditionContext, stringListRecordFilter.FieldName, stringListRecordFilter.CompareOperator, valueExpressions);
        }

        private void RecordFilterCondition(RDBConditionContext conditionContext, ObjectListRecordFilter stringListRecordFilter)
        {
            List<BaseRDBExpression> valueExpressions = null;
            if (stringListRecordFilter.Values != null)
            {
                valueExpressions = new List<BaseRDBExpression>();
                foreach (var value in stringListRecordFilter.Values)
                {
                    conditionContext.CreateExpressionContext((expression) => valueExpressions.Add(expression)).ObjectValue(value);
                }
            }
            RecordFilterCondition(conditionContext, stringListRecordFilter.FieldName, stringListRecordFilter.CompareOperator, valueExpressions);

        }

        private void RecordFilterCondition(RDBConditionContext conditionContext, string fieldName, ListRecordFilterOperator compareOperator, List<BaseRDBExpression> values)
        {
            BaseRDBExpression fieldExpression = null;
            _setExpressionFromField(fieldName, conditionContext.CreateExpressionContext((expression) => fieldExpression = expression));
            conditionContext.ListCondition(fieldExpression, compareOperator == ListRecordFilterOperator.In ? RDBListConditionOperator.IN : RDBListConditionOperator.NotIN, values);
        }

        private void RecordFilterCondition(RDBConditionContext conditionContext, AlwaysFalseRecordFilter alwaysFalseFilter)
        {
            conditionContext.FalseCondition();
        }

        private void RecordFilterCondition(RDBConditionContext conditionContext, AlwaysTrueRecordFilter alwaysTrueFilter)
        {
            conditionContext.TrueCondition();
        }
    }
}
