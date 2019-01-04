using System;
using System.Collections.Generic;
using Vanrise.Data.RDB;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.RDB
{
    public class BusinessEntityDataManager : IBusinessEntityDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_BusinessEntity";
        static string TABLE_ALIAS = "businessEntity";
        const string COL_Id = "Id";
        const string COL_Name = "Name";
        const string COL_Title = "Title";
        const string COL_ModuleId = "ModuleId";
        const string COL_BreakInheritance = "BreakInheritance";
        const string COL_PermissionOptions = "PermissionOptions";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static BusinessEntityDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_Id, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Title, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_ModuleId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_BreakInheritance, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_PermissionOptions, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "BusinessEntity",
                Columns = columns,
                IdColumnName = COL_Id,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Sec", "SecurityDBConnStringKey", "SecurityDBConnString");
        }
        #endregion

        #region Mappers

        BusinessEntity EntityMapper(IRDBDataReader reader)
        {
            BusinessEntity entity = new BusinessEntity
            {
                EntityId = reader.GetGuid(COL_Id),
                Name = reader.GetString(COL_Name),
                Title = reader.GetString(COL_Title),
                ModuleId = reader.GetGuid(COL_ModuleId),
                BreakInheritance = reader.GetBoolean(COL_BreakInheritance),
                PermissionOptions = Common.Serializer.Deserialize<List<string>>(reader.GetString(COL_PermissionOptions))
            };

            return entity;
        }
        #endregion

        #region IBusinessEntityDataManager
        public bool AddBusinessEntity(BusinessEntity businessEntity)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.IfNotExists(TABLE_ALIAS).EqualsCondition(COL_Name).Value(businessEntity.Name);
            insertQuery.Column(COL_Id).Value(businessEntity.EntityId);
            insertQuery.Column(COL_Name).Value(businessEntity.Name);
            insertQuery.Column(COL_Title).Value(businessEntity.Title);
            insertQuery.Column(COL_ModuleId).Value(businessEntity.ModuleId);
            insertQuery.Column(COL_BreakInheritance).Value(businessEntity.BreakInheritance);
            if (businessEntity.PermissionOptions != null)
                insertQuery.Column(COL_PermissionOptions).Value(Common.Serializer.Serialize(businessEntity.PermissionOptions));
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreBusinessEntitiesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public IEnumerable<BusinessEntity> GetEntities()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(EntityMapper);
        }

        public bool ToggleBreakInheritance(Guid entityId)
        {
            var selectQueryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = selectQueryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Where().EqualsCondition(COL_Id).Value(entityId);
            var item = selectQueryContext.GetItem(EntityMapper);
            if (item != null)
            {
                var updateQueryContext = new RDBQueryContext(GetDataProvider());
                var updateQuery = updateQueryContext.AddUpdateQuery();
                updateQuery.FromTable(TABLE_NAME);
                updateQuery.Column(COL_BreakInheritance).Value(!item.BreakInheritance);
                updateQuery.Where().EqualsCondition(COL_Id).Value(entityId);
                return updateQueryContext.ExecuteNonQuery() > 0;
            }
            return false;

        }

        public bool UpdateBusinessEntity(BusinessEntity businessEntity)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(businessEntity.Name);
            ifNotExists.NotEqualsCondition(COL_Id).Value(businessEntity.EntityId);
            updateQuery.Column(COL_Name).Value(businessEntity.Name);
            updateQuery.Column(COL_Title).Value(businessEntity.Title);
            updateQuery.Column(COL_ModuleId).Value(businessEntity.ModuleId);
            updateQuery.Column(COL_BreakInheritance).Value(businessEntity.BreakInheritance);
            if (businessEntity.PermissionOptions != null)
                updateQuery.Column(COL_PermissionOptions).Value(Vanrise.Common.Serializer.Serialize(businessEntity.PermissionOptions));
            else
                updateQuery.Column(COL_PermissionOptions).Null();
            updateQuery.Where().EqualsCondition(COL_Id).Value(businessEntity.EntityId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateBusinessEntityRank(Guid entityId, Guid moduleId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_ModuleId).Value(moduleId);
            updateQuery.Where().EqualsCondition(COL_Id).Value(entityId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
