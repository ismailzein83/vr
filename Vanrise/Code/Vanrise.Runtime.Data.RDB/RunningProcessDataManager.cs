//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Data.RDB;

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
//            throw new NotImplementedException();
//        }

//        public List<Runtime.Entities.RunningProcessDetails> GetFilteredRunningProcesses(Vanrise.Entities.DataRetrievalInput<Runtime.Entities.RunningProcessQuery> input)
//        {
//            throw new NotImplementedException();
//        }

//        public List<Runtime.Entities.RunningProcessInfo> GetRunningProcesses()
//        {
//            throw new NotImplementedException();
//        }

//        public Runtime.Entities.RunningProcessInfo InsertProcessInfo(Guid runtimeNodeId, Guid runtimeNodeInstanceId, int osProcessId, Runtime.Entities.RunningProcessAdditionalInfo additionalInfo)
//        {
//            throw new NotImplementedException();
//        }

//        public bool IsExists(int runningProcessId)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
