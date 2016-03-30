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
    public class DisputeCDRDataManager : BaseSQLDataManager, IDisputeCDRDataManager
    {
        #region Constructors / Fields

        public DisputeCDRDataManager()
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

        public void ApplyDisputeCDRsToDB(object preparedNumberProfiles)
        {
            InsertBulkToTable(preparedNumberProfiles as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[DisputeCDR]",
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

        public void WriteRecordToStream(DisputeCDR record, object dbApplyStream)
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

        public IEnumerable<DisputeCDR> GetDisputeCDRs()
        {
            return GetItemsSP("dbo.sp_DisputeCDR_GetAll", DisputeCDRMapper);
        }

        public int GetDisputeCDRsCount()
        {
            object count = ExecuteScalarSP("dbo.sp_DisputeCDR_GetCount");
            return (int)count;
        }

        #endregion

        #region Mappers

        DisputeCDR DisputeCDRMapper(IDataReader reader)
        {
            return new DisputeCDR()
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
