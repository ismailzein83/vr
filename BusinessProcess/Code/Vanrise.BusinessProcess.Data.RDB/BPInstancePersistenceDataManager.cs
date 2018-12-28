using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPInstancePersistenceDataManager : IBPInstancePersistenceDataManager
    {
        static string TABLE_NAME = "bp_BPInstancePersistence";
        static string TABLE_ALIAS = "InstanceP";

        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_BPState = "BPState";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static BPInstancePersistenceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_BPState, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPInstancePersistence",
                Columns = columns,
                IdColumnName = COL_ProcessInstanceID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcess", "BusinessProcessDBConnStringKey", "BusinessProcessDBConnString");
        }
        #region Public Methods
        public void DeleteState(long processInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var whereContext = deleteQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            queryContext.ExecuteNonQuery();
        }

        public string GetInstanceState(long processInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().Column(COL_BPState);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            return queryContext.ExecuteScalar().StringValue;
        }

        public void InsertOrUpdateInstance(long processInstanceId, string instanceState)
        {
            //Update Query 
            var updateQueryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = updateQueryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_BPState).Value(instanceState);

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            int result = updateQueryContext.ExecuteNonQuery();

            //Insert Query 

            var insertQueryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = insertQueryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ProcessInstanceID).Value(processInstanceId);
            insertQuery.Column(COL_BPState).Value(instanceState);

            insertQueryContext.ExecuteNonQuery();
        }
    }
    #endregion
}

