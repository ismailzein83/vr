﻿using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace CDRComparison.Data.SQL
{
    public class CDRDataManager : BaseSQLDataManager, ICDRDataManager
    {
        #region Constructors / Fields

        public CDRDataManager()
            : base(GetConnectionStringName("CDRComparisonDBConnStringKey", "CDRComparisonDBConnString"))
        {

        }

        static string[] s_Columns = new string[]
        {
            "OriginalCDPN",
            "OriginalCGPN",
            "CDPN",
            "CGPN",
            "IsPartnerCDR",
            "AttemptTime",
            "Duration"
        };
        
        #endregion

        #region Public Methods

        public void LoadCDRs(Action<CDR> onBatchReady)
        {
            ExecuteReaderText(GetLoadCDRsQuery(), (reader) =>
            {
                while (reader.Read())
                {
                    onBatchReady(new CDR()
                    {
                        OriginalCDPN = GetReaderValue<string>(reader, "OriginalCDPN"),
                        OriginalCGPN = GetReaderValue<string>(reader, "OriginalCGPN"),
                        CDPN = GetReaderValue<string>(reader, "CDPN"),
                        CGPN = GetReaderValue<string>(reader, "CGPN"),
                        DurationInSec = (GetReaderValue<Decimal>(reader, "Duration")),
                        Time = (GetReaderValue<DateTime>(reader, "AttemptTime")),
                        IsPartnerCDR = (GetReaderValue<Boolean>(reader, "IsPartnerCDR"))
                    });
                }
            }, null);
        }

        public int GetAllCDRsCount()
        {
            object count = ExecuteScalarText("SELECT COUNT(*) FROM dbo.CDR", null);
            return (int)count;
        }

        #region Bulk Insert Methods

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(CDR record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                record.OriginalCDPN,
                record.OriginalCGPN,
                record.CDPN,
                record.CGPN,
                record.IsPartnerCDR ? "1" : "0",
                record.Time,
                record.DurationInSec

            );
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[CDR]",
                Stream = streamForBulkInsert,
                ColumnNames = s_Columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^'
            };
        }

        public void ApplyCDRsToDB(object preparedCDRs)
        {
            InsertBulkToTable(preparedCDRs as BaseBulkInsertInfo);
        }

        #endregion
        
        #endregion

        #region Private Methods

        private string GetLoadCDRsQuery()
        {
            StringBuilder query = new StringBuilder();
            query.Append
            (
                @"SELECT [ID],
                    [OriginalCDPN],
                    [OriginalCGPN],
		            [CDPN],
		            [CGPN],
		            [IsPartnerCDR],
		            [AttemptTime],
		            [Duration]
	            FROM  [CDRComparison_Dev].[dbo].[CDR]
	            ORDER BY [CDPN]"
            );
            return query.ToString();
        }

        #endregion
    }
}
