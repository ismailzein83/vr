using System;
using System.Collections.Generic;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Runtime.Data.RDB
{
    public class RuntimeManagerDataManager : IRuntimeManagerDataManager
    {
        static string TABLE_NAME = "runtime_RuntimeManager";
        static string TABLE_ALIAS = "rtm";

        const string COL_ID = "ID";
        const string COL_InstanceID = "InstanceID";
        const string COL_TakenTime = "TakenTime";

        static RuntimeManagerDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_InstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_TakenTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "runtime",
                DBTableName = "RuntimeManager",
                Columns = columns,
                IdColumnName = COL_ID
            });
        }

        private BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Runtime", "RuntimeConnStringKey", "RuntimeDBConnString");
        }

        #region Public Methods
        public string GetRuntimeManagerServiceURL(out Guid runtimeNodeInstanceId)
        {
            var nodeStateDataManager = new RuntimeNodeStateDataManager();
            string runtimeNodeStateTableAlias = "ns";

            string serviceURL = null;
            runtimeNodeInstanceId = default(Guid);
            Guid runtimeNodeInstanceId_Local = default(Guid);

            var queryContext = new RDBQueryContext(this.GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

            selectQuery.SelectColumns().Column(runtimeNodeStateTableAlias, RuntimeNodeStateDataManager.COL_InstanceID, "RuntimeNodeInstanceID");
            selectQuery.SelectColumns().Column(runtimeNodeStateTableAlias, RuntimeNodeStateDataManager.COL_ServiceURL, "ServiceURL");

            var joinContext = selectQuery.Join();
            nodeStateDataManager.JoinRuntimeNodeState(joinContext, runtimeNodeStateTableAlias, TABLE_ALIAS, COL_InstanceID, true);

            selectQuery.Where().EqualsCondition("ID").Value(1);

            queryContext.ExecuteReader(reader =>
            {
                if (!reader.Read())
                    return;

                serviceURL = reader.GetString("ServiceURL");
                runtimeNodeInstanceId_Local = reader.GetGuid("RuntimeNodeInstanceID");
            });

            runtimeNodeInstanceId = runtimeNodeInstanceId_Local;
            return serviceURL;
        }

        public bool TryTakePrimaryNode(Guid serviceInstanceId, TimeSpan heartbeatTimeOut)
        {
            //Insert Query

            var insertQueryContext = new RDBQueryContext(this.GetDataProvider());

            var insertQuery = insertQueryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ID).Value(1);
            insertQuery.Column(COL_InstanceID).Value(serviceInstanceId);
            insertQuery.Column(COL_TakenTime).DateNow();

            insertQuery.IfNotExists(TABLE_ALIAS, (RDBConditionGroupOperator)0).EqualsCondition("ID").Value(1);

            if (insertQueryContext.ExecuteNonQuery() > 0)
                return true;

            //Select Query

            var selectQueryContext = new RDBQueryContext(this.GetDataProvider());

            var selectQuery = selectQueryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Column(COL_InstanceID);

            selectQuery.Where().EqualsCondition(COL_ID).Value(1);

            var instanceId = selectQueryContext.ExecuteScalar().NullableGuidValue;

            if (instanceId.HasValue && instanceId.Value == serviceInstanceId)
                return true;

            //Update Query

            var nodeStateDataManager = new RuntimeNodeStateDataManager();

            string runtimeNodeStateTableAlias = "NodeState";


            var updateQueryContext = new RDBQueryContext(this.GetDataProvider());

            var updateQuery = updateQueryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_InstanceID).Value(serviceInstanceId);
            updateQuery.Column(COL_TakenTime).DateNow();

            var join = updateQuery.Join(TABLE_ALIAS);
            nodeStateDataManager.JoinRuntimeNodeState(join, runtimeNodeStateTableAlias, TABLE_ALIAS, COL_InstanceID, true, RDBJoinType.Left);

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(1);

            var childGroupCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            childGroupCondition.NullCondition(runtimeNodeStateTableAlias, RuntimeNodeStateDataManager.COL_RuntimeNodeID);

            var conditionContext3 = childGroupCondition.CompareCondition(RDBCompareConditionOperator.L);
            conditionContext3.Expression1().Value((int)heartbeatTimeOut.TotalSeconds);

            var expressionContext = conditionContext3.Expression2().DateTimeDiff(RDBDateTimeDiffInterval.Seconds);
            expressionContext.DateTimeExpression1().Column(runtimeNodeStateTableAlias, RuntimeNodeStateDataManager.COL_LastHeartBeatTime);
            expressionContext.DateTimeExpression2().DateNow();

            return updateQueryContext.ExecuteNonQuery() > 0;
        }

        public Guid? GetNonTimedOutRuntimeManagerId(TimeSpan heartbeatTimeOut)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);

            selectQuery.SelectColumns().Column(COL_InstanceID);

            var nodeStateDataManager = new RuntimeNodeStateDataManager();

            string runtimeNodeStateTableAlias = "NodeState";
            nodeStateDataManager.JoinRuntimeNodeState(selectQuery.Join(), runtimeNodeStateTableAlias, TABLE_ALIAS, COL_InstanceID, true);
            var where = selectQuery.Where();
            where.EqualsCondition(COL_ID).Value(1);

            var compareConditionCtx = where.CompareCondition(RDBCompareConditionOperator.L);

            var dateDiffCtx = compareConditionCtx.Expression1().DateTimeDiff(RDBDateTimeDiffInterval.Seconds);
            dateDiffCtx.DateTimeExpression1().Column(runtimeNodeStateTableAlias, RuntimeNodeStateDataManager.COL_LastHeartBeatTime);
            dateDiffCtx.DateTimeExpression2().DateNow();

            compareConditionCtx.Expression2().Value((int)heartbeatTimeOut.TotalSeconds);

            return queryContext.ExecuteScalar().NullableGuidValue;
        }
        #endregion
    }
}
