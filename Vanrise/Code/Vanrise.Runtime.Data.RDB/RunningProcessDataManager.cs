//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Data.RDB;
//using Vanrise.Runtime.Entities;
//using Vanrise.Common;

//namespace Vanrise.Runtime.Data.RDB
//{
//    public class RunningProcessDataManager : IRunningProcessDataManager
//    {
//        static string TABLE_NAME = "runtime_RunningProcess";
//        static string TABLE_ALIAS = "RP";

//        const string COL_ID = "ID";
//        const string COL_RuntimeNodeID = "RuntimeNodeID";
//        const string COL_RuntimeNodeInstanceID = "RuntimeNodeInstanceID";
//        const string COL_OSProcessID = "OSProcessID";
//        const string COL_StartedTime = "StartedTime";
//        const string COL_AdditionalInfo = "AdditionalInfo";
//        const string COL_LastModifiedTime = "LastModifiedTime";


//        static RunningProcessDataManager()
//        {
//            var columns = new Dictionary<string, RDBTableColumnDefinition>();
//            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
//            columns.Add(COL_RuntimeNodeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
//            columns.Add(COL_RuntimeNodeInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
//            columns.Add(COL_OSProcessID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
//            columns.Add(COL_StartedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            columns.Add(COL_AdditionalInfo, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
//            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
//            {
//                DBSchemaName = "runtime",
//                DBTableName = "RunningProcess",
//                Columns = columns,
//                IdColumnName = COL_ID,
//                ModifiedTimeColumnName = COL_LastModifiedTime
//            });
//        }

//        BaseRDBDataProvider GetDataProvider()
//        {
//            return RDBDataProviderFactory.CreateProvider("Runtime", "RuntimeConnStringKey", "RuntimeDBConnString");
//        }

//        public void DeleteRunningProcess(int runningProcessId)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());

//            var deleteQuery = queryContext.AddDeleteQuery();
//            deleteQuery.FromTable(TABLE_NAME);
//            deleteQuery.Where().EqualsCondition(COL_ID).Value(runningProcessId);

//            queryContext.ExecuteNonQuery();
//        }

//        public List<Runtime.Entities.RunningProcessDetails> GetFilteredRunningProcesses(Vanrise.Entities.DataRetrievalInput<Runtime.Entities.RunningProcessQuery> input)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());

//            var selectQuery = queryContext.AddSelectQuery();
//            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
//            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
//            selectQuery.Where().EqualsCondition(COL_RuntimeNodeInstanceID).Value(input.Query.RuntimeNodeInstanceId);

//            return queryContext.GetItems(RunningProcessDetailsMapper);
//        }

//        public List<Runtime.Entities.RunningProcessInfo> GetRunningProcesses()
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());

//            var selectQuery = queryContext.AddSelectQuery();
//            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
//            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

//            return queryContext.GetItems(RunningProcessInfoMapper);
//        }

//        public Runtime.Entities.RunningProcessInfo InsertProcessInfo(Guid runtimeNodeId, Guid runtimeNodeInstanceId, int osProcessId, Runtime.Entities.RunningProcessAdditionalInfo additionalInfo)
//        {
//            //Insert Query
//            var firstQueryContext = new RDBQueryContext(GetDataProvider());

//            var insertQuery = firstQueryContext.AddInsertQuery();
//            insertQuery.IntoTable(TABLE_NAME);
//            insertQuery.Column(COL_RuntimeNodeID).Value(runtimeNodeId);
//            insertQuery.Column(COL_RuntimeNodeInstanceID).Value(runtimeNodeInstanceId);
//            insertQuery.Column(COL_OSProcessID).Value(osProcessId);
//            insertQuery.Column(COL_StartedTime).DateNow();
//            if (additionalInfo != null)
//                insertQuery.Column(COL_AdditionalInfo).Value(Common.Serializer.Serialize(additionalInfo));

//            insertQuery.AddSelectGeneratedId();

//            int generatedID = firstQueryContext.ExecuteScalar().IntValue;

//            //SelectQuery
//            var secondQueryContext = new RDBQueryContext(GetDataProvider());


//            var selectQuery = secondQueryContext.AddSelectQuery();
//            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
//            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
//            selectQuery.Where().EqualsCondition(COL_ID).Value(generatedID);

//            return secondQueryContext.GetItem(RunningProcessInfoMapper);
//        }

//        public bool IsExists(int runningProcessId)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());

//            var selectQuery = queryContext.AddSelectQuery();
//            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
//            selectQuery.SelectColumns().Column(COL_ID);
//            selectQuery.Where().EqualsCondition(COL_ID).Value(runningProcessId);

//            return queryContext.ExecuteScalar() != null;
//        }

//        #region Private Methods
//        private RunningProcessInfo RunningProcessInfoMapper(IRDBDataReader reader)
//        {
//            var runningProcessInfo = new RunningProcessInfo
//            {
//                ProcessId = reader.GetInt("ID"),
//                OSProcessId = reader.GetInt("OSProcessID"),
//                RuntimeNodeId = reader.GetGuid("RuntimeNodeID"),
//                RuntimeNodeInstanceId = reader.GetGuid("RuntimeNodeInstanceID"),
//                StartedTime = reader.GetDateTime("StartedTime")
//            };
//            string serializedAdditionInfo = reader.GetString("AdditionalInfo");
//            if (serializedAdditionInfo != null)
//                runningProcessInfo.AdditionalInfo = Common.Serializer.Deserialize(serializedAdditionInfo) as RunningProcessAdditionalInfo;
//            return runningProcessInfo;
//        }

//        private RunningProcessDetails RunningProcessDetailsMapper(IRDBDataReader reader)
//        {
//            var runningProcessDetails = new RunningProcessDetails
//            {
//                ProcessId = reader.GetInt("ID"),
//                OSProcessId = reader.GetInt("OSProcessID"),
//                RuntimeNodeId = reader.GetGuid("RuntimeNodeID"),
//                RuntimeNodeInstanceId = reader.GetGuid("RuntimeNodeInstanceID"),
//                StartedTime = reader.GetDateTime("StartedTime")
//            };
//            string serializedAdditionInfo = reader.GetString("AdditionalInfo");
//            if (serializedAdditionInfo != null)
//                runningProcessDetails.AdditionalInfo = Common.Serializer.Deserialize(serializedAdditionInfo) as RunningProcessAdditionalInfo;
//            return runningProcessDetails;
//        }
//        #endregion
//    }
//}
