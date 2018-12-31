using System;
using System.Collections.Generic;
using Vanrise.Data.RDB;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.RDB
{
    public class TenantDataManager : ITenantDataManager
    {
        #region RDB

        static string TABLE_NAME = "sec_Tenant";
        static string TABLE_ALIAS = "tenant";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_ParentTenantID = "ParentTenantID";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static TenantDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_ParentTenantID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "Tenant",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        #endregion

        #region Mappers
        private Tenant TenantMapper(IRDBDataReader reader)
        {
            return new Tenant
            {
                TenantId = reader.GetInt(COL_ID),
                ParentTenantId = reader.GetNullableInt(COL_ParentTenantID),
                Name = reader.GetString(COL_Name),
                Settings = Common.Serializer.Deserialize<TenantSettings>(reader.GetString(COL_Settings))
            };
        }
        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Sec", "SecurityDBConnStringKey", "SecurityDBConnString");
        }
        #endregion
        #region ITenantDataManager
        public bool AddTenant(Tenant tenant, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();
            insertQuery.IfNotExists(TABLE_ALIAS).EqualsCondition(COL_Name).Value(tenant.Name);
            insertQuery.Column(COL_Name).Value(tenant.Name);
            if (tenant.Settings != null)
                insertQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(tenant.Settings));
            else
                insertQuery.Column(COL_Settings).Null();
            if (tenant.ParentTenantId.HasValue)
                insertQuery.Column(COL_ParentTenantID).Value(tenant.ParentTenantId.Value);
            else
                insertQuery.Column(COL_ParentTenantID).Null();
            var id = queryContext.ExecuteScalar().NullableIntValue;
            if (id.HasValue)
                insertedId = id.Value;
            else
                insertedId = -1;
            return insertedId != -1;
        }

        public bool AreTenantsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<Tenant> GetTenants()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(TenantMapper);
        }

        public bool UpdateTenant(Tenant tenant)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.NotEqualsCondition(COL_ID).Value(tenant.TenantId);
            ifNotExists.EqualsCondition(COL_Name).Value(tenant.Name);
            updateQuery.Column(COL_Name).Value(tenant.Name);
            if (tenant.Settings != null)
                updateQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(tenant.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            if (tenant.ParentTenantId.HasValue)
                updateQuery.Column(COL_ParentTenantID).Value(tenant.ParentTenantId.Value);
            else
                updateQuery.Column(COL_ParentTenantID).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(tenant.TenantId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
