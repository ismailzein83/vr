using System.Collections.Generic;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;
using System;

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
                stream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}",
                                 cdr.CGPN
                                  , cdr.CDPN
                                  , cdr.SwitchID
                                  , cdr.InTrunkSymbol
                                  , cdr.OutTrunkSymbol
                                  , cdr.ConnectDateTime
                                  , cdr.DurationInSeconds
                                  , cdr.DisconnectDateTime
                                  , cdr.InTrunkId
                                  , cdr.OutTrunkId
                                
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
                    FieldSeparator = '^'
                });
        }


        public void LoadStagingCDR(DateTime from, DateTime to, int? batchSize, Action<StagingCDR> onBatchReady)
        {
            ExecuteReaderSP("FraudAnalysis.sp_StagingCDR_GetByConnectDateTime", (reader) =>
            {


                int count = 0;
                int currentIndex = 0;

                while (reader.Read())
                {
                    StagingCDR stagingCDR = new StagingCDR();
                    stagingCDR.CDPN = reader["CDPN"] as string;
                    stagingCDR.CGPN = reader["CGPN"] as string;
                    stagingCDR.ConnectDateTime = GetReaderValue<DateTime?>(reader, "ConnectDateTime");
                    stagingCDR.InTrunkSymbol = reader["InTrunkSymbol"] as string;
                    stagingCDR.OutTrunkSymbol = reader["OutTrunkSymbol"] as string;
                    stagingCDR.DurationInSeconds = GetReaderValue<Decimal?>(reader, "DurationInSeconds");
                    stagingCDR.DisconnectDateTime = GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
                    stagingCDR.SwitchID = GetReaderValue<int>(reader, "SwitchID");

                    currentIndex++;
                    if (currentIndex == 100000)
                    {
                        count += currentIndex;
                        currentIndex = 0;
                    }

                    onBatchReady(stagingCDR);
                }



            }, from, to
               );
        }


    }
}
