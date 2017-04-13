﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SalePriceListChangeDataManager : BaseSQLDataManager, ISalePriceListChangeDataManager
    {
        private readonly string[] _salePricelistCodeChangeColumns =
        {
            "Code","RecentZoneName","ZoneName","Change","BatchID","BED","EED","CountryID"
        };
        private readonly string[] _salePricelistRateChangeColumns =
        {
            "PricelistId","Rate","RecentRate","CountryID","ZoneName","Change","ProcessInstanceID","BED","EED"
        };
        private readonly string[] _salePricelistCustomerChangeColumns =
        {
           "BatchID","PricelistID","CountryID","CustomerID"
        };
        public SalePriceListChangeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #region public Methods
        public List<SalePricelistCodeChange> GetFilteredSalePricelistCodeChanges(int pricelistId, List<int> countryIds)
        {
            string strcountryIds = null;
            if (countryIds != null && countryIds.Count > 0)
                strcountryIds = string.Join(",", countryIds);
            return GetItemsSP("TOneWhS_BE.sp_SalePricelistCodeChange_GetFiltered", SalePricelistCodeChangeMapper,
                pricelistId, strcountryIds);
        }
        public List<SalePricelistRateChange> GetFilteredSalePricelistRateChanges(int pricelistId, List<int> countryIds)
        {
            string strcountryIds = null;
            if (countryIds != null && countryIds.Count > 0)
                strcountryIds = string.Join(",", countryIds);
            return GetItemsSP("TOneWhS_BE.sp_SalePricelistRateChange_GetFiltered", SalePricelistRateChangeMapper,
                pricelistId, strcountryIds);
        }
        public List<SalePricelistCodeChange> GetNotSentCodechanges(IEnumerable<int> customerIds)
        {
            string strcustomerIds = null;
            if (customerIds != null && customerIds.Any())
                strcustomerIds = string.Join(",", customerIds);
            return GetItemsSP("TOneWhS_BE.sp_SalePriceListChange_GetNotSentCodeChanges", SalePricelistCodeChangeMapper, strcustomerIds);
        }

        public List<SalePricelistRateChange> GetNotSentRatechanges(IEnumerable<int> customerIds)
        {
            string strcustomerIds = null;
            if (customerIds != null && customerIds.Any())
                strcustomerIds = string.Join(",", customerIds);
            return GetItemsSP("TOneWhS_BE.sp_SalePriceListChange_GetNotSentRateChanges", SalePricelistRateChangeMapper, strcustomerIds);
        }
        public void SaveCustomerChangesToDb(IEnumerable<SalePriceListCustomerChange> salePriceLists)
        {
            if (salePriceLists == null || !salePriceLists.Any()) return;
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (SalePriceListCustomerChange salePriceList in salePriceLists)
                WriteRecordToStream(salePriceList, dbApplyStream);
            Object preparedSalePriceLists = FinishDBApplyStream(dbApplyStream, "TOneWhS_BE.SalePricelistCustomerChange_New", _salePricelistCustomerChangeColumns);
            ApplySalePriceListsToDB(preparedSalePriceLists);
        }
        public void SaveCustomerCodeChangesToDb(IEnumerable<SalePricelistCodeChange> codeChanges)
        {
            if (codeChanges == null || !codeChanges.Any()) return;
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (SalePricelistCodeChange codeChange in codeChanges)
                WriteRecordToStream(codeChange, dbApplyStream);
            Object preparedSalePriceLists = FinishDBApplyStream(dbApplyStream, "TOneWhS_BE.SalePricelistCodeChange_New", _salePricelistCodeChangeColumns);
            ApplySalePriceListsToDB(preparedSalePriceLists);
        }
        public void SaveCustomerRateChangesToDb(IEnumerable<SalePricelistRateChange> rateChanges, long processInstanceId)
        {
            if (rateChanges == null || !rateChanges.Any()) return;
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (SalePricelistRateChange rateChange in rateChanges)
                WriteRecordToStream(rateChange, dbApplyStream, processInstanceId);
            Object preparedSalePriceLists = FinishDBApplyStream(dbApplyStream, "TOneWhS_BE.SalePricelistRateChange_New", _salePricelistRateChangeColumns);
            ApplySalePriceListsToDB(preparedSalePriceLists);
        }

        #endregion
        #region Bulk Insert
        private void WriteRecordToStream(SalePriceListCustomerChange record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            if (streamForBulkInsert != null)
                streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}",
                    record.BatchId,
                    record.PriceListId,
                    record.CountryId,
                    record.CustomerId
                    );
        }
        private void WriteRecordToStream(SalePricelistCodeChange record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            if (streamForBulkInsert != null)
                streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
                    record.Code,
                    record.RecentZoneName,
                    record.ZoneName,
                    (int)record.ChangeType,
                    record.BatchId,
                    GetDateTimeForBCP(record.BED),
                    GetDateTimeForBCP(record.EED),
                    record.CountryId);
        }
        private void WriteRecordToStream(SalePricelistRateChange record, object dbApplyStream, long processInstanceId)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            if (streamForBulkInsert != null)
                streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}",
                    record.PricelistId,
                    decimal.Round(record.Rate, 8),
                    record.RecentRate.HasValue ? decimal.Round(record.RecentRate.Value, 8) : record.RecentRate,
                    record.CountryId,
                    record.ZoneName,
                    (int)record.ChangeType,
                    processInstanceId,
                    GetDateTimeForBCP(record.BED),
                    GetDateTimeForBCP(record.EED));
        }
        private object FinishDBApplyStream(object dbApplyStream, string tableName, string[] columnNames)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = tableName,
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columnNames
            };

        }
        private void ApplySalePriceListsToDB(object preparedSalePriceLists)
        {
            InsertBulkToTable(preparedSalePriceLists as BaseBulkInsertInfo);
        }
        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        #endregion
        #region Mappers

        SalePricelistCodeChange SalePricelistCodeChangeMapper(IDataReader reader)
        {
            SalePricelistCodeChange salePricelistCodeChange = new SalePricelistCodeChange
            {
                PricelistId = GetReaderValue<int>(reader, "PricelistID"),
                Code = GetReaderValue<string>(reader, "Code"),
                CountryId = GetReaderValue<int>(reader, "CountryID"),
                RecentZoneName = GetReaderValue<string>(reader, "RecentZoneName"),
                ZoneName = GetReaderValue<string>(reader, "ZoneName"),
                ChangeType = (CodeChange)GetReaderValue<byte>(reader, "Change"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
            return salePricelistCodeChange;
        }
        SalePricelistRateChange SalePricelistRateChangeMapper(IDataReader reader)
        {
            SalePricelistRateChange salePricelistCodeChange = new SalePricelistRateChange
            {
                PricelistId = GetReaderValue<int>(reader, "PricelistID"),
                CountryId = GetReaderValue<int>(reader, "CountryID"),
                ZoneName = GetReaderValue<string>(reader, "ZoneName"),
                Rate = GetReaderValue<decimal>(reader, "Rate"),
                RecentRate = GetReaderValue<decimal>(reader, "RecentRate"),
                ChangeType = (RateChangeType)GetReaderValue<byte>(reader, "Change"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
            return salePricelistCodeChange;
        }

        #endregion
    }
}
