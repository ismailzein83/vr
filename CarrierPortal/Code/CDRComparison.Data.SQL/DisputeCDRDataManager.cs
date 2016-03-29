using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace CDRComparison.Data.SQL
{
    public class DisputeCDRDataManager : BaseSQLDataManager, IDisputeCDRDataManager
    {
        public DisputeCDRDataManager()
            : base("CDRComparisonDBConnStringKey")
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
                ColumnNames=s_Columns,
                TabLock = false,
                KeepIdentity = false,
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
    }
}
