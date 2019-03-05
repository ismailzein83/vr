using System;
using System.Collections.Generic;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class BEDataUtility
    {
        public static void SetEffectiveDateCondition(RDBConditionContext conditionContext, string tableAlias, string colBED, string ColEED, DateTime effectiveOn)
        {
            //((sc.BED <= @when ) and (sc.EED is null or sc.EED > @when))
            var andConditionContext = conditionContext.ChildConditionGroup();
            andConditionContext.LessOrEqualCondition(tableAlias, colBED).Value(effectiveOn);
            var orCondition = andConditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(tableAlias, ColEED);
            orCondition.GreaterThanCondition(tableAlias, ColEED).Value(effectiveOn);
        }
        public static void SetFutureDateCondition(RDBConditionContext context, string tableAlias, string colBED, string colEED, DateTime effectiveAfter)
        {
            //(BED > effectiveAfter OR EED IS NULL)
            var effectiveCondition = context.ChildConditionGroup(RDBConditionGroupOperator.OR);
            effectiveCondition.NullCondition(tableAlias, colEED);
            effectiveCondition.GreaterThanCondition(tableAlias, colBED).Value(effectiveAfter);
        }

        public static void SetEffectiveAfterDateCondition(RDBConditionContext context, string tableAlias, string colBED, string ColEED, DateTime effectiveOn)
        {
            //(sc.EED is null or (sc.EED<>sc.BED and sc.EED > @when))
            var effectiveAfterDateCondition = context.ChildConditionGroup(RDBConditionGroupOperator.OR);
            effectiveAfterDateCondition.NullCondition(tableAlias, ColEED);

            var dateAndCondition = effectiveAfterDateCondition.ChildConditionGroup();
            dateAndCondition.NotEqualsCondition(tableAlias, ColEED).Column(tableAlias, colBED);
            dateAndCondition.GreaterThanCondition(tableAlias, ColEED).Value(effectiveOn);
        }

        public static void SetParentCodeCondition(RDBConditionContext conditionContext, string code, string tableAlias, string codeColumnName)
        {
            var compareCondition = conditionContext.CompareCondition(RDBCompareConditionOperator.StartWith);
            compareCondition.Expression1().Value(code);
            compareCondition.Expression2().Column(tableAlias, codeColumnName);
        }

        public static void SetDateCondition(RDBConditionContext context, string tableAlias, string colBED, string ColEED, bool isFuture, DateTime? effectiveOn)
        {
            if (isFuture)
                SetFutureDateCondition(context, tableAlias, colBED, ColEED, DateTime.Now);
            else
            {
                if (effectiveOn.HasValue)
                    SetEffectiveDateCondition(context, tableAlias, colBED, ColEED, effectiveOn.Value);
                else
                    context.FalseCondition();
            }

        }

        public static void SetCodePrefixQuery(RDBQueryContext queryContext, string tableName, string tableAlias, string BEDColumnName, string EEDColumnName, bool isEffectiveInTheFuture,
            DateTime? effectiveDate, int prefixLength, string codeColumnName, IEnumerable<string> codePrefixes)
        {
            string codePrefixAlias = "CodePrefix";
            string codeCountAlias = "CodeCount";

            var allPrefixesTempTableQuery = queryContext.CreateTempTable();
            allPrefixesTempTableQuery.AddColumn(codePrefixAlias, RDBDataType.Varchar);
            allPrefixesTempTableQuery.AddColumn(codeCountAlias, RDBDataType.Int);

            var insertToTempTableQuery = queryContext.AddInsertQuery();
            insertToTempTableQuery.IntoTable(allPrefixesTempTableQuery);

            var fromSelectQuery = insertToTempTableQuery.FromSelect();
            fromSelectQuery.From(tableName, tableAlias, null, true);

            var whereContext = fromSelectQuery.Where();
            SetDateCondition(whereContext, tableAlias, BEDColumnName, EEDColumnName, isEffectiveInTheFuture, effectiveDate);

            var groupBy = fromSelectQuery.GroupBy();
            groupBy.SelectAggregates().Count(codeCountAlias);
            var groupSelectContext = groupBy.Select();
            groupSelectContext.Expression(codePrefixAlias).TextLeftPart(prefixLength).Column(tableAlias, codeColumnName);

            var codePrefixesTempTableQuery = queryContext.CreateTempTable();
            codePrefixesTempTableQuery.AddColumn(codePrefixAlias, RDBDataType.Varchar);
            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(codePrefixesTempTableQuery);

            foreach (var queryItem in codePrefixes)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(codePrefixAlias).Value(queryItem);
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(allPrefixesTempTableQuery, "allPrefixes", null);
            selectQuery.SelectColumns().AllTableColumns("allPrefixes");
            var joinContext = selectQuery.Join();
            var joinStatement = joinContext.Join(codePrefixesTempTableQuery, "cp");
            var onStatement = joinStatement.On();
            onStatement.EqualsCondition("cp", codePrefixAlias).TextLeftPart(prefixLength - 1).Column("allPrefixes", codePrefixAlias);
        }

        public static void SetDistinctCodePrefixesQuery(RDBQueryContext queryContext, string tableName, string tableAlias, string BEDColumnName, string EEDColumnName, bool isEffectiveInTheFuture,
            DateTime? effectiveDate, int prefixLength, string codeColumnName)
        {
            var selectQuery = queryContext.AddSelectQuery();
            string codePrefixAlias = "CodePrefix";
            string codeCountAlias = "CodeCount";

            selectQuery.From(tableName, tableAlias, null, true);

            var whereContext = selectQuery.Where();
            SetDateCondition(whereContext, tableAlias, BEDColumnName, EEDColumnName, isEffectiveInTheFuture, effectiveDate);

            var groupByContext = selectQuery.GroupBy();
            var groupSelect = groupByContext.Select();
            groupSelect.Expression(codePrefixAlias).TextLeftPart(prefixLength).Column(codeColumnName);
            groupByContext.SelectAggregates().Count(codeCountAlias);

            selectQuery.Sort().ByAlias(codeCountAlias, RDBSortDirection.DESC);
        }
    }
}
