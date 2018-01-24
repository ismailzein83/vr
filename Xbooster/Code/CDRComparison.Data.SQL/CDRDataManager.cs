using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace CDRComparison.Data.SQL
{
    public class CDRDataManager : BaseCDRDataManager, ICDRDataManager
    {
        #region Constructors / Fields

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
        public void DeleteCDRTable()
        {
            StringBuilder query = new StringBuilder();
            query.Append
            (
                @"DROP TABLE #TEMPTABLE#"
            );
            query.Replace("#TEMPTABLE#", this.TableName);
            ExecuteNonQueryText(query.ToString(), null);
        }
        public void LoadCDRs(Action<CDR> onBatchReady)
        {
            ExecuteReaderText(GetLoadCDRsQuery(), (reader) =>
            {
                while (reader.Read())
                {
                    onBatchReady(new CDR()
                    {
                        OriginalCDPN = reader["OriginalCDPN"] as string,
                        OriginalCGPN = reader["OriginalCGPN"] as string,
                        CDPN = reader["CDPN"] as string,
                        CGPN = reader["CGPN"] as string,
                        DurationInSec = (GetReaderValue<Decimal>(reader, "Duration")),
                        Time = (GetReaderValue<DateTime>(reader, "AttemptTime")),
                        IsPartnerCDR = (GetReaderValue<Boolean>(reader, "IsPartnerCDR"))
                    });
                }
            }, null);
        }

        public void CreateCDRTempTable()
        {
            StringBuilder query = new StringBuilder();
            query.Append
            (
                @"Create Table #TEMPTABLE# ([ID] [int] IDENTITY(1,1) NOT NULL,
	                                       [OriginalCDPN] [varchar](100) NULL,
	                                       [OriginalCGPN] [varchar](100) NULL,
	                                       [CDPN] [varchar](100) NULL,
	                                       [CGPN] [varchar](100) NULL,
	                                       [IsPartnerCDR] [bit] NULL,
	                                       [AttemptTime] [datetime] NULL,
	                                       [Duration] [decimal](20, 10) NULL);
                    CREATE CLUSTERED INDEX [IX_#TEMPTABLE#_CDPN] ON [#TEMPTABLE#] 
                    (
	                    [CDPN] ASC
                    )"
            );
            query.Replace("#TEMPTABLE#", this.TableName);
            ExecuteNonQueryText(query.ToString(), null);
        }
        public int GetCDRsCount(bool? isPartner = null)
        {
            var queryBuilder = new StringBuilder(String.Format("SELECT COUNT(*) FROM {0}", this.TableName));
            if (isPartner.HasValue)
            {
                string isPartnerCDRValue = (isPartner.Value) ? "1" : "0";
                queryBuilder.Append(String.Format(" WHERE IsPartnerCDR = {0}", isPartnerCDRValue));
            }
            object count = ExecuteScalarText(queryBuilder.ToString(), null);
            return (int)count;
        }
        public Vanrise.Entities.BigResult<CDR> GetFilteredCDRs(Vanrise.Entities.DataRetrievalInput<CDRQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                CreateTempTableIfNotExists(input, tempTableName);
            };
            return RetrieveData(input, createTempTableAction, CDRMapper);
        }
        protected override string TableNamePrefix
        {
            get
            {
                return "CDR";
            }
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
                GetDateTimeForBCP(record.Time),
                record.DurationInSec
            );
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

        public void ApplyCDRsToDB(object preparedCDRs)
        {
            InsertBulkToTable(preparedCDRs as BaseBulkInsertInfo);
        }

        #endregion

        public decimal GetDurationOfCDRs(bool? isPartner)
        {
            var queryBuilder = new StringBuilder(String.Format("SELECT SUM(Duration) FROM {0}", this.TableName));
            if (isPartner.HasValue)
            {
                string isPartnerCDRValue = (isPartner.Value) ? "1" : "0";
                queryBuilder.Append(String.Format(" WHERE IsPartnerCDR = {0}", isPartnerCDRValue));
            }
            object duration = ExecuteScalarText(queryBuilder.ToString(), null);
            return (duration != DBNull.Value) ? (decimal)duration : 0;
        }

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
	            FROM  #TEMPTABLE#
	            ORDER BY [CDPN]"
            );
            query.Replace("#TEMPTABLE#", this.TableName);
            return query.ToString();
        }
        private void CreateTempTableIfNotExists(Vanrise.Entities.DataRetrievalInput<CDRQuery> input, string tempTableName)
        {
            StringBuilder createTempTableQueryBuilder = new StringBuilder(@"
                                                                IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
                                                              BEGIN 
                                                              SELECT [ID],
                                                                    [OriginalCDPN],
                                                                    [OriginalCGPN],
		                                                            [CDPN],
		                                                            [CGPN],
		                                                            [IsPartnerCDR],
		                                                            [AttemptTime],
		                                                            [Duration]
                                                                INTO #TEMPTABLE#
	                                                            FROM #TABLENAME#   
                                                                WHERE IsPartnerCDR = #ISPARTNERCDRS# #ADDITIONALQUERY#             
                                                            END");
            string cdpnQuery = "";
            if (input.Query.CDPN != null)
            {
                cdpnQuery = string.Format("and [CDPN]= '{0}'", input.Query.CDPN);
            }
            createTempTableQueryBuilder.Replace("#TEMPTABLE#", tempTableName);
            createTempTableQueryBuilder.Replace("#ISPARTNERCDRS#", (input.Query.IsPartnerCDRs ? "1" : "0"));
            createTempTableQueryBuilder.Replace("#ADDITIONALQUERY#", cdpnQuery);
            createTempTableQueryBuilder.Replace("#TABLENAME#", this.TableName);
            ExecuteNonQueryText(createTempTableQueryBuilder.ToString(), null);
        }
        CDR CDRMapper(IDataReader reader)
        {
            return new CDR()
            {
                OriginalCDPN = reader["OriginalCDPN"] as string,
                OriginalCGPN = reader["OriginalCGPN"] as string,
                CDPN = reader["CDPN"] as string,
                CGPN = reader["CGPN"] as string,
                Time = GetReaderValue<DateTime>(reader, "AttemptTime"),
                DurationInSec = GetReaderValue<decimal>(reader, "Duration"),
                IsPartnerCDR = GetReaderValue<bool>(reader, "IsPartnerCDR")
            };
        }
        #endregion
    }
}
