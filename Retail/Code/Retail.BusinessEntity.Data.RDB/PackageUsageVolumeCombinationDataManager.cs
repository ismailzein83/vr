using Retail.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Data.RDB
{
    public class PackageUsageVolumeCombinationDataManager : IPackageUsageVolumeCombinationDataManager
    {
        public static string TABLE_NAME = "Retail_CDR_PackageUsageVolumeCombination";
        public static string TABLE_ALIAS = "puvc";

        public const string COL_ID = "ID";
        public const string COL_Combination = "Combination";
        public const string COL_CreatedTime = "CreatedTime";
        public const string COL_LastModifiedTime = "LastModifiedTime";

        static PackageUsageVolumeCombinationDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Combination, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "Retail_CDR",
                DBTableName = "PackageUsageVolumeCombination",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        public List<PackageUsageVolumeCombination> GetAllPackageUsageVolumeCombinations()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectquery = queryContext.AddSelectQuery();
            selectquery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectquery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(PackageUsageVolumeCombinationMapper);
        }

        public Dictionary<int, PackageUsageVolumeCombination> GetPackageUsageVolumeCombinationAfterID(int id)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.GreaterThanCondition(COL_ID).Value(id);

            List<PackageUsageVolumeCombination> packageUsageVolumeCombinationsList = queryContext.GetItems(PackageUsageVolumeCombinationMapper);

            Dictionary<int, PackageUsageVolumeCombination> packageUsageVolumeCombinations = new Dictionary<int, PackageUsageVolumeCombination>();

            foreach (var packageUsageVolumeCombination in packageUsageVolumeCombinationsList)
                packageUsageVolumeCombinations.Add(packageUsageVolumeCombination.PackageUsageVolumeCombinationId, packageUsageVolumeCombination);

            return packageUsageVolumeCombinations.Count > 0 ? packageUsageVolumeCombinations : null;
        }

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', COL_ID, COL_Combination);
            return streamForBulkInsert;
        }

        public void WriteRecordToStream(PackageUsageVolumeCombination record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();
            recordContext.Value(record.PackageUsageVolumeCombinationId);
            recordContext.Value(Retail.BusinessEntity.Entities.Helper.SerializePackageCombinations(record.PackageItemsByPackageId));
        }

        public void ApplyPackageUsageVolumeCombinationForDB(object preparedPackageUsageVolumeCombination)
        {
            preparedPackageUsageVolumeCombination.CastWithValidate<RDBBulkInsertQueryContext>("preparedPackageUsageVolumeCombination").Apply();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }

        #region Private Methods

        private BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Retail_CDR", "RetailCDRDBConnStringKey", "RetailCDRDBConnString");
        }

        private PackageUsageVolumeCombination PackageUsageVolumeCombinationMapper(IRDBDataReader reader)
        {
            string packageCombinations = reader.GetString(COL_Combination);
            packageCombinations.ThrowIfNull("packageCombinations");

            return new PackageUsageVolumeCombination
            {
                PackageUsageVolumeCombinationId = reader.GetInt(COL_ID),
                PackageItemsByPackageId = Helper.DeserializePackageCombinations(packageCombinations)
            };
        }

        #endregion
    }
}