﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class SupplierRatePreviewDataManager : BaseSQLDataManager, ISupplierRatePreviewDataManager
    {
        readonly string[] _columns = { "PriceListId", "ZoneName", "ChangeType", "RecentRate", "NewRate", "BED", "EED" };

        public SupplierRatePreviewDataManager()
            : base(GetConnectionStringName("TOneWhS_SPL_DBConnStringKey", "TOneWhS_SPL_DBConnString"))
        {

        }

        public void Insert(int priceListId, IEnumerable<RatePreview> ratePreviewList)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (RatePreview rate in ratePreviewList)
            {
                WriteRecordToStream(priceListId, rate, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
            ApplyForDB(prepareToApplyInfo);
        }
        public Vanrise.Entities.BigResult<Entities.RatePreview> GetRatePreviewFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SPLPreviewQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {

                ExecuteNonQuerySP("[TOneWhS_SPL].[sp_SupplierRate_Preview_CreateTempByFiltered]", tempTableName, input.Query.PriceListId);
            };

            return RetrieveData(input, createTempTableAction, RatePreviewMapper);
        }

        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        private void WriteRecordToStream(int priceListId, RatePreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                priceListId,
                record.ZoneName,
                (int)record.ChangeType,
                record.RecentRate,
                record.NewRate,
                record.BED,
                record.EED);
        }

        private object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "TOneWhS_SPL.SupplierRate_Preview",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }
        private RatePreview RatePreviewMapper(IDataReader reader)
        {
            RatePreview ratePreview = new RatePreview
            {
                ZoneName = reader["Name"] as string,
                ChangeType = (RateChangeType)GetReaderValue<int>(reader, "ChangeType"),
                RecentRate = GetReaderValue<decimal>(reader, "RecentRate"),
                NewRate = GetReaderValue<decimal>(reader, "NewRate"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime>(reader, "EED")
            };
            return ratePreview;
        }
        private void ApplyForDB(object preparedObject)
        {
            InsertBulkToTable(preparedObject as BaseBulkInsertInfo);
        }
    }
}
