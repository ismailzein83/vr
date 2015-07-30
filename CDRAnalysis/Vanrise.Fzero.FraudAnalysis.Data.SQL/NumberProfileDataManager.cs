using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;


namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class NumberProfileDataManager : BaseSQLDataManager, INumberProfileDataManager
    {
        public NumberProfileDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public void LoadCDR(DateTime from, DateTime to, int? batchSize, Action<CDR> onBatchReady)
        {
            ExecuteReaderSP("FraudAnalysis.sp_NormalCDR_Load", (reader) =>
                {

                    
                    int count = 0;
                    int currentIndex = 0;

                    while (reader.Read())
                    {
                        CDR normalCDR = new CDR();
                        normalCDR.CallType = Enums.CallType.OutgoingVoiceCall;// (Enums.CallType)Enum.ToObject(typeof(Enums.CallType), GetReaderValue<int>(reader, "Call_Type"));
                        normalCDR.BTSId = GetReaderValue<int?>(reader, "BTS_Id");
                        normalCDR.ConnectDateTime = GetReaderValue<DateTime?>(reader, "ConnectDateTime");
                        normalCDR.Id = (int)reader["Id"];
                        normalCDR.IMSI = reader["IMSI"] as string;
                        normalCDR.DurationInSeconds = GetReaderValue<Decimal?>(reader, "DurationInSeconds");
                        normalCDR.DisconnectDateTime = GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
                        normalCDR.CallClass = reader["Call_Class"] as string;
                        normalCDR.IsOnNet = GetReaderValue<Byte?>(reader, "IsOnNet");
                        normalCDR.SubType = reader["Sub_Type"] as string;
                        normalCDR.IMEI = reader["IMEI"] as string;
                        normalCDR.CellId = reader["Cell_Id"] as string;
                        normalCDR.SwitchRecordId = GetReaderValue<int?>(reader, "SwitchRecordId");
                        normalCDR.UpVolume = GetReaderValue<Decimal?>(reader, "Up_Volume");
                        normalCDR.DownVolume = GetReaderValue<Decimal?>(reader, "Down_Volume");
                        normalCDR.CellLatitude = GetReaderValue<Decimal?>(reader, "Cell_Latitude");
                        normalCDR.CellLongitude = GetReaderValue<Decimal?>(reader, "Cell_Longitude");
                        normalCDR.InTrunk = reader["In_Trunk"] as string;
                        normalCDR.OutTrunk = reader["Out_Trunk"] as string;
                        normalCDR.ServiceType = GetReaderValue<int>(reader, "Service_Type");
                        normalCDR.ServiceVASName = reader["Service_VAS_Name"] as string;
                        normalCDR.Destination = reader["Destination"] as string;
                        normalCDR.MSISDN = reader["MSISDN"] as string;


                       

                        currentIndex++;
                        if (currentIndex == 100000)
                        {
                            count += currentIndex;
                            currentIndex = 0;
                            Console.WriteLine("{0} rows read ", count);
                        }

                        onBatchReady(normalCDR);
                    }

                   

                },from,to
               );
        }

        public void ApplyNumberProfilesToDB(object preparedNumberProfiles)
        {
            InsertBulkToTable(preparedNumberProfiles as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[FraudAnalysis].[NumberProfile]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^'
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(NumberProfile record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("0^{0}^{1}^{2}^{3}^{4}^{5}",
                                    record.SubscriberNumber,
                                    record.FromDate,
                                    record.ToDate,
                                    record.PeriodId,
                                    record.StrategyId,
                                    Vanrise.Common.Serializer.Serialize(record.AggregateValues, true));
        }

    }
}
