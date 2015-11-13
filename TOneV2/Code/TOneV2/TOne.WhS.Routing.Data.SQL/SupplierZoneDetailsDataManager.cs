using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class SupplierZoneDetailsDataManager : RoutingDataManager, ISupplierZoneDetailsDataManager
    {
        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[SupplierZoneDetail]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
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
        public IEnumerable<SupplierZoneDetail> LoadSupplierZoneDetails()
        {
            return GetItemsText(query_GetSupplierZoneDetails, SupplierZoneDetailMapper, null);
        }
        SupplierZoneDetail SupplierZoneDetailMapper(IDataReader reader)
        {
            return new SupplierZoneDetail()
            {
                SupplierId = (int)reader["SupplierId"],
                SupplierZoneId = (int)reader["SupplierZoneId"],
                EffectiveRateValue = GetReaderValue<decimal>(reader, "EffectiveRateValue")
            };
        }

        #region Queries

        const string query_GetSupplierZoneDetails = @"                                                       
                                           SELECT  zd.[SupplierId]
                                                  ,zd.[SupplierZoneId]
                                                  ,zd.[EffectiveRateValue]
                                           FROM [dbo].[SupplierZoneDetail] zd";

        #endregion
    }
}
