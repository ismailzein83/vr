using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Data.RDB;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class ProcessSynchronisationDataManager : IProcessSynchronisationDataManager
    {

        static string TABLE_NAME = "bp_ProcessSynchronisation";
        static string TABLE_ALIAS = "ProcessSync";

        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_IsEnabled = "IsEnabled";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_LastModifiedBy = "LastModifiedBy";

        static ProcessSynchronisationDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_IsEnabled, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "ProcessSynchronisation",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcessConfig", "BusinessProcessConfigDBConnStringKey", "BusinessProcessDBConnString");
        }

        #region Public Methods
        public bool AreProcessSynchronisationsUpdated(ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
        }
        public bool DisableProcessSynchronisation(Guid processSynchronisationId, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_IsEnabled).Value(0);
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Column(COL_LastModifiedTime).DateNow();

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(processSynchronisationId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        public bool EnableProcessSynchronisation(Guid processSynchronisationId, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_IsEnabled).Value(1);
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Column(COL_LastModifiedTime).DateNow();

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(processSynchronisationId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        public List<ProcessSynchronisation> GetProcessSynchronisations()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(ProcessSynchronisationMapper);
        }
        public bool InsertProcessSynchronisation(Guid processSynchronisationId, ProcessSynchronisationToAdd processSynchronisationToAdd, int createdBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ID).Value(processSynchronisationId);
            insertQuery.Column(COL_Name).Value(processSynchronisationToAdd.Name);
            insertQuery.Column(COL_IsEnabled).Value(processSynchronisationToAdd.IsEnabled);
            if (processSynchronisationToAdd.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(processSynchronisationToAdd.Settings));
            insertQuery.Column(COL_CreatedBy).Value(createdBy);
            insertQuery.Column(COL_LastModifiedBy).Value(createdBy);

            var ifNotExistContext = insertQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);
            ifNotExistContext.EqualsCondition(COL_Name).Value(processSynchronisationToAdd.Name);

            return queryContext.ExecuteNonQuery() > 0;
        }
        public bool UpdateProcessSynchronisation(ProcessSynchronisationToUpdate processSynchronisationToUpdate, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_ID).Value(processSynchronisationToUpdate.ProcessSynchronisationId);
            updateQuery.Column(COL_Name).Value(processSynchronisationToUpdate.Name);
            updateQuery.Column(COL_IsEnabled).Value(processSynchronisationToUpdate.IsEnabled);
            if (processSynchronisationToUpdate.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(processSynchronisationToUpdate.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);

            var ifNotExistContext = updateQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);
            ifNotExistContext.EqualsCondition(COL_Name).Value(processSynchronisationToUpdate.Name);
            ifNotExistContext.NotEqualsCondition(COL_ID).Value(processSynchronisationToUpdate.ProcessSynchronisationId);

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(processSynchronisationToUpdate.ProcessSynchronisationId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion

        #region Mappers
        ProcessSynchronisation ProcessSynchronisationMapper(IRDBDataReader reader)
        {
            string settings = reader.GetString("Settings");
            return new ProcessSynchronisation
            {
                ProcessSynchronisationId = reader.GetGuid("ID"),
                Name = reader.GetString("Name"),
                IsEnabled = reader.GetBoolean("IsEnabled"),
                Settings = !string.IsNullOrEmpty(settings) ? Vanrise.Common.Serializer.Deserialize<ProcessSynchronisationSettings>(settings) : null,
                CreatedBy = reader.GetInt("CreatedBy"),
                CreatedTime = reader.GetDateTime("CreatedTime"),
                LastModifiedBy = reader.GetInt("LastModifiedBy"),
                LastModifiedTime = reader.GetDateTime("LastModifiedTime")
            };
        }
        #endregion }
    }
}