using Retail.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Data.RDB
{
    public class AccountPackageUsageVolumeBalanceDataManager : IAccountPackageUsageVolumeBalanceDataManager
    {
        public static string TABLE_NAME = "Retail_CDR_AccountPackageUsageVolumeBalance";
        public static string TABLE_ALIAS = "apuvb";

        public const string COL_ID = "ID";
        public const string COL_AccountPackageId = "AccountPackageId";
        public const string COL_PackageItemId = "PackageItemId";
        public const string COL_FromTime = "FromTime";
        public const string COL_ToTime = "ToTime";
        public const string COL_ItemVolume = "ItemVolume";
        public const string COL_UsedVolume = "UsedVolume";
        public const string COL_CreatedTime = "CreatedTime";
        public const string COL_LastModifiedTime = "LastModifiedTime";

        static AccountPackageUsageVolumeBalanceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_AccountPackageId, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_PackageItemId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_FromTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ToTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ItemVolume, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 18, Precision = 0 });
            columns.Add(COL_UsedVolume, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 18, Precision = 0 });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "Retail_CDR",
                DBTableName = "AccountPackageUsageVolumeBalance",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        public List<AccountPackageUsageVolumeBalance> GetAccountPackageUsageVolumeBalancesByKeys(HashSet<PackageUsageVolumeBalanceKey> volumeBalanceKeys)
        {
            if (volumeBalanceKeys == null || volumeBalanceKeys.Count == 0)
                return null;

            string volumeBalanceKeysTempTableAlias = "volBalKeyTempTable";

            var queryContext = new RDBQueryContext(GetDataProvider());

            var volumeBalanceKeysTempTable = queryContext.CreateTempTable();
            volumeBalanceKeysTempTable.AddColumn(COL_AccountPackageId, RDBDataType.BigInt);
            volumeBalanceKeysTempTable.AddColumn(COL_PackageItemId, RDBDataType.UniqueIdentifier);
            volumeBalanceKeysTempTable.AddColumn(COL_FromTime, RDBDataType.DateTime);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(volumeBalanceKeysTempTable);

            foreach (var volumeBalanceKey in volumeBalanceKeys)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_AccountPackageId).Value(volumeBalanceKey.AccountPackageId);
                rowContext.Column(COL_PackageItemId).Value(volumeBalanceKey.PackageItemId);
                rowContext.Column(COL_FromTime).Value(volumeBalanceKey.ItemFromTime);
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            var joinCondition = joinContext.Join(volumeBalanceKeysTempTable, volumeBalanceKeysTempTableAlias).On();
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_AccountPackageId, volumeBalanceKeysTempTableAlias, COL_AccountPackageId);
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_PackageItemId, volumeBalanceKeysTempTableAlias, COL_PackageItemId);
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_FromTime, volumeBalanceKeysTempTableAlias, COL_FromTime);

            return queryContext.GetItems(AccountPackageUsageVolumeBalanceMapper);
        }

        public void AddAccountPackageUsageVolumeBalance(List<AccountPackageUsageVolumeBalanceToAdd> accountPackageUsageVolumeBalances)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(TABLE_NAME);

            foreach (var accountPackageUsageVolumeBalance in accountPackageUsageVolumeBalances)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_AccountPackageId).Value(accountPackageUsageVolumeBalance.AccountPackageId);
                rowContext.Column(COL_PackageItemId).Value(accountPackageUsageVolumeBalance.PackageItemId);
                rowContext.Column(COL_FromTime).Value(accountPackageUsageVolumeBalance.FromTime);
                rowContext.Column(COL_ToTime).Value(accountPackageUsageVolumeBalance.ToTime);
                rowContext.Column(COL_ItemVolume).Value(accountPackageUsageVolumeBalance.ItemVolume);
                rowContext.Column(COL_UsedVolume).Value(accountPackageUsageVolumeBalance.UsedVolume);
            }

            queryContext.ExecuteNonQuery();
        }

        public void UpdateAccountPackageUsageVolumeBalance(List<AccountPackageUsageVolumeBalanceToUpdate> accountPackageUsageVolumeBalances)
        {
            if (accountPackageUsageVolumeBalances == null || accountPackageUsageVolumeBalances.Count == 0)
                return;

            var usedVolumeTempTableAlias = "usedVolumeTempTable";

            var queryContext = new RDBQueryContext(GetDataProvider());

            var usedVolumeTempTable = queryContext.CreateTempTable();
            usedVolumeTempTable.AddColumn(COL_ID, RDBDataType.BigInt);
            usedVolumeTempTable.AddColumn(COL_UsedVolume, RDBDataType.Decimal, 18, 0);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(usedVolumeTempTable);

            foreach (var accountPackageUsageVolumeBalance in accountPackageUsageVolumeBalances)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_ID).Value(accountPackageUsageVolumeBalance.AccountPackageVolumeBalanceId);
                rowContext.Column(COL_UsedVolume).Value(accountPackageUsageVolumeBalance.UsedVolume);
            }

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var joinContext = updateQuery.Join(TABLE_ALIAS);
            joinContext.JoinOnEqualOtherTableColumn(usedVolumeTempTable, usedVolumeTempTableAlias, COL_ID, TABLE_ALIAS, COL_ID);

            updateQuery.Column(COL_UsedVolume).Column(usedVolumeTempTableAlias, COL_UsedVolume);

          

            queryContext.ExecuteNonQuery();
        }

        private BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Retail_CDR", "RetailCDRDBConnStringKey", "RetailCDRDBConnString");
        }

        private AccountPackageUsageVolumeBalance AccountPackageUsageVolumeBalanceMapper(IRDBDataReader reader)
        {
            return new AccountPackageUsageVolumeBalance
            {
                AccountPackageUsageVolumeBalanceId = reader.GetLong(COL_ID),
                AccountPackageId = reader.GetLong(COL_AccountPackageId),
                PackageItemId = reader.GetGuid(COL_PackageItemId),
                FromTime = reader.GetDateTime(COL_FromTime),
                ToTime = reader.GetDateTime(COL_ToTime),
                ItemVolume = reader.GetDecimal(COL_ItemVolume),
                UsedVolume = reader.GetDecimal(COL_UsedVolume)
            };
        }
    }
}