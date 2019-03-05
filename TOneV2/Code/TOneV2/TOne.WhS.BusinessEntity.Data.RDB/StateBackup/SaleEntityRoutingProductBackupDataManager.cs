using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SaleEntityRoutingProductBackupDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "rpbkup";
        static string TABLE_NAME = "TOneWhS_BE_Bkup_SaleEntityRoutingProduct";
        const string COL_ID = "ID";
        const string COL_OwnerType = "OwnerType";
        const string COL_OwnerID = "OwnerID";
        const string COL_ZoneID = "ZoneID";
        const string COL_RoutingProductID = "RoutingProductID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_LastModifiedTime = "LastModifiedTime";

        internal const string COL_StateBackupID = "StateBackupID";

        static SaleEntityRoutingProductBackupDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_OwnerType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_OwnerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_RoutingProductID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_StateBackupID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE_Bkup",
                DBTableName = "SaleEntityRoutingProduct",
                Columns = columns,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        #endregion

        #region Public Methods

        public RDBInsertQuery GetInsertQuery(RDBQueryContext queryContext, string backupDatabaseName)
        {
            var insertCustomerQuery = queryContext.AddInsertQuery();
            insertCustomerQuery.IntoTable(new RDBTableDefinitionQuerySource(backupDatabaseName, TABLE_NAME));
            return insertCustomerQuery;
        }

        public void AddDefaultSelectQuery(RDBInsertQuery insertQuery, string backupDataBaseName, long stateBackupId)
        {
            var selectQuery = insertQuery.FromSelect();
            selectQuery.From(new RDBTableDefinitionQuerySource(backupDataBaseName, TABLE_NAME), TABLE_ALIAS, null, true);

            var selectColumns = selectQuery.SelectColumns();

            selectColumns.Column(COL_ID, COL_ID);
            selectColumns.Column(COL_OwnerType, COL_OwnerType);
            selectColumns.Column(COL_OwnerID, COL_OwnerID);
            selectColumns.Column(COL_ZoneID, COL_ZoneID);
            selectColumns.Column(COL_RoutingProductID, COL_RoutingProductID);
            selectColumns.Column(COL_BED, COL_BED);
            selectColumns.Column(COL_EED, COL_EED);
            selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

            var whereContext = selectQuery.Where();
            whereContext.NullCondition(COL_ZoneID);
            whereContext.EqualsCondition(COL_StateBackupID).Value(stateBackupId);
        }
        public void AddSelectQuery(RDBInsertQuery insertQuery, string backupDataBaseName, long stateBackupId)
        {
            var selectQuery = insertQuery.FromSelect();
            selectQuery.From(new RDBTableDefinitionQuerySource(backupDataBaseName, TABLE_NAME), TABLE_ALIAS, null, true);

            var selectColumns = selectQuery.SelectColumns();

            selectColumns.Column(COL_ID, COL_ID);
            selectColumns.Column(COL_OwnerType, COL_OwnerType);
            selectColumns.Column(COL_OwnerID, COL_OwnerID);
            selectColumns.Column(COL_ZoneID, COL_ZoneID);
            selectColumns.Column(COL_RoutingProductID, COL_RoutingProductID);
            selectColumns.Column(COL_BED, COL_BED);
            selectColumns.Column(COL_EED, COL_EED);
            selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

            var joinContext = selectQuery.Join();
            var saleZoneDataManager = new SaleZoneDataManager();
            saleZoneDataManager.JoinSaleZone(joinContext, "sz", TABLE_ALIAS, COL_ZoneID, true);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_StateBackupID).Value(stateBackupId);
    }
        #endregion
    }
}
