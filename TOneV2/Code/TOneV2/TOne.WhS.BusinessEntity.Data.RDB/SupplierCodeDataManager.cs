using System;
using Vanrise.Entities;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using System.Linq;
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
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_Code, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 20}},
                {COL_ZoneID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_CodeGroupID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_EED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_SourceID, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 50}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_CreatedTime, new RDBTableColumnDefinition{DataType = RDBDataType.DateTime}}
        };
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

        #region ISupplierCodeDataManager Members

        public IEnumerable<SupplierCode> GetParentsBySupplier(int supplierId, string codeNumber)
        {
            SupplierZoneDataManager supplierZoneDataManager = new SupplierZoneDataManager();
            string supplierZoneTableAlias = "spz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinCondition = selectQuery.Join();
            supplierZoneDataManager.JoinSupplierZone(joinCondition, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID);

            var whereQuery = selectQuery.Where();
            whereQuery.ContainsCondition(COL_Code, codeNumber);
            whereQuery.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID).Value(supplierId);
            whereQuery.LessOrEqualCondition(COL_BED).DateNow();

            var orDateCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);
            orDateCondition.GreaterThanCondition(COL_EED).DateNow();

            return queryContext.GetItems(SupplierCodeMapper);
        }

        public IEnumerable<SupplierCode> GetFilteredSupplierCodes(SupplierCodeQuery query)
        {
            SupplierZoneDataManager supplierZoneDataManager = new SupplierZoneDataManager();
            string supplierZoneTableAlias = "spz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinCondition = selectQuery.Join();
            supplierZoneDataManager.JoinSupplierZone(joinCondition, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID).Value(query.SupplierId);

            if (string.IsNullOrEmpty(query.Code))
                whereQuery.ContainsCondition(COL_Code, query.Code);

            if (query.ZoneIds != null && query.ZoneIds.Any())
                whereQuery.ListCondition(RDBListConditionOperator.IN, query.ZoneIds);

            BEDataUtility.SetEffectiveAfterDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, query.EffectiveOn);

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
            supplierZoneDataManager.JoinSupplierZone(joinCondition, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID).Value(supplierId);

            var ordDateCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            ordDateCondition.NullCondition(COL_EED);
            ordDateCondition.GreaterThanCondition(COL_EED).Value(minimumDate);
            ordDateCondition.NotEqualsCondition(COL_EED).Column(COL_BED);

            return queryContext.GetItems(SupplierCodeMapper);
        }

        public List<SupplierCode> GetSupplierCodes(DateTime from, DateTime to)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            whereQuery.LessOrEqualCondition(COL_BED).Value(from);

            var orDateCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orDateCondition.NullCondition(COL_EED);
            orDateCondition.GreaterThanCondition(COL_EED).Value(to);

            return queryContext.GetItems(SupplierCodeMapper);
        }

        public IEnumerable<CodePrefixInfo> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            if (effectiveOn.HasValue)
            {
                if (isFuture)
                    BEDataUtility.SetEffectiveAfterDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
                else
                    BEDataUtility.SetFutureDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
            }
            else
                whereQuery.FalseCondition();

            var groupByContext = selectQuery.GroupBy();
            var groupSelect = groupByContext.Select();
            groupSelect.Expression(TABLE_ALIAS).TextLeftPart(prefixLength).Column(COL_Code);
            groupByContext.SelectAggregates().Count("codeCount");

            selectQuery.Sort().ByAlias("codeCount", RDBSortDirection.DESC);

            return queryContext.GetItems(CodePrefixMapper);
        }

        public IEnumerable<CodePrefixInfo> GetSpecificCodeByPrefixes(int prefixLength, IEnumerable<string> codePrefixes, DateTime? effectiveOn, bool isFuture)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            string codePrefixAlias = "CodePrefix";
            string codeCountAlias = "CodeCount";
            string allPrefixesTableAlias = "allPrefixes";

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumnsFromTable(codePrefixAlias);
            tempTableQuery.AddColumnsFromTable(codeCountAlias);

            var insertToTempTableQuery = queryContext.AddInsertQuery();
            insertToTempTableQuery.IntoTable(tempTableQuery);

            var fromSelectQuery = insertToTempTableQuery.FromSelect();

            fromSelectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            fromSelectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = fromSelectQuery.Where();
            if (effectiveOn.HasValue)
            {
                if (isFuture)
                    BEDataUtility.SetEffectiveAfterDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
                else
                    BEDataUtility.SetFutureDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
            }
            else
                whereQuery.FalseCondition();

            var groupByContext = fromSelectQuery.GroupBy();
            var groupSelect = groupByContext.Select();

            groupSelect.Expression(TABLE_ALIAS).TextLeftPart(prefixLength).Column(COL_Code);
            groupByContext.SelectAggregates().Count(codeCountAlias);


            var tempCodePrefixesTableQuery = queryContext.CreateTempTable();
            tempCodePrefixesTableQuery.AddColumn(codePrefixAlias, RDBDataType.NVarchar, true);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempCodePrefixesTableQuery);

            foreach (var queryItem in codePrefixes)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(codePrefixAlias).Value(queryItem);
            }


            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(tempCodePrefixesTableQuery, allPrefixesTableAlias);
            selectQuery.SelectColumns().AllTableColumns(allPrefixesTableAlias);

            var joinContext = selectQuery.Join();
            var joinStatement = joinContext.Join(tempCodePrefixesTableQuery, "cp");
            joinStatement.JoinType(RDBJoinType.Inner);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(allPrefixesTableAlias, codePrefixAlias).TextLeftPart(prefixLength - 1).Column("cp", codePrefixAlias);

            return queryContext.GetItems(CodePrefixMapper);
        }

        public bool AreSupplierCodesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public void LoadSupplierCodes(IEnumerable<RoutingSupplierInfo> activeSupplierInfo, string codePrefix, DateTime? effectiveOn, bool isFuture, Func<bool> shouldStop, Action<SupplierCode> onCodeLoaded)
        {
            //TODO test it in routing
            SupplierZoneDataManager supplierZoneDataManager = new SupplierZoneDataManager();

            string supplierZoneTableAlias = "spz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(SupplierZoneDataManager.COL_SupplierID, RDBDataType.Int, true);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableQuery);

            foreach (var queryItem in activeSupplierInfo)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(SupplierZoneDataManager.COL_SupplierID).Value(queryItem.SupplierId);
            }

            var joinCondition = selectQuery.Join();
            supplierZoneDataManager.JoinSupplierZone(joinCondition, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID);

            var supplierJoinCondition = selectQuery.Join();
            var supplierJoinStatement = supplierJoinCondition.Join(tempTableQuery, "supplierInfo");
            supplierJoinStatement.JoinType(RDBJoinType.Inner);
            var supplierJoinConditionOn = supplierJoinStatement.On();
            supplierJoinConditionOn.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID, "supplierInfo", SupplierZoneDataManager.COL_SupplierID);

            var whereQuery = selectQuery.Where();

            if (effectiveOn.HasValue)
            {
                if (isFuture)
                    BEDataUtility.SetEffectiveAfterDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
                else
                    BEDataUtility.SetFutureDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
            }
            else whereQuery.FalseCondition();

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

        #region State Backup Methods

        public string BackupAllDataBySupplierId(long stateBackupId, string backupDatabase, int supplierId)
        {


            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SupplierCode] WITH (TABLOCK)
                                            SELECT sc.[ID], sc.[Code], sc.[ZoneID], sc.[CodeGroupID], sc.[BED], sc.[EED], sc.[SourceID],  {1} AS StateBackupID,sc.[LastModifiedTime]  FROM [TOneWhS_BE].[SupplierCode] sc
                                            WITH (NOLOCK)  Inner Join [TOneWhS_BE].[SupplierZone] sz  WITH (NOLOCK)  on sz.ID = sc.ZoneID
                                            Where sz.SupplierID = {2}", backupDatabase, stateBackupId, supplierId);
        }

        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[SupplierCode] ([ID], [Code], [ZoneID], [CodeGroupID], [BED], [EED], [SourceID],[LastModifiedTime])
                                            SELECT [ID], [Code], [ZoneID], [CodeGroupID], [BED], [EED], [SourceID],[LastModifiedTime] FROM [{0}].[TOneWhS_BE_Bkup].[SupplierCode]
                                            WITH (NOLOCK) Where StateBackupID = {1} ", backupDatabase, stateBackupId);
        }

        public void GetDeleteCommandsBySupplierId(RDBQueryContext queryContext, int supplierId)
        {
            SupplierZoneDataManager supplierZoneDataManager = new SupplierZoneDataManager();
            string supplierZoneTableAlias = "splz";

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var joinContext = deleteQuery.Join(TABLE_ALIAS);
            supplierZoneDataManager.JoinSupplierZone(joinContext, supplierZoneTableAlias, TABLE_ALIAS, COL_ZoneID);

            var whereContext = deleteQuery.Where();
            whereContext.EqualsCondition(supplierZoneTableAlias, SupplierZoneDataManager.COL_SupplierID).Value(supplierId);

        }

        #endregion

    }
}
