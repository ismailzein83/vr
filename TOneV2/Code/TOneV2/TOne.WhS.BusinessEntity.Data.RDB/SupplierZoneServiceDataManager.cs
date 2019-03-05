using System;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SupplierZoneServiceDataManager : ISupplierZoneServiceDataManager
    {

        #region RDB

        static string TABLE_ALIAS = "spzs";
        static string TABLE_NAME = "TOneWhS_BE_SupplierZoneService";
        const string COL_ID = "ID";
        const string COL_ZoneID = "ZoneID";
        const string COL_PriceListID = "PriceListID";
        const string COL_SupplierID = "SupplierID";
        const string COL_ReceivedServicesFlag = "ReceivedServicesFlag";
        const string COL_EffectiveServiceFlag = "EffectiveServiceFlag";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";

        const string COL_StateBackupID = "StateBackupID";

        static SupplierZoneServiceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_PriceListID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_SupplierID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ReceivedServicesFlag, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_EffectiveServiceFlag, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SupplierZoneService",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        #endregion

        #region Members

        public List<SupplierZoneService> GetSupplierZonesServicesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();

            var orDateCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);
            orDateCondition.GreaterThanCondition(COL_EED).Value(minimumDate);

            whereQuery.EqualsCondition(COL_SupplierID).Value(supplierId);
            whereQuery.NotNullCondition(COL_ZoneID);

            return queryContext.GetItems(SupplierZoneServiceMapper);
        }

        public List<SupplierZoneService> GetEffectiveSupplierZoneServicesBySuppliers(IEnumerable<RoutingSupplierInfo> supplierInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

            var whereContext = selectQuery.Where();
            whereContext.ListCondition(COL_SupplierID, RDBListConditionOperator.IN, supplierInfos.Select((item => item.SupplierId)));
            whereContext.NotNullCondition(COL_ZoneID);
            BEDataUtility.SetDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, isEffectiveInFuture, effectiveOn);

            var groupByContext = selectQuery.GroupBy();
            groupByContext.Select().Columns(COL_ID, COL_ZoneID, COL_PriceListID, COL_SupplierID, COL_ReceivedServicesFlag, COL_EffectiveServiceFlag, COL_BED, COL_EED, COL_SourceID);

            return queryContext.GetItems(SupplierZoneServiceMapper);
        }

        public List<SupplierDefaultService> GetEffectiveSupplierDefaultServicesBySuppliers(IEnumerable<RoutingSupplierInfo> supplierInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.ListCondition(COL_SupplierID, RDBListConditionOperator.IN, supplierInfos.Select((item => item.SupplierId)));
            whereContext.NullCondition(COL_ZoneID);
            BEDataUtility.SetDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, isEffectiveInFuture, effectiveOn);

            return queryContext.GetItems(SupplierDefaultServiceMapper);
        }

        public SupplierDefaultService GetSupplierDefaultServiceBySupplier(int supplierId, DateTime effectiveOn)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            BEDataUtility.SetEffectiveDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn);
            whereQuery.EqualsCondition(COL_SupplierID).Value(supplierId);
            whereQuery.NullCondition(COL_ZoneID);

            return queryContext.GetItem(SupplierDefaultServiceMapper);
        }

        public bool CloseOverlappedDefaultService(long supplierZoneServiceId, SupplierDefaultService supplierDefaultService, DateTime effectiveDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_EED).Value(effectiveDate);
            updateQuery.Where().EqualsCondition(COL_ID).Value(supplierZoneServiceId);

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            insertQuery.Column(COL_ID).Value(supplierDefaultService.SupplierZoneServiceId);
            insertQuery.Column(COL_SupplierID).Value(supplierDefaultService.SupplierId);

            if (supplierDefaultService.ReceivedServices != null)
                insertQuery.Column(COL_ReceivedServicesFlag).Value(Serializer.Serialize(supplierDefaultService.ReceivedServices));

            if (supplierDefaultService.EffectiveServices != null)
                insertQuery.Column(COL_EffectiveServiceFlag).Value(Serializer.Serialize(supplierDefaultService.EffectiveServices));

            insertQuery.Column(COL_BED).Value(effectiveDate);

            return queryContext.ExecuteNonQuery(false) > 0;
        }

        public bool Insert(SupplierDefaultService supplierDefaultService)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            insertQuery.Column(COL_ID).Value(supplierDefaultService.SupplierZoneServiceId);
            insertQuery.Column(COL_SupplierID).Value(supplierDefaultService.SupplierId);

            if (supplierDefaultService.ReceivedServices != null)
                insertQuery.Column(COL_ReceivedServicesFlag).Value(Serializer.Serialize(supplierDefaultService.ReceivedServices));

            if (supplierDefaultService.EffectiveServices != null)
                insertQuery.Column(COL_EffectiveServiceFlag).Value(Serializer.Serialize(supplierDefaultService.EffectiveServices));

            insertQuery.Column(COL_BED).Value(supplierDefaultService.BED);

            if (supplierDefaultService.EED.HasValue)
                insertQuery.Column(COL_EED).Value(supplierDefaultService.EED.Value);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Update(List<SupplierZoneServiceToClose> listOfZoneServiceToClose, long effectiveZoneId, long supplierZoneServiceId, SupplierZoneServiceToEdit supplierZoneServiceToEdit)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_ID, RDBDataType.BigInt, true);
            tempTableQuery.AddColumn(COL_EED, RDBDataType.DateTime, false);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableQuery);

            foreach (var queryItem in listOfZoneServiceToClose)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_ID).Value(queryItem.SupplierZoneServiceId);
                rowContext.Column(COL_EED).Value(queryItem.CloseDate);
            }

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_EED).Column("szrsToClose", COL_EED);
            var joinContext = updateQuery.Join(TABLE_ALIAS);
            joinContext.JoinOnEqualOtherTableColumn(tempTableQuery, "szrsToClose", COL_ID, TABLE_ALIAS, COL_ID);

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            insertQuery.Column(COL_ID).Value(supplierZoneServiceId);
            insertQuery.Column(COL_SupplierID).Value(supplierZoneServiceToEdit.SupplierId);
            insertQuery.Column(COL_ZoneID).Value(effectiveZoneId);

            if (supplierZoneServiceToEdit.Services != null)
                insertQuery.Column(COL_ReceivedServicesFlag).Value(Serializer.Serialize(supplierZoneServiceToEdit.Services));

            if (supplierZoneServiceToEdit.Services != null)
                insertQuery.Column(COL_EffectiveServiceFlag).Value(Serializer.Serialize(supplierZoneServiceToEdit.Services));

            insertQuery.Column(COL_BED).Value(supplierZoneServiceToEdit.BED);

            return queryContext.ExecuteNonQuery(true) > 0;
        }

        public bool AreSupplierZoneServicesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public IEnumerable<SupplierDefaultService> GetEffectiveSupplierDefaultServices(DateTime from, DateTime to)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();

            whereQuery.LessOrEqualCondition(COL_BED).Value(to);
            var orDateCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);
            orDateCondition.GreaterThanCondition(COL_EED).Value(from);

            whereQuery.NullCondition(COL_ZoneID);

            return queryContext.GetItems(SupplierDefaultServiceMapper);
        }

        public IEnumerable<SupplierZoneService> GetEffectiveSupplierZoneServices(int supplierId, DateTime from, DateTime to)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();

            whereQuery.EqualsCondition(COL_SupplierID).Value(supplierId);
            whereQuery.LessOrEqualCondition(COL_BED).Value(to);
            var orDateCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);
            orDateCondition.GreaterThanCondition(COL_EED).Value(from);

            whereQuery.NotNullCondition(COL_ZoneID);

            return queryContext.GetItems(SupplierZoneServiceMapper);
        }

        public IEnumerable<SupplierZoneService> GetSupplierZonesServicesEffectiveAfterByZoneIds(int supplierId, DateTime effectiveDate, long zoneId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();

            var orDateCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);
            orDateCondition.GreaterThanCondition(COL_EED).Value(effectiveDate);

            whereQuery.EqualsCondition(COL_ZoneID).Value(zoneId);
            return queryContext.GetItems(SupplierZoneServiceMapper);
        }

        public List<SupplierDefaultService> GetSupplierDefaultServicesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();

            var orDateCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);
            orDateCondition.GreaterThanCondition(COL_EED).Value(minimumDate);

            whereQuery.EqualsCondition(COL_SupplierID).Value(supplierId);
            whereQuery.NullCondition(COL_ZoneID);

            selectQuery.Sort().ByColumn(TABLE_ALIAS, COL_BED, RDBSortDirection.ASC);

            return queryContext.GetItems(SupplierDefaultServiceMapper);
        }

        #endregion

        #region StateBackup

        public void BackupBySupplierId(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName, int supplierId)
        {
            var supplierZoneServiceBackupDataManager = new SupplierZoneServiceBackupDataManager();
            var insertQuery = supplierZoneServiceBackupDataManager.GetInsertQuery(queryContext, backupDatabaseName);

            var selectQuery = insertQuery.FromSelect();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

            var selectColumns = selectQuery.SelectColumns();
            selectColumns.Column(COL_ID, COL_ID);
            selectColumns.Column(COL_ZoneID, COL_ZoneID);
            selectColumns.Column(COL_PriceListID, COL_PriceListID);
            selectColumns.Column(COL_SupplierID, COL_SupplierID);
            selectColumns.Column(COL_ReceivedServicesFlag, COL_ReceivedServicesFlag);
            selectColumns.Column(COL_EffectiveServiceFlag, COL_EffectiveServiceFlag);
            selectColumns.Column(COL_BED, COL_BED);
            selectColumns.Column(COL_EED, COL_EED);
            selectColumns.Column(COL_SourceID, COL_SourceID);
            selectColumns.Expression(SupplierZoneServiceBackupDataManager.COL_StateBackupID).Value(stateBackupId);
            selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_SupplierID).Value(supplierId);
            whereContext.NotNullCondition(COL_ZoneID);
        }

        public void SetDeleteQueryBySupplierId(RDBQueryContext queryContext, int supplierId)
        {
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var joinContext = deleteQuery.Join(TABLE_ALIAS);
            string supplierZoneTableAlias = "spz";
            var supplierZoneDataManager = new SupplierZoneDataManager();
            supplierZoneDataManager.JoinSupplierZone(joinContext, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            deleteQuery.Where().EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID).Value(supplierId);
        }
        public void GetRestoreQuery(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName)
        {
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_ALIAS);
            var supplierZoneServiceBackupDataManager = new SupplierZoneServiceBackupDataManager();
            supplierZoneServiceBackupDataManager.AddSelectQuery(insertQuery, backupDatabaseName, stateBackupId);
        }
        #endregion

        #region Mappers

        SupplierZoneService SupplierZoneServiceMapper(IRDBDataReader reader)
        {
            return new SupplierZoneService
            {
                SupplierZoneServiceId = reader.GetLong(COL_ID),
                ZoneId = reader.GetLongWithNullHandling(COL_ZoneID),
                PriceListId = reader.GetIntWithNullHandling(COL_PriceListID),
                SupplierId = reader.GetInt(COL_SupplierID),
                ReceivedServices = Serializer.Deserialize<List<ZoneService>>(reader.GetString(COL_ReceivedServicesFlag)),
                EffectiveServices = Serializer.Deserialize<List<ZoneService>>(reader.GetString(COL_EffectiveServiceFlag)),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED)
            };
        }


        SupplierDefaultService SupplierDefaultServiceMapper(IRDBDataReader reader)
        {
            return new SupplierDefaultService
            {
                SupplierZoneServiceId = reader.GetLong(COL_ID),
                SupplierId = reader.GetInt(COL_SupplierID),
                PriceListId = reader.GetIntWithNullHandling(COL_PriceListID),
                ReceivedServices = Serializer.Deserialize<List<ZoneService>>(reader.GetString(COL_ReceivedServicesFlag)),
                EffectiveServices = Serializer.Deserialize<List<ZoneService>>(reader.GetString(COL_EffectiveServiceFlag)),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED)
            };
        }

        #endregion

    }
}
