﻿using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace CDRComparison.Data.SQL
{
    public class PartialMatchCDRDataManager : BaseSQLDataManager, IPartialMatchCDRDataManager
    {
        #region Constructors / Fields

        public PartialMatchCDRDataManager()
            : base(GetConnectionStringName("CDRComparisonDBConnStringKey", "CDRComparisonDBConnString"))
        {

        }

        static string[] s_Columns = new string[]
        {
            "OriginalSystemCDPN",
            "OriginalPartnerCDPN",
            "OriginalSystemCGPN",
            "OriginalPartnerCGPN",
            "SystemCDPN",
            "PartnerCDPN",
            "SystemCGPN",
            "PartnerCGPN",
            "SystemTime",
            "PartnerTime",
            "SystemDurationInSec",
            "PartnerDurationInSec"
        };

        #endregion

        #region Public Methods

        public IEnumerable<PartialMatchCDR> GetPartialMatchCDRs()
        {
            return GetItemsText(GetPartialMatchCDRsQuery(), PartialMatchCDRMapper, null);
        }

        public int GetPartialMatchCDRsCount()
        {
            object count = ExecuteScalarText("SELECT COUNT(*) FROM dbo.PartialMatchCDR", null);
            return (int)count;
        }

        #region Bulk Insert Methods

        public void ApplyPartialMatchCDRsToDB(object preparedNumberProfiles)
        {
            InsertBulkToTable(preparedNumberProfiles as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[PartialMatchCDR]",
                Stream = streamForBulkInsert,
                ColumnNames = s_Columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^'
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(PartialMatchCDR record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}",
                record.OriginalSystemCDPN,
                record.OriginalPartnerCDPN,
                record.OriginalSystemCGPN,
                record.OriginalPartnerCGPN,
                record.SystemCDPN,
                record.PartnerCDPN,
                record.SystemCGPN,
                record.PartnerCGPN,
                record.SystemTime,
                record.PartnerTime,
                record.SystemDurationInSec,
                record.PartnerDurationInSec
            );
        }
        
        #endregion

        #endregion

        #region Private Methods

        private string GetPartialMatchCDRsQuery()
        {
            StringBuilder query = new StringBuilder();
            query.Append
            (
                @"SELECT TOP 1000 [ID]
                    [OriginalSystemCDPN],
                    [OriginalPartnerCDPN],
                    [OriginalSystemCGPN],
                    [OriginalPartnerCGPN],
                    [SystemCDPN],
                    [PartnerCDPN],
                    [SystemCGPN],
                    [PartnerCGPN],
                    [SystemTime],
                    [PartnerTime],
                    [SystemDurationInSec],
                    [PartnerDurationInSec]
                FROM [CDRComparison_Dev].[dbo].[PartialMatchCDR]"
            );
            return query.ToString();
        }

        PartialMatchCDR PartialMatchCDRMapper(IDataReader reader)
        {
            return new PartialMatchCDR()
            {
                OriginalSystemCDPN = reader["OriginalSystemCDPN"] as string,
                OriginalPartnerCDPN = reader["OriginalPartnerCDPN"] as string,
                OriginalSystemCGPN = reader["OriginalSystemCGPN"] as string,
                OriginalPartnerCGPN = reader["OriginalPartnerCGPN"] as string,
                SystemCDPN = reader["SystemCDPN"] as string,
                PartnerCDPN = reader["PartnerCDPN"] as string,
                SystemCGPN = reader["SystemCGPN"] as string,
                PartnerCGPN = reader["PartnerCGPN"] as string,
                SystemTime = GetReaderValue<DateTime>(reader, "SystemTime"),
                PartnerTime = GetReaderValue<DateTime>(reader, "PartnerTime"),
                SystemDurationInSec = GetReaderValue<decimal>(reader, "SystemDurationInSec"),
                PartnerDurationInSec = GetReaderValue<decimal>(reader, "PartnerDurationInSec")
            };
        }
        
        #endregion
    }
}
