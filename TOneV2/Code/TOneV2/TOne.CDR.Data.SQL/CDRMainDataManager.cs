using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Entities;
using TOne.Data.SQL;
using Vanrise.Data.SQL;

namespace TOne.CDR.Data.SQL
{
    public class CDRMainDataManager : BaseTOneDataManager, ICDRMainDataManager
    {
       
        public void ReserverIdRange(bool isMain, bool isNegative, int numberOfIds, out long id)
        {
            id = (long)ExecuteScalarSP("CDR.sp_CDRIDManager_ReserveIDRange", isMain, isNegative, numberOfIds);
        }
        public object FinishDBApplyStream(object dbApplyStream)
        {
            List<StreamForBulkInsert> streamForBulkInsert = dbApplyStream as List<StreamForBulkInsert>;
            streamForBulkInsert[0].Close();
            streamForBulkInsert[1].Close();
            streamForBulkInsert[2].Close();
            List<StreamBulkInsertInfo> streamBulkInsertInfo = new List<StreamBulkInsertInfo>();
            streamBulkInsertInfo.Add(new StreamBulkInsertInfo
            {
                TableName = "[dbo].[Billing_CDR_Main]",
                Stream = streamForBulkInsert[0],
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            });
            streamBulkInsertInfo.Add(new StreamBulkInsertInfo
            {
                TableName = "[dbo].[Billing_CDR_Cost]",
                Stream = streamForBulkInsert[1],
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            });
            streamBulkInsertInfo.Add(new StreamBulkInsertInfo
            {
                TableName = "[dbo].[Billing_CDR_Sale]",
                Stream = streamForBulkInsert[2],
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            });
            return streamBulkInsertInfo;
        }

        public object InitialiazeStreamForDBApply()
        {
            List<StreamForBulkInsert> dbApplyStream = new List<StreamForBulkInsert>();
            for (var i = 0; i < 3;i++ )
                dbApplyStream.Add(base.InitializeStreamForBulkInsert());
            return dbApplyStream;
        }

        public void WriteRecordToStream(Entities.BillingCDRMain record, object dbApplyStream)
        {

            List<StreamForBulkInsert> streamForBulkInsert =dbApplyStream as List<StreamForBulkInsert>;
            streamForBulkInsert[0].WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}^{20}^{21}^{22}^{23}^{24}^{25}",
                                 record.ID,
                       record.Attempt.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        record.Alert.HasValue ? record.Alert.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "",
                        record.Connect.HasValue ? record.Connect.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "",
                        record.Disconnect.HasValue ? record.Disconnect.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "",
                        record.DurationInSeconds,
                        record.CustomerID,
                        record.OurZoneID,
                        record.OriginatingZoneID,
                        record.SupplierID,
                        record.SupplierZoneID,
                        record.CDPN,
                        record.CGPN,
                        record.ReleaseCode,
                        record.ReleaseSource,
                        record.SwitchID,
                        record.SwitchCdrID,
                        record.Tag,
                        record.Extra_Fields,
                         record.Port_IN,
                        record.Port_OUT,
                         record.OurCode,
                          record.SupplierCode,
                          record.CDPNOut,
                        record.SubscriberID,
                        record.SIP);

