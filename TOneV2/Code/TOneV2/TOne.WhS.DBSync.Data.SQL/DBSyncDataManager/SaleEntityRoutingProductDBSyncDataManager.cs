using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SaleEntityRoutingProductDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SaleEntityRoutingProduct);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SaleEntityRoutingProductDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnStringKey"))
        {
            _UseTempTables = useTempTables;
        }
        static string[] s_columns = new string[] { "ID", "OwnerType", "OwnerID", "ZoneID", "RoutingProductID", "BED", "EED" };

        public void ApplySaleEntityRoutingProductsToTemp(List<SaleEntityRoutingProduct> saleEntityRoutingProducts, long startingId)
        {
            //DataTable dt = new DataTable();
            //dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            //dt.Columns.Add("OwnerType", typeof(byte));
            //dt.Columns.Add("OwnerId", typeof(int));
            //dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "SaleZoneId", DataType = typeof(long) });
            //dt.Columns.Add("RoutingProductId", typeof(int));
            //dt.Columns.Add("BED", typeof(DateTime));
            //dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "EED", DataType = typeof(DateTime) });
            //dt.BeginLoadData();
            //foreach (var item in saleEntityRoutingProducts)
            //{
            //    DataRow row = dt.NewRow();
            //    int index = 0;
            //    row[index++] = item.OwnerType;
            //    row[index++] = item.OwnerId;
            //    if (item.SaleZoneId == null)
            //        row[index++] = DBNull.Value;
            //    else
            //    row[index++] = item.SaleZoneId;
            //    row[index++] = item.RoutingProductId;
            //    row[index++] = item.BED;
            //    if (item.EED == null)
            //        row[index++] = DBNull.Value;
            //    else
            //        row[index++] = item.EED; ;
            //    dt.Rows.Add(row);
            //}
            //dt.EndLoadData();
            //WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);


            var stream = base.InitializeStreamForBulkInsert();
            foreach (var item in saleEntityRoutingProducts)
            {
                stream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}", startingId++, (short)item.OwnerType, item.OwnerId, item.SaleZoneId.HasValue ? item.SaleZoneId.Value : default(long?), item.RoutingProductId, GetDateTimeForBCP(item.BED), item.EED.HasValue ? GetDateTimeForBCP(item.EED.Value) : "");
            }
            stream.Close();
            StreamBulkInsertInfo streamBulkInsert = new StreamBulkInsertInfo
            {
                Stream = stream,
                ColumnNames = s_columns,
                FieldSeparator = '^',
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                TabLock = true
            };
            base.InsertBulkToTable(streamBulkInsert);
        }

        public string GetConnection()
        {
            return base.GetConnectionString();
        }

        public string GetTableName()
        {
            return _TableName;
        }

        public string GetSchema()
        {
            return _Schema;
        }
    }
}
