using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class FinancialAccountDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.FinancialAccount);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public FinancialAccountDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

            _UseTempTables = useTempTables;
        }
        static string[] s_columns = new string[] { "ID", "CarrierProfileId", "CarrierAccountId", "FinancialAccountDefinitionId", "FinancialAccountSettings", "BED", "EED" };


        public void ApplyFinancialAccountsToTemp(List<WHSFinancialAccount> carrierAccounts, int startingId)
        {
            //DataTable dt = new DataTable();
            //dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            //dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "CarrierProfileID", DataType = typeof(int) });
            //dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "CarrierAccountId", DataType = typeof(int) });
            //dt.Columns.Add("FinancialAccountDefinitionId", typeof(Guid));
            //dt.Columns.Add("FinancialAccountSettings", typeof(string));
            //dt.Columns.Add("BED", typeof(DateTime));
            //dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "EED", DataType = typeof(DateTime) });

            //dt.BeginLoadData();
            //foreach (var item in carrierAccounts)
            //{
            //    DataRow row = dt.NewRow();
            //    int index = 0;

            //    if (item.CarrierProfileId == null)
            //        row[index++] = DBNull.Value;
            //    else
            //        row[index++] = item.CarrierProfileId;

            //    if (item.CarrierAccountId == null)
            //        row[index++] = DBNull.Value;
            //    else
            //        row[index++] = item.CarrierAccountId;

            //    row[index++] = item.FinancialAccountDefinitionId;
            //    row[index++] = Vanrise.Common.Serializer.Serialize(item.Settings);
            //    row[index++] = item.BED;
            //    if (item.EED == null)
            //        row[index++] = DBNull.Value;
            //    else
            //        row[index++] = item.EED;
            //    dt.Rows.Add(row);
            //}
            //dt.EndLoadData();
            //WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);


            var stream = base.InitializeStreamForBulkInsert();
            foreach (var item in carrierAccounts)
            {
                stream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}", startingId++,
                                                                item.CarrierProfileId.HasValue ? item.CarrierProfileId.Value : default(int?),
                                                                item.CarrierAccountId.HasValue ? item.CarrierAccountId.Value : default(int?),
                                                                item.FinancialAccountDefinitionId,
                                                                item.Settings != null ? Vanrise.Common.Serializer.Serialize(item.Settings) : null,
                                                                GetDateTimeForBCP(item.BED), 
                                                                item.EED.HasValue ? GetDateTimeForBCP(item.EED.Value) : "");
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