            if (record.cost != null)
                streamForBulkInsert[1].WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}^{20}",
                                     record.ID,
                                record.cost.ZoneID.ToString(),
                                Math.Round(record.cost.Net, 5),
                                record.cost.CurrencySymbol != null ? record.cost.CurrencySymbol.ToString() : "",
                                record.cost.RateValue,
                                record.cost.RateID.ToString(),
                                record.cost.Discount.HasValue ? record.cost.Discount.Value.ToString() : null,
                                Convert.ChangeType(record.cost.RateType, record.cost.RateType.GetTypeCode()),
                                record.cost.ToDConsiderationID.ToString(),
                                record.cost.FirstPeriod.HasValue ? record.cost.FirstPeriod.Value.ToString() : null,
                                record.cost.RepeatFirstperiod.HasValue ? record.cost.RepeatFirstperiod.Value.ToString() : null,
                                record.cost.FractionUnit.HasValue ? record.cost.RepeatFirstperiod.Value.ToString() : null,
                                record.cost.TariffID.ToString(),
                                record.cost.CommissionValue,
                                record.cost.CommissionID.ToString(),
                                record.cost.ExtraChargeValue,
                                record.cost.ExtraChargeID.ToString(),
                                record.cost.Updated.ToShortDateString(),
                                record.cost.DurationInSeconds,
                                record.cost.Code,
                                record.cost.Attempt.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            if (record.sale != null)
                streamForBulkInsert[2].WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}^{20}",
                                   record.ID,
                           record.sale.ZoneID.ToString(),
                           Math.Round(record.sale.Net, 5),
                           record.sale.CurrencySymbol != null ? record.sale.CurrencySymbol : "",
                           record.sale.RateValue,
                           record.sale.RateID.ToString(),
                           record.sale.Discount.HasValue ? record.sale.Discount.Value.ToString() : null,
                           Convert.ChangeType(record.sale.RateType, record.sale.RateType.GetTypeCode()),
                           record.sale.ToDConsiderationID.ToString(),
                           record.sale.FirstPeriod.HasValue ? record.sale.FirstPeriod.Value.ToString() : null,
                           record.sale.RepeatFirstperiod.HasValue ? record.sale.RepeatFirstperiod.Value.ToString() : null,
                           record.sale.FractionUnit.HasValue ? record.sale.FractionUnit.Value.ToString() : null,
                           record.sale.TariffID.ToString(),
                           record.sale.CommissionValue,
                           record.sale.CommissionID.ToString(),
                           record.sale.ExtraChargeValue,
                           record.sale.ExtraChargeID.ToString(),
                           record.sale.Updated.ToShortDateString(),
                           record.sale.DurationInSeconds,
                           record.sale.Code,
                           record.sale.Attempt.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }




        public void ApplyMainCDRsToDB(object preparedMainCDRs)
        {
            List<StreamBulkInsertInfo> listPreparedCDRs = preparedMainCDRs as List<StreamBulkInsertInfo>;

            Parallel.ForEach(listPreparedCDRs, item =>
            {
                InsertBulkToTable(item);
            });
        }

        public void SaveMainCDRsToDB(List<Entities.BillingCDRMain> cdrsMain)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (BillingCDRMain cdrMain in cdrsMain)
                WriteRecordToStream(cdrMain, dbApplyStream);
            Object preparedInvalidCDRs = FinishDBApplyStream(dbApplyStream);
            ApplyMainCDRsToDB(preparedInvalidCDRs);
        }

        public void DeleteCDRMain(DateTime from, DateTime to, List<string> customerIds, List<string> supplierIds)
        {
            ExecuteNonQueryText(GetQuery( customerIds, supplierIds,"Billing_CDR_Main","IX_Billing_CDR_Main_Attempt"),
                (cmd) =>
                {
                    cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@From", from));
                    cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@To", to));
                });
        }
        public void DeleteCDRSale(DateTime from, DateTime to, List<string> customerIds, List<string> supplierIds)
        {
            ExecuteNonQueryText(GetQuery( customerIds, supplierIds,"Billing_CDR_Sale","IX_Billing_CDR_Sale_Attempt"),
               (cmd) =>
               {
                   cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@From", from));
                   cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@To", to));
               });
        }

        public void DeleteCDRCost(DateTime from, DateTime to, List<string> customerIds, List<string> supplierIds)
        {
            ExecuteNonQueryText(GetQuery( customerIds, supplierIds,"Billing_CDR_Cost","IX_Billing_CDR_Cost_Attempt"),
               (cmd) =>
               {
                   cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@From", from));
                   cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@To", to));
               });
        }
        private string GetQuery(List<string> customerIds, List<string> supplierIds,string tableName,string index)
        {

            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(String.Format(query_DeleteTemplate, tableName, "Attempt", Guid.NewGuid().ToString().Replace("-", ""), index));
            if (customerIds != null && customerIds.Count > 0)
                queryBuilder.AppendFormat("AND CustomerID IN ('{0}')", String.Join("', '", customerIds));
            if (supplierIds != null && supplierIds.Count > 0)
                queryBuilder.AppendFormat("AND SupplierID IN ('{0}')", String.Join("', '", supplierIds));
            return queryBuilder.ToString();
        }  
        #region Queries 
             const string query_DeleteTemplate = @"DELETE {0} FROM {0} WITH(NOLOCK, INDEX({3})) WHERE  {1} >= @From AND {1} < @To --{2} ";
        #endregion

    }
}
