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
    public class MissingCDRDataManager :BaseCDRDataManager , IMissingCDRDataManager 
    {
        #region Constructors / Fields

        static string[] s_Columns = new string[]
        {
            "OriginalCDPN",
            "OriginalCGPN",
            "CDPN",
            "CGPN",
            "Time",
            "DurationInSec",
            "IsPartnerCDR"
        };
        
        #endregion

        #region Public Methods

        public IEnumerable<MissingCDR> GetMissingCDRs(bool isPartnerCDRs)
        {
            return GetItemsText(GetMissingCDRsQuery(isPartnerCDRs), MissingCDRMapper, null);
        }

        public int GetMissingCDRsCount()
        {
            object count = ExecuteScalarText(string.Format("SELECT COUNT(*) FROM {0}",this.TableName), null);
            return (int)count;
        }
        public void CreateMissingCDRTempTable()
        {
            StringBuilder query = new StringBuilder();
            query.Append
            (
                    @"Create Table #TEMPTABLE# ([ID] [int] IDENTITY(1,1) NOT NULL,
                    [OriginalCDPN] [varchar](100) NULL,
                    [OriginalCGPN] [varchar](100) NULL,
                    [CDPN] [varchar](100) NULL,
                    [CGPN] [varchar](100) NULL,
                    [Time] [datetime] NULL,
                    [DurationInSec] [decimal](18, 0) NULL,
                    [IsPartnerCDR] [bit] NULL) "
            );
            query.Replace("#TEMPTABLE#", this.TableName);
            ExecuteNonQueryText(query.ToString(), null);
        }

        #region Bulk Insert Methods

        public void ApplyMissingCDRsToDB(object preparedNumberProfiles)
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

        public void WriteRecordToStream(MissingCDR record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                record.OriginalCDPN,
                record.OriginalCGPN,
                record.CDPN,
                record.CGPN,
                record.Time,
                record.DurationInSec,
                record.IsPartnerCDR ? "1" : "0"
            );
        }
        
        #endregion

        #endregion

        #region Private Methods

        private string GetMissingCDRsQuery(bool isPartnerCDRs)
        {
            StringBuilder query = new StringBuilder();
            query.Append
            (
                @"SELECT [ID],
                    [OriginalCDPN],
                    [OriginalCGPN],
		            [CDPN],
		            [CGPN],
		            [Time],
		            [DurationInSec],
		            [IsPartnerCDR]
	            FROM #TEMPTABLE#
	            WHERE IsPartnerCDR = #ISPARTNERCDRS#"
            );
            query.Replace("#ISPARTNERCDRS#", (isPartnerCDRs ? "1" : "0"));
            query.Replace("#TEMPTABLE#", this.TableName);
            return query.ToString();
        }

        MissingCDR MissingCDRMapper(IDataReader reader)
        {
            return new MissingCDR()
            {
                OriginalCDPN = reader["OriginalCDPN"] as string,
                OriginalCGPN = reader["OriginalCGPN"] as string,
                CDPN = reader["CDPN"] as string,
                CGPN = reader["CGPN"] as string,
                Time = GetReaderValue<DateTime>(reader, "Time"),
                DurationInSec = GetReaderValue<decimal>(reader, "DurationInSec"),
                IsPartnerCDR = GetReaderValue<bool>(reader, "IsPartnerCDR")
            };
        }
        
        #endregion

        protected override string TableNamePrefix
        {
            get
            {
                return "MissingCDR";
            }
        }
    }
}
