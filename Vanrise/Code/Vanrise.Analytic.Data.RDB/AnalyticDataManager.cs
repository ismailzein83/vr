using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Data.RDB;
using Vanrise.Entities;
using Vanrise.Common.Business;

namespace Vanrise.Analytic.Data.RDB
{
    /// <summary>
    /// TODO: this class should be moved to Vanrise.Common.Data.RDB
    /// </summary>
    public class CurrencyExchangeRateWithEEDDataManager
    {
        public static string TABLE_NAME = "VR_Common_CurrencyExchangeRateWithEED";
        static string TABLE_ALIAS = "curExchRate";

        const string COL_CurrencyID = "CurrencyID";
        const string COL_Rate = "Rate";
        const string COL_BED = "BED";
        const string COL_EED = "EED";

        static CurrencyExchangeRateWithEEDDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_CurrencyID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Rate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size =  18, Precision = 6});
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBTableName = "CurrencyExchangeRate",
                Columns = columns,
                IdColumnName = COL_CurrencyID
            });
        }

        public void SetSelectQueryForExchangeRatesConvertedToCurrency(RDBSelectQuery selectQuery, int currencyId, DateTime fromTime, DateTime? toTime)
        {
            string exRate1Alias = "exRate1";
            string exRate2Alias = "exRate2";
            selectQuery.From(TABLE_NAME, exRate1Alias);

            var joinStatement = selectQuery.Join().Join(TABLE_NAME, exRate2Alias);
            var joinCondition = joinStatement.On();

            joinCondition.EqualsCondition(exRate2Alias, COL_CurrencyID).Value(currencyId);

            var joinSubCondition1 = joinCondition.ChildConditionGroup(RDBConditionGroupOperator.OR);

            var joinSubCondition11 = joinSubCondition1.ChildConditionGroup(RDBConditionGroupOperator.AND);
            joinSubCondition11.GreaterOrEqualCondition(exRate1Alias, COL_BED).Column(exRate2Alias, COL_BED);
            joinSubCondition11.ConditionIfColumnNotNull(exRate2Alias, COL_EED).GreaterThanCondition(exRate2Alias, COL_EED).Column(exRate1Alias, COL_BED);

            var joinSubCondition12 = joinSubCondition1.ChildConditionGroup(RDBConditionGroupOperator.AND);
            joinSubCondition12.LessThanCondition(exRate1Alias, COL_BED).Column(exRate2Alias, COL_BED);
            joinSubCondition12.ConditionIfColumnNotNull(exRate1Alias, COL_EED).GreaterThanCondition(exRate1Alias, COL_EED).Column(exRate2Alias, COL_BED);

            var selectColumns = selectQuery.SelectColumns();

            selectColumns.Column(COL_CurrencyID);

            var rateExp = selectColumns.Expression(COL_Rate).ArithmeticExpression(RDBArithmeticExpressionOperator.Divide);
            rateExp.Expression1().Column(exRate1Alias, COL_Rate);
            rateExp.Expression2().Column(exRate2Alias, COL_Rate);

            var bedExp = selectColumns.Expression(COL_BED).CaseExpression();
            var bedExpCase1 = bedExp.AddCase();
            bedExpCase1.When().GreaterOrEqualCondition(exRate1Alias, COL_BED).Column(exRate2Alias, COL_BED);
            bedExpCase1.Then().Column(exRate1Alias, COL_BED);
            bedExp.Else().Column(exRate2Alias, COL_BED);

            var eedExp = selectColumns.Expression(COL_EED).CaseExpression();
            var eedExpCase1 = eedExp.AddCase();
            eedExpCase1.When().NullCondition(exRate1Alias, COL_EED);
            eedExpCase1.Then().Column(exRate2Alias, COL_EED);
            var eedExpCase2 = eedExp.AddCase();
            eedExpCase2.When().NullCondition(exRate2Alias, COL_EED);
            eedExpCase2.Then().Column(exRate1Alias, COL_EED);
            var eedExpCase3 = eedExp.AddCase();
            eedExpCase3.When().GreaterThanCondition(exRate1Alias, COL_EED).Column(exRate2Alias, COL_EED);
            eedExpCase3.Then().Column(exRate2Alias, COL_EED);
            eedExp.Else().Column(exRate1Alias, COL_EED);

            var where = selectQuery.Where();
            where.ConditionIfColumnNotNull(exRate1Alias, COL_EED).GreaterThanCondition(exRate1Alias, COL_EED).Value(fromTime);
            where.ConditionIfColumnNotNull(exRate2Alias, COL_EED).GreaterThanCondition(exRate2Alias, COL_EED).Value(fromTime);
        }

        public void AddJoinExchangeRates(RDBJoinContext joinContext, RDBTempTableQuery exchangeRatesConvertedToCurrency, string exchRateTableAlias, string otherTableAlias, string otherTableCurrencyColumnName, string otherTableTimeColumnName)
        {
            RDBJoinStatementContext joinStatement;
            if (exchangeRatesConvertedToCurrency != null)
                joinStatement = joinContext.Join(exchangeRatesConvertedToCurrency, exchRateTableAlias);
            else
                joinStatement = joinContext.Join(TABLE_NAME, exchRateTableAlias);
            joinStatement.JoinType(RDBJoinType.Left);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(exchRateTableAlias, COL_CurrencyID).Column(otherTableAlias, otherTableCurrencyColumnName);
            joinCondition.GreaterOrEqualCondition(otherTableAlias, otherTableTimeColumnName).Column(exchRateTableAlias, COL_BED);
            joinCondition.ConditionIfColumnNotNull(exchRateTableAlias, COL_EED).LessThanCondition(otherTableAlias, otherTableTimeColumnName).Column(exchRateTableAlias, COL_EED);
        }

        public RDBExpressionContext CreateConvertedRateExpressionContext(RDBExpressionContext expressionContext, string exchRateTableAlias)
        {
            var arithmeticExp = expressionContext.ArithmeticExpression(RDBArithmeticExpressionOperator.Divide);
            var divideByRateExp = arithmeticExp.Expression2().CaseExpression();
            var divideByRateExpCase1 = divideByRateExp.AddCase();
            divideByRateExpCase1.When().NotNullCondition(exchRateTableAlias, COL_Rate);
            divideByRateExpCase1.Then().Column(exchRateTableAlias, COL_Rate);
            divideByRateExp.Else().Value(1);

            return arithmeticExp.Expression1();
        }
    }
    public class AnalyticDataManager : IAnalyticDataManager
    {
        IAnalyticTableQueryContext _analyticTableQueryContext;
        public IAnalyticTableQueryContext AnalyticTableQueryContext
        {
            set { _analyticTableQueryContext = value; }
        }

        ResolvedConfigs _resolvedConfigs;
        internal ResolvedConfigs ResolvedConfigs
        {
            set
            {
                _resolvedConfigs = value;
            }
        }

        string _tableName;

        public string TableName
        {
            set
            {
                _tableName = value;
            }
        }

        string _moduleName;

        public string ModuleName
        {
            set
            {
                _moduleName = value;
            }
        }

        static CurrencyExchangeRateWithEEDDataManager s_currencyExchangeRateWithEEDDataManager = new CurrencyExchangeRateWithEEDDataManager();
        public IEnumerable<DBAnalyticRecord> GetAnalyticRecords(IAnalyticDataManagerGetAnalyticRecordsContext context)
        {
            var query = context.Query;
            var queryContext = new RDBQueryContext(GetDataProvider());
            var allQueryDBDimensions = context.AllQueryDBDimensions;
            var allQueryDBAggregates = context.AllQueryDBAggregates;
            var allQueryDBFilters = context.DBFilters;
            var queryDBFilterGroup = context.DBFilterGroup;

            HashSet<string> includeJoinConfigNames = new HashSet<string>();

            Dictionary<string, string> currencyFieldNamesWithAlias;
            RDBTempTableQuery currencyExchRateTempTable;
            PrepareCurrencyExchRatesForQuery(queryContext, query, allQueryDBDimensions, allQueryDBAggregates, out currencyFieldNamesWithAlias, out currencyExchRateTempTable);

            string tableAlias = "ant";
            string timeColumnName = GetTableWithValidate().Settings.TimeColumnName;
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(_tableName, tableAlias, null, true);
            AddJoinCurrencyExchRate(currencyFieldNamesWithAlias, selectQuery, currencyExchRateTempTable, tableAlias, timeColumnName);

            List<QueryDimensionInfo> dimensionInfos = new List<QueryDimensionInfo>();
            List<QueryAggregateInfo> aggregateInfos = new List<QueryAggregateInfo>();

            RDBGroupByContext groupBy = AddDimensions(allQueryDBDimensions, selectQuery, dimensionInfos, query.TimeGroupingUnit, 
                timeColumnName, includeJoinConfigNames, currencyFieldNamesWithAlias);
            AddAggregates(allQueryDBAggregates, selectQuery, groupBy, aggregateInfos, includeJoinConfigNames, currencyFieldNamesWithAlias);


            var where = selectQuery.Where();
            where.GreaterOrEqualCondition(timeColumnName).Value(query.FromTime);
            if (query.ToTime.HasValue)
                where.LessOrEqualCondition(timeColumnName).Value(query.ToTime.Value);

            AddFilters(where, allQueryDBFilters);
            AddFilterGroup(where, queryDBFilterGroup);

            if (includeJoinConfigNames.Count > 0)
                AddJoins(selectQuery.Join(), includeJoinConfigNames);

            return queryContext.GetItems(reader => DBAnalyticRecordMapper(reader, query.TimeGroupingUnit, dimensionInfos, aggregateInfos));
        }

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            var analyticTable = GetTableWithValidate();
            analyticTable.Settings.ThrowIfNull("analyticTable.Settings", analyticTable.AnalyticTableId);
            if (!String.IsNullOrWhiteSpace(analyticTable.Settings.ConnectionString))
                return RDBDataProviderFactory.CreateProviderFromConnString(_moduleName, analyticTable.Settings.ConnectionString);
            else
                return RDBDataProviderFactory.CreateProvider(_moduleName, analyticTable.Settings.ConnectionStringAppSettingName,analyticTable.Settings.ConnectionStringName);
        }

        AnalyticTable GetTableWithValidate()
        {
            return GetQueryContextWithValidate().GetTable();
        }

        ResolvedAnalyticDimensionConfig GetDimensionConfigWithValidate(string dimensionName)
        {
            ResolvedAnalyticDimensionConfig resolvedAnalyticDimensionConfig;
            if (!GetResolvedConfigsWithValidate().DimensionConfigs.TryGetValue(dimensionName, out resolvedAnalyticDimensionConfig))
                throw new Exception($"ResolvedAnalyticDimensionConfig '{dimensionName}' not found");
            return resolvedAnalyticDimensionConfig;
        }

        ResolvedAnalyticAggregateConfig GetAggregateConfigWithValidate(string aggregateName)
        {
            ResolvedAnalyticAggregateConfig resolvedAnalyticAggregateConfig;
            if (!GetResolvedConfigsWithValidate().AggregateConfigs.TryGetValue(aggregateName, out resolvedAnalyticAggregateConfig))
                throw new Exception($"ResolvedAnalyticAggregateConfig '{aggregateName}' not found");
            return resolvedAnalyticAggregateConfig;
        }

        ResolvedAnalyticJoinConfig GetJoinConfigWithValidate(string joinName)
        {
            ResolvedAnalyticJoinConfig resolvedAnalyticJoinConfig;
            if (!GetResolvedConfigsWithValidate().JoinConfigs.TryGetValue(joinName, out resolvedAnalyticJoinConfig))
                throw new Exception($"ResolvedAnalyticJoinConfig '{joinName}' not found");
            return resolvedAnalyticJoinConfig;
        }

        private IAnalyticTableQueryContext GetQueryContextWithValidate()
        {
            _analyticTableQueryContext.ThrowIfNull("_analyticTableQueryContext");
            return _analyticTableQueryContext;
        }

        private ResolvedConfigs GetResolvedConfigsWithValidate()
        {
            _resolvedConfigs.ThrowIfNull("_resolvedConfigs");
            return _resolvedConfigs;
        }

        private void PrepareCurrencyExchRatesForQuery(RDBQueryContext queryContext, AnalyticQuery query, HashSet<string> allQueryDBDimensions, HashSet<string> allQueryDBAggregates, out Dictionary<string, string> currencyFieldNamesWithAlias, out RDBTempTableQuery currencyExchRateTempTable)
        {
            currencyFieldNamesWithAlias = new Dictionary<string, string>();
            if (allQueryDBDimensions != null)
            {
                foreach (var dimName in allQueryDBDimensions)
                {
                    var dimConfig = GetDimensionConfigWithValidate(dimName);
                    var currencyColumnName = dimConfig.DimensionConfig.Config.CurrencySQLColumnName;
                    if (!String.IsNullOrWhiteSpace(currencyColumnName) && !currencyFieldNamesWithAlias.ContainsKey(currencyColumnName))
                        currencyFieldNamesWithAlias.Add(currencyColumnName, $"{currencyColumnName}_ExRates");
                }
            }
            if (allQueryDBAggregates != null)
            {
                foreach (var aggName in allQueryDBAggregates)
                {
                    var aggConfig = GetAggregateConfigWithValidate(aggName);
                    var currencyColumnName = aggConfig.AggregateConfig.Config.CurrencySQLColumnName;
                    if (!String.IsNullOrWhiteSpace(currencyColumnName) && !currencyFieldNamesWithAlias.ContainsKey(currencyColumnName))
                        currencyFieldNamesWithAlias.Add(currencyColumnName, $"{currencyColumnName}_ExRates");
                }
            }

            currencyExchRateTempTable = null;
            if (currencyFieldNamesWithAlias.Count > 0)
            {
                int systemCurrencyId = new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId();
                if (query.CurrencyId.HasValue && query.CurrencyId.Value != systemCurrencyId)
                {
                    currencyExchRateTempTable = queryContext.CreateTempTable();
                    currencyExchRateTempTable.AddColumnsFromTable(CurrencyExchangeRateWithEEDDataManager.TABLE_NAME);

                    var insertExchRateQuery = queryContext.AddInsertQuery();
                    insertExchRateQuery.IntoTable(currencyExchRateTempTable);
                    var selectConvertedExchangeRateQuery = insertExchRateQuery.FromSelect();
                    s_currencyExchangeRateWithEEDDataManager.SetSelectQueryForExchangeRatesConvertedToCurrency(selectConvertedExchangeRateQuery, query.CurrencyId.Value, query.FromTime, query.ToTime);
                }
            }
        }

        private void AddJoinCurrencyExchRate(Dictionary<string, string> currencyFieldNamesWithAlias, RDBSelectQuery selectQuery, RDBTempTableQuery currencyExchRateTempTable, string tableAlias, string timeColumnName)
        {
            if (currencyFieldNamesWithAlias.Count > 0)
            {
                var join = selectQuery.Join();
                foreach (var currencyFieldNameEntry in currencyFieldNamesWithAlias)
                {
                    s_currencyExchangeRateWithEEDDataManager.AddJoinExchangeRates(join, currencyExchRateTempTable,
                        currencyFieldNameEntry.Value, tableAlias, currencyFieldNameEntry.Key, timeColumnName);
                }
            }
        }

        private RDBGroupByContext AddDimensions(HashSet<string> allQueryDBDimensions, RDBSelectQuery selectQuery, 
            List<QueryDimensionInfo> dimensionInfos, TimeGroupingUnit? timeGroupingUnit, string timeColumnName,
            HashSet<string> includeJoinConfigNames, Dictionary<string, string> currencyFieldNamesWithAlias)
        {
            RDBGroupByContext groupBy = null;
            RDBSelectColumnsContext groupByColumns = null;
            if (allQueryDBDimensions != null && allQueryDBDimensions.Count > 0)
            {
                groupBy = selectQuery.GroupBy();
                groupByColumns = groupBy.Select();
                foreach (var dimName in allQueryDBDimensions)
                {
                    var dimConfig = GetDimensionConfigWithValidate(dimName);
                    string dimAlias = $"{dimName}_{dimConfig.DimensionConfig.AnalyticDimensionConfigId.ToString().Replace("-", "")}";
                    var dimInfo = new QueryDimensionInfo
                    {
                        DimensionName = dimName,
                        Alias = dimAlias,
                        DimensionConfig = dimConfig
                    };
                    dimensionInfos.Add(dimInfo);
                    dimConfig.RDBExpressionSetter.ThrowIfNull("dimConfig.RDBExpressionSetter", dimConfig.DimensionConfig.AnalyticDimensionConfigId);
                    var expToSet = groupByColumns.Expression(dimAlias);
                    if (!String.IsNullOrWhiteSpace(dimConfig.DimensionConfig.Config.CurrencySQLColumnName))
                    {
                        expToSet = expToSet.ConvertDecimal();
                        string exchRateTableAlias = currencyFieldNamesWithAlias[dimConfig.DimensionConfig.Config.CurrencySQLColumnName];
                        expToSet = s_currencyExchangeRateWithEEDDataManager.CreateConvertedRateExpressionContext(expToSet, exchRateTableAlias);
                    }
                    dimConfig.RDBExpressionSetter.SetExpression(new AnalyticItemRDBExpressionSetterContext(expToSet));

                    if(dimConfig.DimensionConfig.Config.JoinConfigNames != null)
                    {
                        foreach (var join in dimConfig.DimensionConfig.Config.JoinConfigNames)
                        {
                            includeJoinConfigNames.Add(join);
                        }
                    }
                }
            }

            if (timeGroupingUnit.HasValue)
            {
                if (groupByColumns == null)
                {
                    groupBy = selectQuery.GroupBy();
                    groupByColumns = groupBy.Select();
                }
                RDBDateTimePart rdbDateTimePart;
                switch (timeGroupingUnit.Value)
                {
                    case TimeGroupingUnit.Day: rdbDateTimePart = RDBDateTimePart.DateOnly; break;
                    case TimeGroupingUnit.Hour: rdbDateTimePart = RDBDateTimePart.DateAndHour; break;
                    default: throw new NotSupportedException($"timeGroupingUnit '{timeGroupingUnit.Value.ToString()}'");
                }
                groupByColumns.Expression("Date").DateTimePart(rdbDateTimePart).Column(timeColumnName);
            }
            
            return groupBy;
        }

        private void AddAggregates(HashSet<string> allQueryDBAggregates, RDBSelectQuery selectQuery, RDBGroupByContext groupBy, List<QueryAggregateInfo> aggregateInfos, HashSet<string> includeJoinConfigNames, Dictionary<string, string> currencyFieldNamesWithAlias)
        {
            if (allQueryDBAggregates != null && allQueryDBAggregates.Count > 0)
            {
                var selectAggregates = groupBy != null ? groupBy.SelectAggregates() : selectQuery.SelectAggregates();
                foreach (var aggName in allQueryDBAggregates)
                {
                    var aggConfig = GetAggregateConfigWithValidate(aggName);
                    string aggAlias = $"{aggName}_{aggConfig.AggregateConfig.AnalyticAggregateConfigId.ToString().Replace("-", "")}";
                    var aggInfo = new QueryAggregateInfo
                    {
                        AggregateName = aggName,
                        Alias = aggAlias,
                        AggregateConfig = aggConfig
                    };
                    aggregateInfos.Add(aggInfo);
                    if (aggConfig.AggregateConfig.Config.AggregateType == AnalyticAggregateType.Count)
                    {
                        selectAggregates.Count(aggAlias);
                    }
                    else
                    {
                        RDBNonCountAggregateType aggregateType;
                        switch (aggConfig.AggregateConfig.Config.AggregateType)
                        {
                            case AnalyticAggregateType.Sum: aggregateType = RDBNonCountAggregateType.SUM; break;
                            case AnalyticAggregateType.Max: aggregateType = RDBNonCountAggregateType.MAX; break;
                            case AnalyticAggregateType.Min: aggregateType = RDBNonCountAggregateType.MIN; break;
                            default: throw new NotSupportedException($"aggConfig.Config.AggregateType '{aggConfig.AggregateConfig.Config.AggregateType.ToString()}'");
                        }
                        aggConfig.RDBExpressionSetter.ThrowIfNull("aggConfig.RDBExpressionSetter", aggConfig.AggregateConfig.AnalyticAggregateConfigId);
                        var aggExpressionContext = selectAggregates.Expression(aggAlias);
                        if (!String.IsNullOrWhiteSpace(aggConfig.AggregateConfig.Config.CurrencySQLColumnName))
                            aggExpressionContext = aggExpressionContext.ConvertDecimal();
                        var expToSet = aggExpressionContext.ExpressionAggregate(aggregateType);
                        if (!String.IsNullOrWhiteSpace(aggConfig.AggregateConfig.Config.CurrencySQLColumnName))
                        {
                            string exchRateTableAlias = currencyFieldNamesWithAlias[aggConfig.AggregateConfig.Config.CurrencySQLColumnName];
                            expToSet = s_currencyExchangeRateWithEEDDataManager.CreateConvertedRateExpressionContext(expToSet, exchRateTableAlias);
                        }
                        aggConfig.RDBExpressionSetter.SetExpression(new AnalyticItemRDBExpressionSetterContext(expToSet));
                    }

                    if (aggConfig.AggregateConfig.Config.JoinConfigNames != null)
                    {
                        foreach (var join in aggConfig.AggregateConfig.Config.JoinConfigNames)
                        {
                            includeJoinConfigNames.Add(join);
                        }
                    }
                }
            }
        }

        private void AddFilters(RDBConditionContext where, List<DimensionFilter> allQueryDBFilters)
        {
            if (allQueryDBFilters != null)
            {
                foreach (var dbFilter in allQueryDBFilters)
                {
                    if (dbFilter.FilterValues != null && dbFilter.FilterValues.Count() > 0)
                    {
                        bool hasNullValue = false;
                        List<string> parameterNames = new List<string>();
                        List<BaseRDBExpression> filterValueExpressions = new List<BaseRDBExpression>();
                        var expressionContext = where.CreateExpressionContext((exp) => filterValueExpressions.Add(exp));
                        foreach (var filterValue in dbFilter.FilterValues)
                        {
                            if (filterValue == null)
                                hasNullValue = true;
                            else
                                expressionContext.ObjectValue(filterValue);
                        }

                        var conditionContext = where;
                        if (filterValueExpressions.Count > 0 && hasNullValue)
                            conditionContext = where.ChildConditionGroup(RDBConditionGroupOperator.OR);

                        var dimConfig = GetDimensionConfigWithValidate(dbFilter.Dimension);
                        dimConfig.RDBExpressionSetter.ThrowIfNull("dimConfig.RDBExpressionSetter", dimConfig.DimensionConfig.AnalyticDimensionConfigId);

                        if (filterValueExpressions.Count > 0)
                            dimConfig.RDBExpressionSetter.SetExpression(new AnalyticItemRDBExpressionSetterContext(conditionContext.ListCondition(RDBListConditionOperator.IN, filterValueExpressions)));

                        if (hasNullValue)
                            dimConfig.RDBExpressionSetter.SetExpression(new AnalyticItemRDBExpressionSetterContext(conditionContext.NullCondition()));
                    }
                }
            }
        }

        private void AddFilterGroup(RDBConditionContext where, GenericData.Entities.RecordFilterGroup queryDBFilterGroup)
        {
            if (queryDBFilterGroup != null)
            {
                var recordFilterRDBBuilder = new RecordFilterRDBBuilder(
                                    (fieldName, expressionContext) =>
                                    {
                                        var dimConfig = GetDimensionConfigWithValidate(fieldName);
                                        dimConfig.RDBExpressionSetter.ThrowIfNull("dimConfig.RDBExpressionSetter", dimConfig.DimensionConfig.AnalyticDimensionConfigId);
                                        dimConfig.RDBExpressionSetter.SetExpression(new AnalyticItemRDBExpressionSetterContext(expressionContext));
                                    });
                recordFilterRDBBuilder.RecordFilterGroupCondition(where, queryDBFilterGroup);
            }
        }

        private void AddJoins(RDBJoinContext joinContext, HashSet<string> includeJoinConfigNames)
        {
            foreach(var joinName in includeJoinConfigNames)
            {
                var joinConfig = GetJoinConfigWithValidate(joinName);
                joinConfig.JoinRDBExpressionSetter.ThrowIfNull("joinConfig.JoinRDBExpressionSetter", joinConfig.JoinName);
                joinConfig.JoinRDBExpressionSetter.SetExpression(new AnalyticJoinRDBExpressionSetterContext(joinContext));
            }
        }

        private DBAnalyticRecord DBAnalyticRecordMapper(IRDBDataReader reader, TimeGroupingUnit? timeGroupingUnit, List<QueryDimensionInfo> dimensionInfos, List<QueryAggregateInfo> aggregateInfos)
        {
            DBAnalyticRecord record = new DBAnalyticRecord { GroupingValuesByDimensionName = new Dictionary<string, DBAnalyticRecordGroupingValue>(), AggValuesByAggName = new Dictionary<string, DBAnalyticRecordAggValue>() };
            if(timeGroupingUnit.HasValue)
            {
                record.Time = reader.GetNullableDateTime("Date");
            }
            if(dimensionInfos != null)
            {
                foreach(var dimInfo in dimensionInfos)
                {
                    var value = dimInfo.DimensionConfig.ReaderValueGetter.GetReaderValue(reader, dimInfo.Alias);
                    record.GroupingValuesByDimensionName.Add(dimInfo.DimensionName, new DBAnalyticRecordGroupingValue { Value = value });
                }
            }
            if(aggregateInfos != null)
            {
                foreach(var aggInfo in aggregateInfos)
                {
                    var value = aggInfo.AggregateConfig.ReaderValueGetter.GetReaderValue(reader, aggInfo.Alias);
                    record.AggValuesByAggName.Add(aggInfo.AggregateName, new DBAnalyticRecordAggValue { Value = value });
                }
            }
            return record;
        }

        #endregion

        #region Private Classes

        private class QueryDimensionInfo
        {
            public string DimensionName { get; set; }

            public string Alias { get; set; }

            public ResolvedAnalyticDimensionConfig DimensionConfig { get; set; }
        }

        private class QueryAggregateInfo
        {
            public string AggregateName { get; set; }

            public string Alias { get; set; }

            public ResolvedAnalyticAggregateConfig AggregateConfig { get; set; }
        }

        #endregion
    }
}
