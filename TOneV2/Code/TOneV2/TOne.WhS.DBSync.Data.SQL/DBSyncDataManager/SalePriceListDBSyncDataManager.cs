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
        readonly string[] columns = { "OwnerType", "OwnerID", "CurrencyID", "SourceID" };
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SalePriceList);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SalePriceListDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySalePriceListsToTemp(List<SalePriceList> salePriceLists)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in salePriceLists)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}", (int)c.OwnerType, c.OwnerId, c.CurrencyId, c.SourceId));
                }
                wr.Close();
            }

            Object preparedSalePriceLists = new BulkInsertInfo
            {
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedSalePriceLists as BaseBulkInsertInfo);
        }

        public Dictionary<string, SalePriceList> GetSalePriceLists(bool useTempTables)
        {
            return GetItemsText("SELECT ID,  OwnerType, OwnerID, CurrencyID, SourceID FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, useTempTables), SalePriceListMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
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
