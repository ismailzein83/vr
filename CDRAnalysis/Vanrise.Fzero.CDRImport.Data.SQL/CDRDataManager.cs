using System.Collections.Generic;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Data.SQL
{
    public class CDRDataManager : BaseSQLDataManager, ICDRDataManager
    {
        public CDRDataManager()
            : base("CDRDBConnectionString")
        {
        }

        public void SaveCDRsToDB(List<CDR> cdrs)
        {

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (CDR cdr in cdrs)
            {
                stream.WriteRecord("0,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}",
                                 cdr.MSISDN
                                  , cdr.IMSI
                                  , cdr.ConnectDateTime
                                  , cdr.Destination
                                  , cdr.DurationInSeconds
                                  , ""
                                  , cdr.CallClass
                                  , cdr.IsOnNet
                                  , (int) cdr.CallType
                                  , cdr.SubType
                                  , cdr.IMEI
                                  , ""
                                  , cdr.CellId
                                  , ""
                                  , ""
                                  , ""
                                  , cdr.CellLatitude
                                  , cdr.CellLongitude
                                  , cdr.InTrunk
                                  , cdr.OutTrunk
                                  , ""
                                  , ""
                );
            }

            stream.Close();

            InsertBulkToTable(  
                new StreamBulkInsertInfo
                {
                    TableName = "[FraudAnalysis].[NormalCDR]",
                    Stream = stream,
                    TabLock = true,
                    KeepIdentity = false,
                    FieldSeparator = ','
                });
        }

        public void ApplyCDRsToDB(object preparedCDRs)
        {
            InsertBulkToTable(preparedCDRs as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            throw new System.NotImplementedException();
        }

        public object InitialiazeStreamForDBApply()
        {
            throw new System.NotImplementedException();
        }

        public void WriteRecordToStream(CDR record, object dbApplyStream)
        {
            throw new System.NotImplementedException();
        }
    }
}
