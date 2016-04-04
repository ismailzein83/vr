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
    public class PartialMatchCDRDataManager : BaseCDRDataManager, IPartialMatchCDRDataManager
    {
        #region Constructors / Fields

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
        public void CreatePartialMatchCDRTempTable()
        {
          
            StringBuilder query = new StringBuilder();
            query.Append
            (
                    @"Create Table #TEMPTABLE# ([ID] [int] IDENTITY(1,1) NOT NULL,
                    [OriginalSystemCDPN] [varchar](100) NULL,
                    [OriginalPartnerCDPN] [varchar](100) NULL,
                    [OriginalSystemCGPN] [varchar](100) NULL,
                    [OriginalPartnerCGPN] [varchar](100) NULL,
                    [SystemCDPN] [varchar](100) NULL,
                    [PartnerCDPN] [varchar](100) NULL,
                    [SystemCGPN] [varchar](100) NULL,
                    [PartnerCGPN] [varchar](100) NULL,
                    [SystemTime] [datetime] NULL,
                    [PartnerTime] [datetime] NULL,
                    [SystemDurationInSec] [decimal](18, 0) NULL,
                    [PartnerDurationInSec] [decimal](18, 0) NULL) 
                    "
            );
            query.Replace("#TEMPTABLE#", this.TableName);
            ExecuteNonQueryText(query.ToString(), null);
        }
        protected override string TableNamePrefix
        {
            get
            {
                return "PartialMatchCDR";
            }
        }
        public int GetPartialMatchCDRsCount()
        {
            object count = ExecuteScalarText(string.Format("SELECT COUNT(*) FROM {0}",this.TableName), null);
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
                TableName = this.TableName,
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
                FROM #TEMPTABLE#"
            );
            query.Replace("#TEMPTABLE#", this.TableName);
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
