using System;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data.RDB.StateBackup;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SupplierCodeDataManager : ISupplierCodeDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "sc";
        static string TABLE_NAME = "TOneWhS_BE_SupplierCode";
        const string COL_ID = "ID";
        const string COL_Code = "Code";
        const string COL_ZoneID = "ZoneID";
        const string COL_CodeGroupID = "CodeGroupID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";

        static SupplierCodeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 20 });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CodeGroupID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SupplierCode",
                Columns = columns,
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

        public IEnumerable<SupplierCode> GetParentsBySupplier(int supplierId, string codeNumber)
        {
            var supplierZoneDataManager = new SupplierZoneDataManager();
            string supplierZoneTableAlias = "spz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinCondition = selectQuery.Join();
            supplierZoneDataManager.JoinSupplierZone(joinCondition, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, false);

            var whereContext = selectQuery.Where();

            BEDataUtility.SetParentCodeCondition(whereContext, codeNumber, TABLE_ALIAS, COL_Code);

            whereContext.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID).Value(supplierId);

            BEDataUtility.SetEffectiveDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, DateTime.Now);

            selectQuery.Sort().ByColumn(COL_Code, RDBSortDirection.ASC);
            return queryContext.GetItems(SupplierCodeMapper);
        }

        public IEnumerable<SupplierCode> GetFilteredSupplierCodes(SupplierCodeQuery query)
        {
            var supplierZoneDataManager = new SupplierZoneDataManager();
            string supplierZoneTableAlias = "spz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinCondition = selectQuery.Join();
            supplierZoneDataManager.JoinSupplierZone(joinCondition, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID).Value(query.SupplierId);

            if (!string.IsNullOrEmpty(query.Code))
                whereContext.StartsWithCondition(COL_Code, query.Code);

            if (query.ZoneIds != null && query.ZoneIds.Any())
                whereContext.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, query.ZoneIds);

            BEDataUtility.SetEffectiveDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, query.EffectiveOn);

            return queryContext.GetItems(SupplierCodeMapper);
        }

        public List<SupplierCode> GetSupplierCodesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            SupplierZoneDataManager supplierZoneDataManager = new SupplierZoneDataManager();
            string supplierZoneTableAlias = "spz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinCondition = selectQuery.Join();
            supplierZoneDataManager.JoinSupplierZone(joinCondition, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID).Value(supplierId);

            BEDataUtility.SetEffectiveAfterDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, minimumDate);

            return queryContext.GetItems(SupplierCodeMapper);
        }

        public List<SupplierCode> GetSupplierCodes(DateTime from, DateTime to)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.LessOrEqualCondition(COL_BED).Value(to);

            var orDateCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);
            orDateCondition.GreaterThanCondition(COL_EED).Value(from);

            return queryContext.GetItems(SupplierCodeMapper);
        }

        public IEnumerable<CodePrefixInfo> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            BEDataUtility.SetDistinctCodePrefixesQuery(queryContext, TABLE_NAME, TABLE_ALIAS, COL_BED, COL_EED, isFuture, effectiveOn, prefixLength, COL_Code);
            return queryContext.GetItems(CodePrefixMapper);
        }

        public IEnumerable<CodePrefixInfo> GetSpecificCodeByPrefixes(int prefixLength, IEnumerable<string> codePrefixes, DateTime? effectiveOn, bool isFuture)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            BEDataUtility.SetCodePrefixQuery(queryContext, TABLE_NAME, TABLE_ALIAS, COL_BED, COL_EED, isFuture, effectiveOn, prefixLength, COL_Code, codePrefixes);
            return queryContext.GetItems(CodePrefixMapper);
        }

        public bool AreSupplierCodesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public void LoadSupplierCodes(IEnumerable<RoutingSupplierInfo> activeSupplierInfo, string codePrefix, DateTime? effectiveOn, bool isFuture, Func<bool> shouldStop, Action<SupplierCode> onCodeLoaded)
        {
            var supplierZoneDataManager = new SupplierZoneDataManager();

            string supplierZoneTableAlias = "spz";
            var queryContext = new RDBQueryContext(GetDataProvider());

            var activeSuppliersInfoTempTableQuery = queryContext.CreateTempTable();
            activeSuppliersInfoTempTableQuery.AddColumn(SupplierZoneDataManager.COL_SupplierID, RDBDataType.Int, true);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(activeSuppliersInfoTempTableQuery);

            foreach (var queryItem in activeSupplierInfo)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(SupplierZoneDataManager.COL_SupplierID).Value(queryItem.SupplierId);
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinCondition = selectQuery.Join();
            supplierZoneDataManager.JoinSupplierZone(joinCondition, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);
            var supplierJoinStatement = joinCondition.Join(activeSuppliersInfoTempTableQuery, "supplierInfo");
            var supplierJoinConditionOn = supplierJoinStatement.On();
            supplierJoinConditionOn.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID, "supplierInfo", SupplierZoneDataManager.COL_SupplierID);

            var whereContext = selectQuery.Where();
            whereContext.StartsWithCondition(COL_Code, codePrefix);
            BEDataUtility.SetDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, isFuture, effectiveOn);

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    if (shouldStop != null && shouldStop())
                        break;

                    onCodeLoaded(SupplierCodeMapper(reader));
                }
            });
        }
        public List<ZoneCodeGroup> GetCostZoneCodeGroups(DateTime? effectiveOn, bool isFuture)
        {
            CodeGroupDataManager codeGroupDataManager = new CodeGroupDataManager();
            Dictionary<long, List<string>> codeGroupsByZone = new Dictionary<long, List<string>>();

            string codeGroupTableAlias = "cg";

            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            var selectColoumns = selectQuery.SelectColumns();
            selectColoumns.Column(COL_ZoneID);
            selectColoumns.Column(TABLE_ALIAS, COL_Code, "CodeGroup");

            var joinCondition = selectQuery.Join();
            codeGroupDataManager.JoinCodeGroup(joinCondition, codeGroupTableAlias, TABLE_ALIAS, COL_CodeGroupID);

            var whereContext = selectQuery.Where();

            BEDataUtility.SetDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, isFuture, effectiveOn);

            whereContext.EqualsCondition(TABLE_ALIAS, COL_Code).Column(codeGroupTableAlias, CodeGroupDataManager.COL_Code);

            queryContext.ExecuteReader(
               (reader) =>
               {
                   while (reader.Read())
                   {
                       long zoneId = reader.GetLong(COL_ZoneID);
                       string codeGroup = reader.GetString("CodeGroup");
                       List<string> codeGroups = codeGroupsByZone.GetOrCreateItem(zoneId);
                       codeGroups.Add(codeGroup);
                   }
               });

            return codeGroupsByZone.Select(itm => new ZoneCodeGroup() { CodeGroups = itm.Value, ZoneId = itm.Key, IsSale = true }).ToList();
        }

        #endregion

        #region Mappers
        SupplierCode SupplierCodeMapper(IRDBDataReader reader)
        {
            return new SupplierCode
            {
                Code = reader.GetString(COL_Code),
                SupplierCodeId = reader.GetLong(COL_ID),
                ZoneId = reader.GetLong(COL_ZoneID),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                CodeGroupId = reader.GetIntWithNullHandling(COL_CodeGroupID),
                SourceId = reader.GetString(COL_SourceID)
            };
        }
        CodePrefixInfo CodePrefixMapper(IRDBDataReader reader)
        {
            return new CodePrefixInfo
            {
                CodePrefix = reader.GetString("CodePrefix"),
                Count = reader.GetInt("codeCount")
            };
        }

        #endregion

        #region Public Methods
        public bool HasSupplierCodesByCodeGroup(int codeGroupId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
            selectQuery.SelectColumns().Column(COL_ID);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_CodeGroupID).Value(codeGroupId);

            return queryContext.ExecuteScalar().NullableLongValue.HasValue;
        }

        #endregion

        #region StateBackup
        public void BackupBySupplierId(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName, int supplierId)
        {
            var supplierCodeBackupDataManager = new SupplierCodeBackupDataManager();
            var insertQuery = supplierCodeBackupDataManager.GetInsertQuery(queryContext, backupDatabaseName);

            var selectQuery = insertQuery.FromSelect();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

            var selectColumns = selectQuery.SelectColumns();
            selectColumns.Column(COL_ID, COL_ID);
            selectColumns.Column(COL_Code, COL_Code);
            selectColumns.Column(COL_ZoneID, COL_ZoneID);
            selectColumns.Column(COL_CodeGroupID, COL_CodeGroupID);
            selectColumns.Column(COL_BED, COL_BED);
            selectColumns.Column(COL_EED, COL_EED);
            selectColumns.Column(COL_SourceID, COL_SourceID);
            selectColumns.Expression(SupplierCodeBackupDataManager.COL_StateBackupID).Value(stateBackupId);
            selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

            var joinContext = selectQuery.Join();
            string supplierZoneTableAlias = "spz";
            var supplierZoneDataManager = new SupplierZoneDataManager();
            supplierZoneDataManager.JoinSupplierZone(joinContext, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID).Value(supplierId);
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
            var supplierCodeBackupDataManager = new SupplierCodeBackupDataManager();
            supplierCodeBackupDataManager.AddSelectQuery(insertQuery, backupDatabaseName, stateBackupId);
        }
        #endregion

    }
}
