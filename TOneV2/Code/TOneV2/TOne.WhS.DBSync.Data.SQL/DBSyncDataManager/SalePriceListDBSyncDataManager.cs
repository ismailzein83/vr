using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SalePriceListDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SalePriceList);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SalePriceListDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySalePriceListsToTemp(List<SalePriceList> salePriceLists, long startingId)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("OwnerType", typeof(int));
            dt.Columns.Add("OwnerID", typeof(int));
            dt.Columns.Add("CurrencyID", typeof(int));
            dt.Columns.Add("SourceID", typeof(string));
            dt.Columns.Add("EffectiveOn", typeof(DateTime));
            dt.Columns.Add("CreatedTime", typeof(DateTime));
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("IsSent", typeof(bool));
            dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "PriceListType", DataType = typeof(byte) });
            dt.Columns.Add("Description", typeof(string));
            dt.BeginLoadData();
            foreach (var item in salePriceLists)
            {
                DataRow row = dt.NewRow();
                row["OwnerType"] = (int)item.OwnerType;
                row["OwnerID"] = item.OwnerId;
                row["CurrencyID"] = item.CurrencyId;
                row["SourceID"] = item.SourceId;
                row["EffectiveOn"] = item.EffectiveOn;
                row["CreatedTime"] = item.CreatedTime;
                row["IsSent"] = item.IsSent;
                row["ID"] = startingId++;
                row["Description"] = item.Description;
                if (item.PriceListType.HasValue)
                    row["PriceListType"] = item.PriceListType.Value;
                else
                    row["PriceListType"] = DBNull.Value;

                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, SalePriceList> GetSalePriceLists(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT ID,  OwnerType, OwnerID, CurrencyID, EffectiveOn, IsSent, SourceID FROM {0} {1}"
                , MigrationUtils.GetTableName(_Schema, _TableName, useTempTables), "where sourceid is not null"), SalePriceListMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public SalePriceList SalePriceListMapper(IDataReader reader)
        {
            return new SalePriceList
            {
                OwnerId = (int)reader["OwnerID"],
                CurrencyId = (int)reader["CurrencyID"],
                PriceListId = (int)reader["ID"],
                OwnerType = (SalePriceListOwnerType)GetReaderValue<int>(reader, "OwnerType"),
                SourceId = reader["SourceID"] as string,
                EffectiveOn = GetReaderValue<DateTime>(reader, "EffectiveOn"),
                IsSent = GetReaderValue<bool>(reader, "IsSent")
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
