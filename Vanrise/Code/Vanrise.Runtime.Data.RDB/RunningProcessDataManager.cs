using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Runtime.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Runtime.Data.RDB
{
    public class RunningProcessDataManager : IRunningProcessDataManager
    {
        static string TABLE_NAME = "runtime_RunningProcess";
        static string TABLE_ALIAS = "rp";

        const string COL_ID = "ID";
        const string COL_RuntimeNodeID = "RuntimeNodeID";
        const string COL_RuntimeNodeInstanceID = "RuntimeNodeInstanceID";
        const string COL_OSProcessID = "OSProcessID";
        const string COL_StartedTime = "StartedTime";
        const string COL_AdditionalInfo = "AdditionalInfo";

        static RunningProcessDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_RuntimeNodeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_RuntimeNodeInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_OSProcessID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_StartedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_AdditionalInfo, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "runtime",
                DBTableName = "RunningProcess",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_StartedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Runtime", "RuntimeConnStringKey", "RuntimeDBConnString");
        }

        #region Public Methods

        public Runtime.Entities.RunningProcessInfo InsertProcessInfo(Guid runtimeNodeId, Guid runtimeNodeInstanceId, int osProcessId, RunningProcessAdditionalInfo additionalInfo)
        {
            //Insert Query
            var firstQueryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = firstQueryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            insertQuery.Column(COL_RuntimeNodeID).Value(runtimeNodeId);
            insertQuery.Column(COL_RuntimeNodeInstanceID).Value(runtimeNodeInstanceId);
            insertQuery.Column(COL_OSProcessID).Value(osProcessId);

            if (additionalInfo != null)
                insertQuery.Column(COL_AdditionalInfo).Value(Common.Serializer.Serialize(additionalInfo));

            int generatedID = firstQueryContext.ExecuteScalar().IntValue;

            //SelectQuery
            var secondQueryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = secondQueryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Where().EqualsCondition(COL_ID).Value(generatedID);

            return secondQueryContext.GetItem(RunningProcessInfoMapper);
        }

        public List<Runtime.Entities.RunningProcessInfo> GetRunningProcesses()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(RunningProcessInfoMapper);
        }

        public List<Runtime.Entities.RunningProcessDetails> GetFilteredRunningProcesses(DataRetrievalInput<Runtime.Entities.RunningProcessQuery> input)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Where().EqualsCondition(COL_RuntimeNodeInstanceID).Value(input.Query.RuntimeNodeInstanceId);

            return queryContext.GetItems(RunningProcessDetailsMapper);
        }

        public void DeleteRunningProcess(int runningProcessId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ID).Value(runningProcessId);

            queryContext.ExecuteNonQuery();
        }

        public bool IsExists(int runningProcessId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Column(COL_ID);
            selectQuery.Where().EqualsCondition(COL_ID).Value(runningProcessId);

            return queryContext.ExecuteScalar().NullableIntValue.HasValue;
        }

        #endregion

        #region Private Methods
        private RunningProcessInfo RunningProcessInfoMapper(IRDBDataReader reader)
        {
            var runningProcessInfo = new RunningProcessInfo
            {
                ProcessId = reader.GetInt(COL_ID),
                OSProcessId = reader.GetInt(COL_OSProcessID),
                RuntimeNodeId = reader.GetGuid(COL_RuntimeNodeID),
                RuntimeNodeInstanceId = reader.GetGuid(COL_RuntimeNodeInstanceID),
                StartedTime = reader.GetDateTime(COL_StartedTime)
            };

            string serializedAdditionInfo = reader.GetString(COL_AdditionalInfo);
            if (!string.IsNullOrEmpty(serializedAdditionInfo))
                runningProcessInfo.AdditionalInfo = Common.Serializer.Deserialize<RunningProcessAdditionalInfo>(serializedAdditionInfo);

            return runningProcessInfo;
        }

        private RunningProcessDetails RunningProcessDetailsMapper(IRDBDataReader reader)
        {
            var runningProcessDetails = new RunningProcessDetails
            {
                ProcessId = reader.GetInt(COL_ID),
                OSProcessId = reader.GetInt(COL_OSProcessID),
                RuntimeNodeId = reader.GetGuid(COL_RuntimeNodeID),
                RuntimeNodeInstanceId = reader.GetGuid(COL_RuntimeNodeInstanceID),
                StartedTime = reader.GetDateTime(COL_StartedTime)
            };
            string serializedAdditionInfo = reader.GetString(COL_AdditionalInfo);
            if (serializedAdditionInfo != null)
                runningProcessDetails.AdditionalInfo = Common.Serializer.Deserialize(serializedAdditionInfo) as RunningProcessAdditionalInfo;
            return runningProcessDetails;
        }
        #endregion
    }
}
