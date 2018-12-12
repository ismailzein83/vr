using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;

namespace Vanrise.Analytic.Data.RDB
{
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

        public IEnumerable<DBAnalyticRecord> GetAnalyticRecords(IAnalyticDataManagerGetAnalyticRecordsContext context)
        {
            var query = context.Query;
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(_tableName, "ant", null, true);
            RDBGroupByContext groupBy = null;
            List<QueryDimensionInfo> dimensionInfos = new List<QueryDimensionInfo>();
            List<QueryAggregateInfo> aggregateInfos = new List<QueryAggregateInfo>();

            if (context.AllQueryDBDimensions != null && context.AllQueryDBDimensions.Count > 0)
            {
                groupBy = selectQuery.GroupBy();
                var groupByColumns = groupBy.Select();
                foreach (var dimName in context.AllQueryDBDimensions)
                {
                    var dimConfig = GetDimensionConfigWithValidate(dimName);
                    dimConfig.DimensionConfig.Config.SQLExpression.ThrowIfNull("dimensionConfig.Config.SQLExpression", dimName);
                    string dimAlias = $"{dimName}_{dimConfig.DimensionConfig.AnalyticDimensionConfigId}";
                    var dimInfo = new QueryDimensionInfo
                    {
                        DimensionName = dimName,
                        Alias = dimAlias,
                        DimensionConfig = dimConfig
                    };
                    dimensionInfos.Add(dimInfo);
                    if (dimConfig.RDBExpressionSetter != null)
                        dimConfig.RDBExpressionSetter.SetExpression(new AnalyticItemRDBExpressionSetterContext(groupByColumns.Expression(dimAlias)));
                    else
                        groupByColumns.Column(dimConfig.DimensionConfig.Config.SQLExpression, dimAlias);
                }
            }

            if (context.AllQueryDBAggregates != null && context.AllQueryDBAggregates.Count > 0)
            {
                var selectAggregates = groupBy != null ? groupBy.SelectAggregates() : selectQuery.SelectAggregates();                
                foreach (var aggName in context.AllQueryDBAggregates)
                {
                    var aggConfig = GetAggregateConfigWithValidate(aggName);
                    aggConfig.AggregateConfig.Config.SQLColumn.ThrowIfNull("aggregateConfig.Config.SQLColumn", aggName);
                    string aggAlias = $"{aggName}_{aggConfig.AggregateConfig.AnalyticAggregateConfigId}";
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
                        switch(aggConfig.AggregateConfig.Config.AggregateType)
                        {
                            case AnalyticAggregateType.Sum: aggregateType = RDBNonCountAggregateType.SUM;break;
                            case AnalyticAggregateType.Max: aggregateType = RDBNonCountAggregateType.MAX; break;
                            case AnalyticAggregateType.Min: aggregateType = RDBNonCountAggregateType.MIN; break;
                            default:throw new NotSupportedException($"aggConfig.Config.AggregateType '{aggConfig.AggregateConfig.Config.AggregateType.ToString()}'");
                        }
                        if (aggConfig.RDBExpressionSetter != null)
                            aggConfig.RDBExpressionSetter.SetExpression(new AnalyticItemRDBExpressionSetterContext(selectAggregates.ExpressionAggregate(aggregateType, aggAlias)));
                        else
                            selectAggregates.Aggregate(aggregateType, aggConfig.AggregateConfig.Config.SQLColumn, aggAlias);
                    }
                }
            }

            var where = selectQuery.Where();
            string timeColumnName = GetTableWithValidate().Settings.TimeColumnName;
            where.GreaterOrEqualCondition(timeColumnName).Value(query.FromTime);
            if (query.ToTime.HasValue)
                where.LessOrEqualCondition(timeColumnName).Value(query.ToTime.Value);

            return queryContext.GetItems(reader => DBAnalyticRecordMapper(reader, dimensionInfos, aggregateInfos));
        }



        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            var analyticTable = GetTableWithValidate();
            analyticTable.Settings.ThrowIfNull("analyticTable.Settings", analyticTable.AnalyticTableId);
            if (!String.IsNullOrWhiteSpace(analyticTable.Settings.ConnectionString))
                return RDBDataProviderFactory.CreateProviderFromConnString(_moduleName, analyticTable.Settings.ConnectionString);
            else
                return RDBDataProviderFactory.CreateProvider(_moduleName, analyticTable.Settings.ConnectionStringName);
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

        private DBAnalyticRecord DBAnalyticRecordMapper(IRDBDataReader reader, List<QueryDimensionInfo> dimensionInfos, List<QueryAggregateInfo> aggregateInfos)
        {
            DBAnalyticRecord record = new DBAnalyticRecord { GroupingValuesByDimensionName = new Dictionary<string, DBAnalyticRecordGroupingValue>(), AggValuesByAggName = new Dictionary<string, DBAnalyticRecordAggValue>() };
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
