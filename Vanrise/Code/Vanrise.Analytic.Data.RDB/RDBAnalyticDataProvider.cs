using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Entities;
namespace Vanrise.Analytic.Data.RDB
{
    public class RDBAnalyticDataProvider : AnalyticDataProvider
    {
        public override Guid ConfigId => new Guid("2E88B323-DE47-4C6B-8AD5-CC941BB3611E");

        public string ModuleName { get; set; }

        public RDBAnalyticDataProviderTable Table { get; set; }

        public override IAnalyticDataManager CreateDataManager(IAnalyticTableQueryContext queryContext)
        {
            this.Table.ThrowIfNull("this.Table");
            var dataManager = new AnalyticDataManager() { AnalyticTableQueryContext = queryContext, ModuleName = this.ModuleName, TableName = this.Table.GetRDBTableName() };
            dataManager.ResolvedConfigs = DynamicTypeGenerator.GetResolvedConfigs(queryContext);
            return dataManager;
        }
    }

    public abstract class RDBAnalyticDataProviderTable
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetRDBTableName();
    }

    public class StaticRDBAnalyticDataProviderTable : RDBAnalyticDataProviderTable
    {
        public override Guid ConfigId => new Guid("2FA4D832-1351-4253-9F31-2F8B30DBE067");

        public string RDBTableName { get; set; }

        public override string GetRDBTableName()
        {
            return this.RDBTableName;
        }
    }
}
