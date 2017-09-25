using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CarrierAccountStatusHistoryDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.CarrierAccountStatusHistory);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public CarrierAccountStatusHistoryDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

            _UseTempTables = useTempTables;
        }

        public void ApplyCarrierAccountsStatusHistoryToTemp(List<CarrierAccountStatusHistory> carrierAccountsStatusHistory)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("CarrierAccountID", typeof(int));
            dt.Columns.Add("StatusID", typeof(int));
            dt.Columns.Add("StatusChangedDate", typeof(DateTime));

            dt.BeginLoadData();
            DateTime now = DateTime.Now;
            foreach (var carrierAccountStatusHistory in carrierAccountsStatusHistory)
            {
                DataRow row = dt.NewRow();
                row["CarrierAccountID"] = carrierAccountStatusHistory.CarrierAccountId;
                row["StatusID"] = (int)carrierAccountStatusHistory.Status;
                row["StatusChangedDate"] = now;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<int, CarrierAccountStatusHistory> GetCarrierAccountsStatusHistory(bool useTempTables)
        {
            return GetItemsText("SELECT [ID] ,[CarrierAccountId], StatusID, [StatusChangedDate] FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, useTempTables), CarrierAccountStatusHistoryMapper, cmd => { }).ToDictionary(x => x.CarrierAccountStatusHistoryId, x => x);
        }

        private CarrierAccountStatusHistory CarrierAccountStatusHistoryMapper(IDataReader reader)
        {
            return new CarrierAccountStatusHistory
            {
                CarrierAccountStatusHistoryId = (int)reader["ID"],
                CarrierAccountId = (int)reader["CarrierAccountId"],
                Status = (ActivationStatus)GetReaderValue<int>(reader, "StatusID"),
                StatusChangedDate = GetReaderValue<DateTime>(reader, "StatusChangedDate")
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