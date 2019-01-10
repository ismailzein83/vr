using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;
using Vanrise.RDBTests.Common;
using Vanrise.Entities;
namespace Vanrise.RDB.Tests.GenericData
{
    [TestClass]
    public class RecordFilterRDBBuilderTests
    {
        static string TABLE_NAME = "TestRecordFilterTable";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Amount = "Amount";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_IsDeleted = "IsDeleted";
        static RecordFilterRDBBuilderTests()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 450 });
            columns.Add(COL_Amount, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 30, Precision = 8 });
            columns.Add(COL_IsDeleted, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TestSchema",
                DBTableName = "TestRecordFilterTable",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });

            foreach(var col in columns)
            {
                s_fieldInfos.Add(col.Key, new FieldInfo
                {
                    SQLExpression = $@"""tbl"".""{col.Key}""",
                    SetRDBExpression = (expContext) => expContext.Column(col.Key)
                });
            }

            Action<RDBExpressionContext> setAmountIfNotDeletedExpression = (expContext) =>
            {
                var caseContext = expContext.CaseExpression();
                var case1 = caseContext.AddCase();
                case1.When().EqualsCondition(COL_IsDeleted).Value(false);
                case1.Then().Column(COL_Amount);
                caseContext.Else().Value(0);
            };
            s_fieldInfos.Add("AmountIfNotDeleted", new FieldInfo
            {
                SetRDBExpression = setAmountIfNotDeletedExpression,
                SQLExpression = $@"( CASE WHEN ""tbl"".""{COL_IsDeleted}"" = 0 THEN ""tbl"".""{COL_Amount}"" ELSE 0 END )"
            });
        }

        static Dictionary<string, FieldInfo> s_fieldInfos = new Dictionary<string, FieldInfo>();

        [TestMethod]
        public void TestRecordFilterGroup()
        {
            TestRecordFilter(new NumberRecordFilter { FieldName = "AmountIfNotDeleted", CompareOperator = NumberRecordFilterOperator.Greater, Value = 34 });
            TestRecordFilter(new EmptyRecordFilter { FieldName = COL_Name });
            TestRecordFilter(new NonEmptyRecordFilter { FieldName = COL_Name });
            TestRecordFilter(new AlwaysTrueRecordFilter { FieldName = COL_Name });
            TestRecordFilter(new AlwaysFalseRecordFilter { FieldName = COL_Name });
            UTUtilities.CallActionIteratively(
                (compareOperator) =>
                {
                    var filter = new StringRecordFilter { FieldName = COL_Name, CompareOperator = compareOperator, Value = "tfff" };
                    TestRecordFilter(filter);
                }, UTUtilities.GetEnumListForTesting<StringRecordFilterOperator>());
            UTUtilities.CallActionIteratively(
                (compareOperator) =>
                {
                    var filter = new NumberRecordFilter { FieldName = COL_Amount, CompareOperator = compareOperator, Value = 457.5M };
                    TestRecordFilter(filter);
                }, UTUtilities.GetEnumListForTesting<NumberRecordFilterOperator>());

            UTUtilities.CallActionIteratively(
                (filterValue) =>
                {
                    var filter = new BooleanRecordFilter { FieldName = COL_IsDeleted, IsTrue = filterValue};
                    TestRecordFilter(filter);
                }, UTUtilities.GetBoolListForTesting());

            UTUtilities.CallActionIteratively(
                (compareOperator, comparisonPart, excludeValue2) =>
                {
                    Object value;
                    Object value2;
                    switch(comparisonPart)
                    {
                        case DateTimeRecordFilterComparisonPart.TimeOnly:
                        case DateTimeRecordFilterComparisonPart.Hour:
                            value = new Vanrise.Entities.Time(DateTime.Today);
                            value2 = new Vanrise.Entities.Time(DateTime.Now);
                            break;
                        case DateTimeRecordFilterComparisonPart.DateOnly:
                        case DateTimeRecordFilterComparisonPart.DateTime:
                        case DateTimeRecordFilterComparisonPart.YearMonth:
                        case DateTimeRecordFilterComparisonPart.YearWeek:
                            value = DateTime.Today;
                            value2 = DateTime.Now;
                            break;
                        default:
                            throw new NotSupportedException($"comparisonPart '{compareOperator.ToString()}");
                    }
                    var filter = new DateTimeRecordFilter
                    {
                        FieldName = COL_CreatedTime,
                        CompareOperator = compareOperator,
                        ComparisonPart = comparisonPart,
                        Value = value,
                        Value2 = value2,
                        ExcludeValue2 = excludeValue2
                    };
                    TestRecordFilter(filter);
                }, UTUtilities.GetEnumListForTesting<DateTimeRecordFilterOperator>(), 
                UTUtilities.GetEnumListForTesting<DateTimeRecordFilterComparisonPart>(),
                UTUtilities.GetBoolListForTesting());

            UTUtilities.CallActionIteratively(
                (listOperator) =>
                {
                    var filter = new NumberListRecordFilter
                    {
                        FieldName = COL_Amount,
                        Values = new List<decimal> { 34.5M, 53.3M, 34.5M },
                        CompareOperator = listOperator
                    };
                    TestRecordFilter(filter);
                }, UTUtilities.GetEnumListForTesting<ListRecordFilterOperator>());

            UTUtilities.CallActionIteratively(
                (listOperator) =>
                {
                    var filter = new StringListRecordFilter
                    {
                        FieldName = COL_Name,
                        Values = new List<string> { "fsd", "tre", "fsd" },
                        CompareOperator = listOperator
                    };
                    TestRecordFilter(filter);
                }, UTUtilities.GetEnumListForTesting<ListRecordFilterOperator>());

            UTUtilities.CallActionIteratively(
                (listOperator) =>
                {
                    var filter = new ObjectListRecordFilter
                    {
                        FieldName = COL_CreatedTime,
                        Values = new List<Object> { DateTime.Today, DateTime.Now },
                        CompareOperator = listOperator
                    };
                    TestRecordFilter(filter);
                }, UTUtilities.GetEnumListForTesting<ListRecordFilterOperator>());


            UTUtilities.CallActionIteratively(
                (listOperator) =>
                {
                    var filter = new ObjectListRecordFilter
                    {
                        FieldName = COL_Amount,
                        Values = new List<Object> { 34.5M, 53.3M, 34.5M },
                        CompareOperator = listOperator
                    };
                    TestRecordFilter(filter);
                }, UTUtilities.GetEnumListForTesting<ListRecordFilterOperator>());


            UTUtilities.CallActionIteratively(
                (listOperator) =>
                {
                    var filter = new ObjectListRecordFilter
                    {
                        FieldName = COL_Name,
                        Values = new List<Object> { "fsd", "tre", "fsd" },
                        CompareOperator = listOperator
                    };
                    TestRecordFilter(filter);
                }, UTUtilities.GetEnumListForTesting<ListRecordFilterOperator>());
            
            TestComplexFilterGroup();
        }

        private void TestComplexFilterGroup()
        {
            var filterGroup = new RecordFilterGroup { Filters = new List<RecordFilter>() };
            filterGroup.Filters.Add(new StringRecordFilter { FieldName = COL_Name, CompareOperator = StringRecordFilterOperator.StartsWith, Value = "fdsf" });
            filterGroup.Filters.Add(new NumberRecordFilter { FieldName = COL_Amount, CompareOperator = NumberRecordFilterOperator.GreaterOrEquals, Value = 34.4M });
            var subFilterGroup1 = new RecordFilterGroup { Filters = new List<RecordFilter>(), LogicalOperator = RecordQueryLogicalOperator.Or };
            filterGroup.Filters.Add(subFilterGroup1);
            subFilterGroup1.Filters.Add(new StringRecordFilter { FieldName = COL_Name, CompareOperator = StringRecordFilterOperator.StartsWith, Value = "fdsf" });
            subFilterGroup1.Filters.Add(new NumberRecordFilter { FieldName = COL_Amount, CompareOperator = NumberRecordFilterOperator.GreaterOrEquals, Value = 34.4M });
            var subFilterGroup2 = new RecordFilterGroup { Filters = new List<RecordFilter>(), LogicalOperator = RecordQueryLogicalOperator.Or };
            filterGroup.Filters.Add(subFilterGroup2);
            subFilterGroup2.Filters.Add(new StringRecordFilter { FieldName = COL_Name, CompareOperator = StringRecordFilterOperator.StartsWith, Value = "fdsf" });
            subFilterGroup2.Filters.Add(new NumberRecordFilter { FieldName = COL_Amount, CompareOperator = NumberRecordFilterOperator.GreaterOrEquals, Value = 34.4M });
            var subFilterGroup21 = new RecordFilterGroup { Filters = new List<RecordFilter>(), LogicalOperator = RecordQueryLogicalOperator.And };
            subFilterGroup2.Filters.Add(subFilterGroup21);
            subFilterGroup21.Filters.Add(new StringRecordFilter { FieldName = COL_Name, CompareOperator = StringRecordFilterOperator.StartsWith, Value = "fdsf" });
            subFilterGroup21.Filters.Add(new NumberRecordFilter { FieldName = COL_Amount, CompareOperator = NumberRecordFilterOperator.GreaterOrEquals, Value = 34.4M });
            var subFilterGroup22 = new RecordFilterGroup { Filters = new List<RecordFilter>(), LogicalOperator = RecordQueryLogicalOperator.Or };
            subFilterGroup2.Filters.Add(subFilterGroup22);
            subFilterGroup22.Filters.Add(new StringRecordFilter { FieldName = COL_Name, CompareOperator = StringRecordFilterOperator.StartsWith, Value = "fdsf" });
            
            AssertFilterGroupSimilarResolution(filterGroup);
        }

        private static void TestRecordFilter(RecordFilter filter)
        {
            var filterGroup = new RecordFilterGroup { Filters = new List<RecordFilter>() };
            filterGroup.Filters.Add(filter);
            AssertFilterGroupSimilarResolution(filterGroup);
        }

        private static void AssertFilterGroupSimilarResolution(RecordFilterGroup filterGroup)
        {
            var dataProvider = RDBDataProviderFactory.CreateProvider(null, Constants.CONNSTRING_NAME_CONFIG);
            var queryContext = new RDBQueryContext(dataProvider);
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "tbl");
            selectQuery.SelectColumns().Column("ID");
            var where = selectQuery.Where();
            new Vanrise.GenericData.Data.RDB.RecordFilterRDBBuilder((fldName, expContext) => s_fieldInfos[fldName].SetRDBExpression(expContext)).RecordFilterGroupCondition(where, filterGroup);

            var resolveQueryContext = new Vanrise.Data.RDB.RDBQueryGetResolvedQueryContext(dataProvider);
            StringBuilder rdbQueryBuilder = new StringBuilder(queryContext.GetResolvedQuery(resolveQueryContext).Statements[0].TextStatement);
            foreach (var prm in resolveQueryContext.Parameters.Values)
            {
                ReplaceParameterValue(rdbQueryBuilder, prm.DBParameterName, prm.Value);
            }
            rdbQueryBuilder.Replace(@"SELECT ""tbl"".""ID"" ""ID"" FROM ""TestSchema"".""TestRecordFilterTable"" ""tbl""  WHERE ", "");

            int parameterIndex = 0;
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            string filterGroupSQLString = new Vanrise.GenericData.Data.SQL.RecordFilterSQLBuilder((fldName) => s_fieldInfos[fldName].SQLExpression).BuildRecordFilter(filterGroup, ref parameterIndex, parameterValues);
            StringBuilder sqlQueryBuilder = new StringBuilder(filterGroupSQLString);
            foreach (var prm in parameterValues)
            {
                ReplaceParameterValue(sqlQueryBuilder, prm.Key, prm.Value.ToString());
            }
            var rdbQuery = rdbQueryBuilder.ToString().Trim()
                    .Replace("  ", " ").Replace("( ", "(").Replace(" )", ")");
            while (rdbQuery.StartsWith("(") && rdbQuery.EndsWith(")"))
                rdbQuery = rdbQuery.Substring(1, rdbQuery.Length - 2).Trim();
            var sqlQuery = sqlQueryBuilder.ToString()
                .Replace("and", "AND").Replace("or", "OR").Replace("And", "AND").Replace("Or", "OR")
                .Replace("0 = 1", "1 = 0")
                .Replace("  ", " ").Replace("( ", "(").Replace(" )", ")").Replace(", ", ",");
            while (sqlQuery.StartsWith("(") && sqlQuery.EndsWith(")"))
                sqlQuery = sqlQuery.Substring(1, sqlQuery.Length - 2).Trim();
            UTUtilities.AssertObjectsAreSimilar(sqlQuery, rdbQuery);
        }

        private static void ReplaceParameterValue(StringBuilder queryBuilder, string prmName, Object prmValue)
        {
            if (prmValue is bool)
                queryBuilder.Replace(prmName, (bool)prmValue ? "1" : "0");
            else
                queryBuilder.Replace(prmName, prmValue.ToString());
        }

        private class FieldInfo
        {
            public string SQLExpression { get; set; }

            public Action<RDBExpressionContext> SetRDBExpression { get; set; }
        }
    }
}
