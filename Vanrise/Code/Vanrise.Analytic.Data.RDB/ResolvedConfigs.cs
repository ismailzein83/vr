using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace Vanrise.Analytic.Data.RDB
{
    public interface IAnalyticItemRDBExpressionSetter
    {
        void SetExpression(IAnalyticItemRDBExpressionSetterContext context);
    }

    public interface IAnalyticItemRDBExpressionSetterContext
    {
        RDBExpressionContext RDBExpressionContext { get; }
    }

    public class AnalyticItemRDBExpressionSetterContext : IAnalyticItemRDBExpressionSetterContext
    {
        public AnalyticItemRDBExpressionSetterContext(RDBExpressionContext rdbExpressionContext)
        {
            this.RDBExpressionContext = rdbExpressionContext;
        }
        public RDBExpressionContext RDBExpressionContext { get; private set; }
    }

    public interface IAnalyticItemRDBReaderValueGetter
    {
        Object GetReaderValue(Vanrise.Data.RDB.IRDBDataReader reader, string fieldName);
    }

    internal interface IResolvedAnalyticItemConfig
    {
        IAnalyticItemRDBExpressionSetter RDBExpressionSetter { get; set; }

        IAnalyticItemRDBReaderValueGetter ReaderValueGetter { get; set; }
    }

    internal class ResolvedAnalyticDimensionConfig : IResolvedAnalyticItemConfig
    {
        public AnalyticDimension DimensionConfig { get; set; }

        public IAnalyticItemRDBExpressionSetter RDBExpressionSetter { get; set; }

        public IAnalyticItemRDBReaderValueGetter ReaderValueGetter { get; set; }
    }

    internal class ResolvedAnalyticAggregateConfig : IResolvedAnalyticItemConfig
    {
        public AnalyticAggregate AggregateConfig { get; set; }

        public IAnalyticItemRDBExpressionSetter RDBExpressionSetter { get; set; }

        public IAnalyticItemRDBReaderValueGetter ReaderValueGetter { get; set; }
    }

    internal class ResolvedAnalyticJoinConfig
    {
        public string JoinName { get; set; }

        public AnalyticJoin JoinConfig { get; set; }

        public IAnalyticJoinRDBExpressionSetter JoinRDBExpressionSetter { get; set; }
    }

    public interface IAnalyticJoinRDBExpressionSetter
    {
        void SetExpression(IAnalyticJoinRDBExpressionSetterContext context);
    }

    public interface IAnalyticJoinRDBExpressionSetterContext
    {
        RDBJoinContext RDBJoinContext { get; }
    }


    public class AnalyticJoinRDBExpressionSetterContext : IAnalyticJoinRDBExpressionSetterContext
    {
        public AnalyticJoinRDBExpressionSetterContext(RDBJoinContext rdbJoinContext)
        {
            this.RDBJoinContext = rdbJoinContext;
        }
        public RDBJoinContext RDBJoinContext { get; private set; }
    }


    internal class ResolvedConfigs
    {
        public Dictionary<string, ResolvedAnalyticDimensionConfig> DimensionConfigs { get; set; }

        public Dictionary<string, ResolvedAnalyticAggregateConfig> AggregateConfigs { get; set; }

        public Dictionary<string, ResolvedAnalyticJoinConfig> JoinConfigs { get; set; }

    }
}
