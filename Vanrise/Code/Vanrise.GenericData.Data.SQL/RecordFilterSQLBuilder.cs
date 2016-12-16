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
            string parameterName = GenerateParameterName(ref parameterIndex);
            string compareOperator = null;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals: compareOperator = "="; break;
                case DateTimeRecordFilterOperator.NotEquals: compareOperator = "<>"; break;
                case DateTimeRecordFilterOperator.Greater: compareOperator = ">"; break;
                case DateTimeRecordFilterOperator.GreaterOrEquals: compareOperator = ">="; break;
                case DateTimeRecordFilterOperator.Less: compareOperator = "<"; break;
                case DateTimeRecordFilterOperator.LessOrEquals: compareOperator = "<="; break;
            }
            parameterValues.Add(parameterName, dateTimeFilter.Value);
            return string.Format("{0} {1} {2}", GetSQLExpression(dateTimeFilter), compareOperator, parameterName);
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
