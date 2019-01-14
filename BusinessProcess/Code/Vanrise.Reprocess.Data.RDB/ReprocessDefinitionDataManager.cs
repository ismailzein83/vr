
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.Data.RDB
{
    public class ReprocessDefinitionDataManager : Vanrise.Reprocess.Data.IReprocessDefinitionDataManager
    {
        #region Constructor

        static string TABLE_NAME = "reprocess_ReprocessDefinition";
        static string TABLE_ALIAS = "ReprocessDef";

        const string COL_Id = "Id";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static ReprocessDefinitionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_Id, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "reprocess",
                DBTableName = "ReprocessDefinition",
                Columns = columns,
                IdColumnName = COL_Id,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        #endregion

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Reprocess", "ConfigurationDBConnStringKey", "ConfigurationDBConnString");
        }

        #region Public Methods
        public List<ReprocessDefinition> GetReprocessDefinition()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(ReprocessDefinitionMapper);
        }

        public bool Insert(ReprocessDefinition reprocessDefinitionItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();

            var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExist.EqualsCondition(COL_Name).Value(reprocessDefinitionItem.Name);

            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_Id).Value(reprocessDefinitionItem.ReprocessDefinitionId);
            insertQuery.Column(COL_Name).Value(reprocessDefinitionItem.Name);

            if (reprocessDefinitionItem.Settings != null)
                insertQuery.Column(COL_Settings).Value(Serializer.Serialize(reprocessDefinitionItem.Settings));

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Update(ReprocessDefinition reprocessDefinitionItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();

            var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);

            ifNotExist.NotEqualsCondition(COL_Id).Value(reprocessDefinitionItem.ReprocessDefinitionId);
            ifNotExist.EqualsCondition(COL_Name).Value(reprocessDefinitionItem.Name);

            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Name).Value(reprocessDefinitionItem.Name);

            if (reprocessDefinitionItem.Settings != null)
                updateQuery.Column(COL_Settings).Value(Serializer.Serialize(reprocessDefinitionItem.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            var where = updateQuery.Where();
            where.EqualsCondition(COL_Id).Value(reprocessDefinitionItem.ReprocessDefinitionId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreReprocessDefinitionUpdated(ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
        }

        #endregion

        #region Mappers
        ReprocessDefinition ReprocessDefinitionMapper(IRDBDataReader reader)
        {
            return new ReprocessDefinition
            {
                ReprocessDefinitionId = reader.GetGuid(COL_Id),
                Name = reader.GetString(COL_Name),
                Settings = Serializer.Deserialize<ReprocessDefinitionSettings>(reader.GetString(COL_Settings))
            };
        }
        #endregion
    }
}