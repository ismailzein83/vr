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
    public class PartialMatchCDRDataManager : BaseSQLDataManager, IPartialMatchCDRDataManager
    {
        #region Constructors / Fields

        public PartialMatchCDRDataManager()
            : base(GetConnectionStringName("CDRComparisonDBConnStringKey", "CDRComparisonDBConnString"))
        {

        }

        static string[] s_Columns = new string[] {
            "SystemCDPN"
            ,"PartnerCDPN"
            ,"SystemCGPN"
            ,"PartnerCGPN"
            ,"SystemTime"
            ,"PartnerTime"
            ,"SystemDurationInSec"
            ,"PartnerDurationInSec"
        };

        #endregion

        #region Public Methods

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
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
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

        public IEnumerable<PartialMatchCDR> GetPartialMatchCDRs()
        {
            return GetItemsSP("dbo.sp_PartialMatchCDR_GetAll", PartialMatchCDRMapper);
        }

        public int GetPartialMatchCDRsCount()
        {
            object count = ExecuteScalarSP("dbo.sp_PartialMatchCDR_GetCount");
            return (int)count;
        }

        #endregion

        #region Mappers

        PartialMatchCDR PartialMatchCDRMapper(IDataReader reader)
        {
            return new PartialMatchCDR()
            {
                SystemCDPN = GetReaderValue<string>(reader, "SystemCDPN"),
                PartnerCDPN = GetReaderValue<string>(reader, "PartnerCDPN"),
                SystemCGPN = GetReaderValue<string>(reader, "SystemCGPN"),
                PartnerCGPN = GetReaderValue<string>(reader, "PartnerCGPN"),
                SystemTime = GetReaderValue<DateTime>(reader, "SystemTime"),
                PartnerTime = GetReaderValue<DateTime>(reader, "PartnerTime"),
                SystemDurationInSec = GetReaderValue<decimal>(reader, "SystemDurationInSec"),
                PartnerDurationInSec = GetReaderValue<decimal>(reader, "PartnerDurationInSec")
            };
        }
        
        #endregion
    }
}
