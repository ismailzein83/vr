using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace CDRComparison.Data.SQL
{
    public class MissingCDRDataManager : BaseSQLDataManager, IMissingCDRDataManager
    {
        public MissingCDRDataManager()
            : base("CDRComparisonDBConnStringKey")
        {

        }

        static string[] s_Columns = new string[] {
            "CDPN"
            ,"CGPN"
            ,"Time"
            ,"DurationInSec"
            ,"IsPartnerCDR"
        };


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

        public void WriteRecordToStream(MissingCDR record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}",
                                    record.CDPN,
                                    record.CGPN,
                                    record.DurationInSec,
                                   record.IsPartnerCDR
                                    );
        }
    }
}
