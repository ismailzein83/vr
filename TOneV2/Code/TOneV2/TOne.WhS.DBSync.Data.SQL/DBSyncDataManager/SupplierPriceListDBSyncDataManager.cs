using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SupplierPriceListDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SupplierPriceList);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SupplierPriceListDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySupplierPriceListsToTemp(List<SupplierPriceList> supplierPriceList)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("SupplierID", typeof(int));
            dt.Columns.Add("CurrencyID", typeof(int));
            dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "FileID", DataType = typeof(long) });
            dt.Columns.Add("SourceID", typeof(string));
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("CreatedTime", typeof(DateTime));
            dt.Columns.Add("EffectiveOn", typeof(DateTime));
           
            dt.BeginLoadData();
            foreach (var item in supplierPriceList)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = item.SupplierId;
                row[index++] = item.CurrencyId;
                row[index++] = item.FileId.HasValue ? (object)item.FileId.Value : DBNull.Value;
                row[index++] = item.SourceId;
                row[index++] = item.PriceListId;
                row[index++] = item.CreateTime;
                row[index++] = item.EffectiveOn;

                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, SupplierPriceList> GetSupplierPriceLists(bool useTempTables)
        {
            return GetItemsText("SELECT ID,  SupplierID, CurrencyID, FileID, CreatedTime, EffectiveOn,SourceID FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, useTempTables), SupplierPriceListMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public SupplierPriceList SupplierPriceListMapper(IDataReader reader)
        {
            return new SupplierPriceList
            {
                SupplierId = (int)reader["SupplierID"],
                CurrencyId = (int)reader["CurrencyID"],
                PriceListId = (int)reader["ID"],
                FileId = GetReaderValue <long?>(reader, "FileID"),
                CreateTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                EffectiveOn = GetReaderValue<DateTime>(reader, "EffectiveOn"),
                SourceId = reader["SourceID"] as string,
            };
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
