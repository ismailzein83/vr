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

        public void DeletePartialMatchTable()
        {
            StringBuilder query = new StringBuilder();
            query.Append
            (
                @"DROP TABLE #TEMPTABLE#"
            );
            query.Replace("#TEMPTABLE#", this.TableName);
            ExecuteNonQueryText(query.ToString(), null);
        }

        public Vanrise.Entities.BigResult<PartialMatchCDR> GetFilteredPartialMatchCDRs(Vanrise.Entities.DataRetrievalInput<PartialMatchCDRQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                CreateTempTableIfNotExists(input, tempTableName);
            };
            return RetrieveData(input, createTempTableAction, PartialMatchCDRMapper);
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
                    [SystemDurationInSec] [decimal](20, 10) NULL,
                    [PartnerDurationInSec] [decimal](20, 10) NULL) 
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
            object count = ExecuteScalarText(string.Format("SELECT COUNT(*) FROM {0}", this.TableName), null);
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

        public decimal GetDurationOfPartialMatchCDRs(bool isPartner)
        {
            string durationColumnName = (isPartner) ? "PartnerDurationInSec" : "SystemDurationInSec";
            object duration = ExecuteScalarText(String.Format("SELECT SUM({0}) FROM {1}", durationColumnName, this.TableName), null);
            return (duration != DBNull.Value) ? (decimal)duration : 0;
        }

        public decimal GetTotalDurationDifferenceOfPartialMatchCDRs(string tableKey)
        {
            object durationDifference = ExecuteScalarText(String.Format("SELECT SUM(ABS(SystemDurationInSec - PartnerDurationInSec)) FROM {0}", this.TableName), null);
            return (durationDifference != DBNull.Value) ? (decimal)durationDifference : 0;
        }

        #endregion

        #region Private Methods


        private void CreateTempTableIfNotExists(Vanrise.Entities.DataRetrievalInput<PartialMatchCDRQuery> input, string tempTableName)
        {
            StringBuilder createTempTableQueryBuilder = new StringBuilder(@"
                                                                IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
                                                              BEGIN 
                                                              SELECT [ID]
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
                                                                INTO #TEMPTABLE#
	                                                            FROM #TABLENAME#                
                                                            END");
            createTempTableQueryBuilder.Replace("#TEMPTABLE#", tempTableName);
            createTempTableQueryBuilder.Replace("#TABLENAME#", this.TableName);
            ExecuteNonQueryText(createTempTableQueryBuilder.ToString(), null);
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
