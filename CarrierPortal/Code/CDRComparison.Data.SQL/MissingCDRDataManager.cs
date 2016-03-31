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
    public class MissingCDRDataManager : BaseSQLDataManager, IMissingCDRDataManager
    {
        #region Constructors / Fields

        public MissingCDRDataManager()
            : base(GetConnectionStringName("CDRComparisonDBConnStringKey", "CDRComparisonDBConnString"))
        {

        }

        static string[] s_Columns = new string[] {
            "CDPN"
            ,"CGPN"
            ,"Time"
            ,"DurationInSec"
            ,"IsPartnerCDR"
        };
        
        #endregion

        #region Public Methods

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
                TableName = "[dbo].[MissingCDR]",
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
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}",
                                    record.CDPN,
                                    record.CGPN,
                                    record.Time,
                                    record.DurationInSec,
                                   record.IsPartnerCDR ? "1" : "0"
                                    );
        }

        public IEnumerable<MissingCDR> GetMissingCDRs(bool isPartnerCDRs)
        {
            return GetItemsText(GetMissingCDRsQuery(isPartnerCDRs), MissingCDRMapper,null);
        }
        private string GetMissingCDRsQuery(bool isPartnerCDRs)
        {
            StringBuilder query = new StringBuilder();
                    query.Append(@"SELECT ID,
		                                  CDPN,
		                                  CGPN,
		                                  [Time],
		                                  DurationInSec,
		                                  IsPartnerCDR
	                                      FROM dbo.MissingCDR
	                                      WHERE IsPartnerCDR = #ISPARTNERCDRS#");
            query.Replace("#ISPARTNERCDRS#",(isPartnerCDRs? "1" : "0"));
            return query.ToString();
        }
        public int GetMissingCDRsCount()
        {
            object count = ExecuteScalarText("SELECT COUNT(*) FROM dbo.MissingCDR", null);
            return (int)count;
        }

        #endregion

        #region Mappers

        MissingCDR MissingCDRMapper(IDataReader reader)
        {
            return new MissingCDR()
            {
                CDPN = GetReaderValue<string>(reader, "CDPN"),
                CGPN = GetReaderValue<string>(reader, "CGPN"),
                Time = GetReaderValue<DateTime>(reader, "Time"),
                DurationInSec = GetReaderValue<decimal>(reader, "DurationInSec"),
                IsPartnerCDR = GetReaderValue<bool>(reader, "IsPartnerCDR")
            };
        }
        
        #endregion
    }
}
