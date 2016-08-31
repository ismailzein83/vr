using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.Whs.Routing.Data.TOneV1SQL
{
    public class SupplierZoneDetailsDataManager : RoutingDataManager, ISupplierZoneDetailsDataManager
    {
        readonly string[] columns = { "SupplierId", "SupplierZoneId", "EffectiveRateValue" };
        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[SupplierZoneDetail]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }
        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        public void WriteRecordToStream(SupplierZoneDetail record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}", record.SupplierId, record.SupplierZoneId, record.EffectiveRateValue);
        }
        public void SaveSupplierZoneDetailsForDB(List<SupplierZoneDetail> supplierZoneDetails)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (SupplierZoneDetail supplierZoneDetail in supplierZoneDetails)
                WriteRecordToStream(supplierZoneDetail, dbApplyStream);
            Object preparedSupplierZoneDetails = FinishDBApplyStream(dbApplyStream);
            ApplySupplierZoneDetailsForDB(preparedSupplierZoneDetails);
        }
        public void ApplySupplierZoneDetailsForDB(object preparedSupplierZoneDetails)
        {
            InsertBulkToTable(preparedSupplierZoneDetails as BaseBulkInsertInfo);
        }
        public IEnumerable<SupplierZoneDetail> GetSupplierZoneDetails()
        {
            return GetItemsText(query_GetSupplierZoneDetails, SupplierZoneDetailMapper, null);
        }
        public IEnumerable<SupplierZoneDetail> GetFilteredSupplierZoneDetailsBySupplierZone(IEnumerable<long> supplierZoneIds)
        {
            DataTable dtZoneIds = BuildZoneIdsTable(new HashSet<long>(supplierZoneIds));
            return GetItemsText(query_GetFilteredSupplierZoneDetailsBySupplierZones, SupplierZoneDetailMapper, (cmd) =>
            {

                var dtPrm = new SqlParameter("@ZoneList", SqlDbType.Structured);
                dtPrm.TypeName = "LongIDType";
                dtPrm.Value = dtZoneIds;
                cmd.Parameters.Add(dtPrm);
            });
        }

        #region Private Motheds
        SupplierZoneDetail SupplierZoneDetailMapper(IDataReader reader)
        {
            return new SupplierZoneDetail()
            {
                SupplierId = (int)reader["SupplierId"],
                SupplierZoneId = (long)reader["SupplierZoneId"],
                EffectiveRateValue = GetReaderValue<decimal>(reader, "EffectiveRateValue")
            };
        }

        DataTable BuildZoneIdsTable(HashSet<long> zoneIds)
        {
            DataTable dtZoneInfo = new DataTable();
            dtZoneInfo.Columns.Add("ZoneID", typeof(Int64));
            dtZoneInfo.BeginLoadData();
            foreach (var z in zoneIds)
            {
                DataRow dr = dtZoneInfo.NewRow();
                dr["ZoneID"] = z;
                dtZoneInfo.Rows.Add(dr);
            }
            dtZoneInfo.EndLoadData();
            return dtZoneInfo;
        }
        #endregion
        #region Queries

        const string query_GetSupplierZoneDetails = @"                                                       
                                           SELECT  zd.[SupplierId]
                                                  ,zd.[SupplierZoneId]
                                                  ,zd.[EffectiveRateValue]
                                           FROM [dbo].[SupplierZoneDetail] zd with(nolock)";

        const string query_GetFilteredSupplierZoneDetailsBySupplierZones = @"                                                       
                                           SELECT  zd.[SupplierId]
                                                  ,zd.[SupplierZoneId]
                                                  ,zd.[EffectiveRateValue]
                                           FROM [dbo].[SupplierZoneDetail] zd with(nolock)
                                           JOIN @ZoneList z ON z.ID = zd.SupplierZoneId
                                            ";

        #endregion

    }
}
