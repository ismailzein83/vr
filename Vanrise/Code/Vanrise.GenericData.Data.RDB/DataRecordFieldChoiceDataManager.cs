using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;
using Vanrise.Entities;
namespace Vanrise.GenericData.Data.RDB
{
    public class DataRecordFieldChoiceDataManager : IDataRecordFieldChoiceDataManager
    {
        #region RDB
        static string TABLE_NAME = "genericdata_DataRecordFieldChoice";
        static string TABLE_ALIAS = "drfc";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static DataRecordFieldChoiceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "genericdata",
                DBTableName = "DataRecordFieldChoice",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        #endregion
        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_GenericData", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #endregion

        #region Mappers
        DataRecordFieldChoice DataRecordFieldChoiceMapper(IRDBDataReader reader)
        {
            return new DataRecordFieldChoice()
            {
                DataRecordFieldChoiceId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Settings = Vanrise.Common.Serializer.Deserialize<DataRecordFieldChoiceSettings>(reader.GetString(COL_Settings))
            };
        }
        #endregion
        #region IDataRecordFieldChoiceDataManager
        public bool AddDataRecordFieldChoice(DataRecordFieldChoice dataRecordFieldChoice)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(dataRecordFieldChoice.Name);
            insertQuery.Column(COL_ID).Value(dataRecordFieldChoice.DataRecordFieldChoiceId);
            insertQuery.Column(COL_Name).Value(dataRecordFieldChoice.Name);
            if(dataRecordFieldChoice.Settings!=null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataRecordFieldChoice.Settings));
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreDataRecordFieldChoicesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public IEnumerable<DataRecordFieldChoice> GetDataRecordFieldChoices()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(DataRecordFieldChoiceMapper);
        }

        public bool UpdateDataRecordFieldChoice(DataRecordFieldChoice dataRecordFieldChoice)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.NotEqualsCondition(COL_ID).Value(dataRecordFieldChoice.DataRecordFieldChoiceId);
            ifNotExists.EqualsCondition(COL_Name).Value(dataRecordFieldChoice.Name);
            updateQuery.Column(COL_Name).Value(dataRecordFieldChoice.Name);
            if (dataRecordFieldChoice.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataRecordFieldChoice.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(dataRecordFieldChoice.DataRecordFieldChoiceId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
