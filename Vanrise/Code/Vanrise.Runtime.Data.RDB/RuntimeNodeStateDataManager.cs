using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.RDB
{
    public class RuntimeNodeStateDataManager : IRuntimeNodeStateDataManager
    {
        static string TABLE_NAME = "runtime_RuntimeNodeState";
        static string TABLE_ALIAS = "NodeState";

        internal const string COL_RuntimeNodeID = "RuntimeNodeID";
        internal const string COL_InstanceID = "InstanceID";
        const string COL_MachineName = "MachineName";
        const string COL_OSProcessID = "OSProcessID";
        const string COL_OSProcessName = "OSProcessName";
        internal const string COL_ServiceURL = "ServiceURL";
        const string COL_StartedTime = "StartedTime";
        internal const string COL_LastHeartBeatTime = "LastHeartBeatTime";
        const string COL_CPUUsage = "CPUUsage";
        const string COL_AvailableRAM = "AvailableRAM";
        const string COL_DiskInfos = "DiskInfos";
        const string COL_NbOfEnabledProcesses = "NbOfEnabledProcesses";
        const string COL_NbOfRunningProcesses = "NbOfRunningProcesses";

        static RuntimeNodeStateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_RuntimeNodeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_InstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_MachineName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_OSProcessID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_OSProcessName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_ServiceURL, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_StartedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastHeartBeatTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CPUUsage, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 6, Precision = 2 });
            columns.Add(COL_AvailableRAM, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 24, Precision = 4 });
            columns.Add(COL_DiskInfos, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_NbOfEnabledProcesses, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_NbOfRunningProcesses, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "runtime",
                DBTableName = "RuntimeNodeState",
                Columns = columns,
                IdColumnName = COL_RuntimeNodeID
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Runtime", "RuntimeConnStringKey", "RuntimeDBConnString");
        }

        #region Public Methods
        public List<RuntimeNodeState> GetAllNodes()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

            var selectColumnsContext = selectQuery.SelectColumns();
            selectColumnsContext.Columns(COL_RuntimeNodeID, COL_InstanceID, COL_MachineName, COL_OSProcessID, COL_OSProcessName, COL_ServiceURL, COL_StartedTime, COL_LastHeartBeatTime);

            var s = selectColumnsContext.Expression("NbOfSecondsHeartBeatReceived").DateTimeDiff(RDBDateTimeDiffInterval.Seconds);
            s.DateTimeExpression1().Column(COL_LastHeartBeatTime);
            s.DateTimeExpression2().DateNow();

            return queryContext.GetItems(RuntimeNodeStateMapper);
        }

        public RuntimeNodeState GetNodeState(Guid runtimeNodeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);

            var selectColumnsContext = selectQuery.SelectColumns();
            selectColumnsContext.Columns(COL_RuntimeNodeID, COL_InstanceID, COL_MachineName, COL_OSProcessID, COL_OSProcessName, COL_ServiceURL, COL_StartedTime, COL_LastHeartBeatTime);

            var expression = selectColumnsContext.Expression("NbOfSecondsHeartBeatReceived").DateTimeDiff(RDBDateTimeDiffInterval.Seconds);
            expression.DateTimeExpression1().Column(COL_LastHeartBeatTime);
            expression.DateTimeExpression2().DateNow();

            selectQuery.Where().EqualsCondition(COL_RuntimeNodeID).Value(runtimeNodeId);

            return queryContext.GetItem(RuntimeNodeStateMapper);
        }

        public bool TrySetInstanceStarted(Guid runtimeNodeId, Guid serviceInstanceId, string machineName, int osProcessId, string osProcessName, string serviceURL, TimeSpan heartBeatTimeout)
        {
            //Insert Query
            var insertQueryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = insertQueryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_RuntimeNodeID).Value(runtimeNodeId);
            insertQuery.Column(COL_InstanceID).Value(serviceInstanceId);
            insertQuery.Column(COL_MachineName).Value(machineName);
            insertQuery.Column(COL_OSProcessID).Value(osProcessId);
            insertQuery.Column(COL_OSProcessName).Value(osProcessName);
            insertQuery.Column(COL_ServiceURL).Value(serviceURL);
            insertQuery.Column(COL_StartedTime).DateNow();
            insertQuery.Column(COL_LastHeartBeatTime).DateNow();

            insertQuery.IfNotExists(TABLE_ALIAS).EqualsCondition(COL_RuntimeNodeID).Value(runtimeNodeId);

            insertQueryContext.ExecuteNonQuery();

            //Update Query 
            var updateQueryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = updateQueryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_InstanceID).Value(serviceInstanceId);
            updateQuery.Column(COL_MachineName).Value(machineName);
            updateQuery.Column(COL_OSProcessID).Value(osProcessId);
            updateQuery.Column(COL_OSProcessName).Value(osProcessName);
            updateQuery.Column(COL_ServiceURL).Value(serviceURL);
            updateQuery.Column(COL_StartedTime).DateNow();
            updateQuery.Column(COL_LastHeartBeatTime).DateNow();

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_RuntimeNodeID).Value(runtimeNodeId);

            var childConditionGroup = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            childConditionGroup.NullCondition(COL_InstanceID);
            childConditionGroup.EqualsCondition(COL_InstanceID).Value(serviceInstanceId);

            var compareConditionContext = childConditionGroup.CompareCondition(RDBCompareConditionOperator.G);

            var dateTimeDiffContext = compareConditionContext.Expression1().DateTimeDiff(RDBDateTimeDiffInterval.Seconds);
            dateTimeDiffContext.DateTimeExpression1().Column(COL_LastHeartBeatTime);
            dateTimeDiffContext.DateTimeExpression2().DateNow();

            compareConditionContext.Expression2().Value((int)heartBeatTimeout.TotalSeconds);

            return updateQueryContext.ExecuteNonQuery() > 0;
        }

        public bool TryUpdateHeartBeat(Guid runtimeNodeId, Guid serviceInstanceId, decimal cpuUsage, decimal availableRAM, string diskInfos, int nbOfEnabledProcesses, int nbOfRunningProcesses)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_LastHeartBeatTime).DateNow();
            updateQuery.Column(COL_CPUUsage).Value(cpuUsage);
            updateQuery.Column(COL_AvailableRAM).Value(availableRAM);
            updateQuery.Column(COL_DiskInfos).Value(diskInfos);
            updateQuery.Column(COL_NbOfEnabledProcesses).Value(nbOfEnabledProcesses);
            updateQuery.Column(COL_NbOfRunningProcesses).Value(nbOfRunningProcesses);

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_RuntimeNodeID).Value(runtimeNodeId);
            whereContext.EqualsCondition(COL_InstanceID).Value(serviceInstanceId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion

        #region Internal Methods

        internal void JoinRuntimeNodeState(RDBJoinContext join, string runtimeNodeStateTableAlias, string otherTableAlias, string otherTableColumnName, bool withNoLock, RDBJoinType rDBJoinType = RDBJoinType.Inner)
        {
            join.JoinOnEqualOtherTableColumn(rDBJoinType, TABLE_NAME, runtimeNodeStateTableAlias, COL_InstanceID, otherTableAlias, otherTableColumnName, withNoLock);
        }

        #endregion

        #region Mappers

        private RuntimeNodeState RuntimeNodeStateMapper(IRDBDataReader reader)
        {
            return new RuntimeNodeState
            {
                RuntimeNodeId = reader.GetGuid(COL_RuntimeNodeID),
                InstanceId = reader.GetGuid(COL_InstanceID),
                MachineName = reader.GetString(COL_MachineName),
                OSProcessId = reader.GetInt(COL_OSProcessID),
                OSProcessName = reader.GetString(COL_OSProcessName),
                ServiceURL = reader.GetString(COL_ServiceURL),
                StartedTime = reader.GetDateTime(COL_StartedTime),
                LastHeartBeatTime = reader.GetDateTime(COL_LastHeartBeatTime),
                NbOfSecondsHeartBeatReceived = reader.GetDouble("NbOfSecondsHeartBeatReceived")
            };
        }

        #endregion
    }
}
