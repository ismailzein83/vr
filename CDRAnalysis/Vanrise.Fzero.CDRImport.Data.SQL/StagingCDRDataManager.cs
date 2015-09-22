using System.Collections.Generic;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Data.SQL
{
    public class StagingCDRDataManager : BaseSQLDataManager, IStagingCDRDataManager
    {
        public StagingCDRDataManager()
            : base("CDRDBConnectionString")
        {
        }

        public void SaveStagingCDRsToDB(List<StagingCDR> cdrs)
        {

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (StagingCDR cdr in cdrs)
            {
                stream.WriteRecord("{0},{1},{2},{3},{4},{5},{6},{7}",
                                 cdr.CGPN
                                  , cdr.CDPN
                                  , cdr.SwitchID
                                  , cdr.InTrunkSymbol
                                  , cdr.OutTrunkSymbol
                                  , cdr.ConnectDateTime
                                  , cdr.DurationInSeconds
                                  , cdr.DisconnectDateTime
                                
                );
            }

            stream.Close();

            InsertBulkToTable(  
                new StreamBulkInsertInfo
                {
                    TableName = "[FraudAnalysis].[StagingCDR]",
                    Stream = stream,
                    TabLock = true,
                    KeepIdentity = false,
                    FieldSeparator = ','
                });
        }
    }
}
