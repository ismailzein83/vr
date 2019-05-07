//using Retail.BusinessEntity.Entities;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using Vanrise.Data.SQL;

//namespace Retail.BusinessEntity.Data.SQL
//{
//    public class PackageUsageVolumeCombinationDataManager : BaseSQLDataManager, IPackageUsageVolumeCombinationDataManager
//    {
//        #region Properties/Ctor    
//        readonly string[] columns = { "ID", "Combination" };

//        public PackageUsageVolumeCombinationDataManager()
//           : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "Retail_BE_DBConnString"))
//        {

//        }
//        #endregion

//        #region Public Methods

//        public List<PackageUsageVolumeCombination> GetAllPackageUsageVolumeCombinations()
//        {
//            List<PackageUsageVolumeCombination> packageUsageVolumeCombination = new List<PackageUsageVolumeCombination>();

//            string query = string.Format(query_GetRouteCases.Replace("#FILTER#", ""), SwitchId);
//            ExecuteReaderText(query, (reader) =>
//            {
//                while (reader.Read())
//                {
//                    RouteCase routeCase = RouteCaseMapper(reader);
//                    routeCases.Add(routeCase);
//                }
//            }, null);

//            return routeCases.Count > 0 ? routeCases : null;
//        }


//        public Dictionary<string, PackageUsageVolumeCombination> GetPackageUsageVolumeCombinationAfterID(int id)
//        {
//            Dictionary<string, PackageUsageVolumeCombination> packageUsageVolumeCombinations = new Dictionary<string, PackageUsageVolumeCombination>();

//            string query = string.Format(query_GetPackageUsageVolumeCombinations.Replace("#FILTER#", " WHERE ID > {0}"), id);
//            ExecuteReaderText(query, (reader) =>
//            {
//                while (reader.Read())
//                {
//                    PackageUsageVolumeCombination packageUsageVolumeCombination = PackageUsageVolumeCombinationMapper(reader);
//                    var serializedPackageCombination = Retail.BusinessEntity.Entities.Helper.SerializePackageCombinations(packageUsageVolumeCombination.PackageItemsByPackageId);
//                    packageUsageVolumeCombinations.Add(serializedPackageCombination, packageUsageVolumeCombination);
//                }
//            }, null);

//            return packageUsageVolumeCombinations;
//        }

//        public void ApplyPackageUsageVolumeCombinationForDB(object preparedPackageUsageVolumeCombination)
//        {
//            InsertBulkToTable(preparedPackageUsageVolumeCombination as BaseBulkInsertInfo);
//        }

//        #endregion

//        #region Private Methods
//        private PackageUsageVolumeCombination PackageUsageVolumeCombinationMapper(IDataReader reader)
//        {
//            return new PackageUsageVolumeCombination
//            {
//                PackageUsageVolumeCombinationId = (int)reader["ID"],
//                PackageItemsByPackageId = reader["Combination"] as string != null ? Retail.BusinessEntity.Entities.Helper.DeserializePackageCombinations(reader["Combination"] as string) : null
//            };
//        }

//        #endregion

//        #region Queries
//        const string query_GetPackageUsageVolumeCombinations = @"SELECT ID, Combination
//                                                                 FROM [Retail_BE].[PackageUsageVolumeCombination]
//                                                                 #FILTER#";
//        #endregion

//        #region IBulkApplyDataManager

//        public object InitialiazeStreamForDBApply()
//        {
//            return base.InitializeStreamForBulkInsert();
//        }

//        public void WriteRecordToStream(PackageUsageVolumeCombination record, object dbApplyStream)
//        {
//            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
//            streamForBulkInsert.WriteRecord("{0}^{1}", record.PackageUsageVolumeCombinationId, Retail.BusinessEntity.Entities.Helper.SerializePackageCombinations(record.PackageItemsByPackageId));
//        }

//        public object FinishDBApplyStream(object dbApplyStream)
//        {
//            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
//            streamForBulkInsert.Close();
//            return new StreamBulkInsertInfo
//            {
//                TableName = "[Retail_BE].[PackageUsageVolumeCombination]",
//                Stream = streamForBulkInsert,
//                TabLock = true,
//                KeepIdentity = false,
//                FieldSeparator = '^',
//                ColumnNames = columns
//            };
//        }

//        #endregion
//    }
//}
