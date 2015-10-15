using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class NormalCDRDataManager : BaseSQLDataManager, INormalCDRDataManager
    {
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        static NormalCDRDataManager()
        {
            _columnMapper.Add("CallClass", "Call_Class");
            _columnMapper.Add("CallTypeDescription", "Call_Type");
            _columnMapper.Add("SubType", "Sub_Type");
            _columnMapper.Add("CellId", "Cell_Id");
            _columnMapper.Add("UpVolume", "Up_Volume");
            _columnMapper.Add("DownVolume", "Down_Volume");
            _columnMapper.Add("ServiceType", "Service_Type");
            _columnMapper.Add("ServiceVASName", "Service_VAS_Name");

        }

        public NormalCDRDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public void LoadCDR(DateTime from, DateTime to, int? batchSize, Action<CDR> onBatchReady)
        {
            ExecuteReaderSP("FraudAnalysis.sp_NormalCDR_GetByConnectDateTime", (reader) =>
            {


                int count = 0;
                int currentIndex = 0;

                while (reader.Read())
                {
                    CDR normalCDR = new CDR();
                    normalCDR.CallType = GetReaderValue<CallType>(reader, "Call_Type");
                    normalCDR.BTSId = GetReaderValue<int?>(reader, "BTS_Id");
                    normalCDR.ConnectDateTime = GetReaderValue<DateTime?>(reader, "ConnectDateTime");
                    normalCDR.Id = (int)reader["Id"];
                    normalCDR.IMSI = reader["IMSI"] as string;
                    normalCDR.DurationInSeconds = GetReaderValue<Decimal?>(reader, "DurationInSeconds");
                    normalCDR.DisconnectDateTime = GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
                    normalCDR.CallClass = reader["Call_Class"] as string;
                    normalCDR.IsOnNet = GetReaderValue<short?>(reader, "IsOnNet");
                    normalCDR.SubType = reader["Sub_Type"] as string;
                    normalCDR.IMEI = reader["IMEI"] as string;
                    normalCDR.CellId = reader["Cell_Id"] as string;
                    normalCDR.UpVolume = GetReaderValue<Decimal?>(reader, "Up_Volume");
                    normalCDR.DownVolume = GetReaderValue<Decimal?>(reader, "Down_Volume");
                    normalCDR.CellLatitude = GetReaderValue<Decimal?>(reader, "Cell_Latitude");
                    normalCDR.CellLongitude = GetReaderValue<Decimal?>(reader, "Cell_Longitude");
                    normalCDR.InTrunkSymbol = reader["In_Trunk"] as string;
                    normalCDR.OutTrunkSymbol = reader["Out_Trunk"] as string;
                    normalCDR.InTrunkId = GetReaderValue<int?>(reader, "InTrunkID");
                    normalCDR.OutTrunkId = GetReaderValue<int?>(reader, "OutTrunkID");
                    normalCDR.ServiceType = GetReaderValue<int?>(reader, "Service_Type");
                    normalCDR.ServiceVASName = reader["Service_VAS_Name"] as string;
                    normalCDR.ReleaseCode = reader["ReleaseCode"] as string;
                    normalCDR.AreaCode = reader["AreaCode"] as string;
                    normalCDR.Destination = reader["Destination"] as string;
                    normalCDR.MSISDN = reader["MSISDN"] as string;


                    currentIndex++;
                    if (currentIndex == 100000)
                    {
                        count += currentIndex;
                        currentIndex = 0;

                    }

                    onBatchReady(normalCDR);
                }



            }, from, to
               );
        }

        public BigResult<CDR> GetNormalCDRs(Vanrise.Entities.DataRetrievalInput<NormalCDRResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_NormalCDR_CreateTempByMSISDN", tempTableName, input.Query.MSISDN, input.Query.FromDate, input.Query.ToDate);
            };
            return RetrieveData(input, createTempTableAction, NormalCDRMapper, _columnMapper);
        }

        #region Private Methods

        private CDR NormalCDRMapper(IDataReader reader)
        {
            var normalCDR = new CDR();
            normalCDR.CallType = (CallType)reader["Call_Type"];
            normalCDR.ConnectDateTime = GetReaderValue<DateTime?>(reader, "ConnectDateTime");
            normalCDR.IMSI = reader["IMSI"] as string;
            normalCDR.DurationInSeconds = GetReaderValue<Decimal?>(reader, "DurationInSeconds");
            normalCDR.CallClass = reader["Call_Class"] as string;
            normalCDR.SubType = reader["Sub_Type"] as string;
            normalCDR.IMEI = reader["IMEI"] as string;
            normalCDR.CellId = reader["Cell_Id"] as string;
            normalCDR.UpVolume = GetReaderValue<Decimal?>(reader, "Up_Volume");
            normalCDR.DownVolume = GetReaderValue<Decimal?>(reader, "Down_Volume");
            normalCDR.ServiceType = GetReaderValue<int?>(reader, "Service_Type");
            normalCDR.ServiceVASName = reader["Service_VAS_Name"] as string;
            normalCDR.Destination = reader["Destination"] as string;
            normalCDR.ReleaseCode = reader["ReleaseCode"] as string;
            normalCDR.AreaCode = reader["AreaCode"] as string;
            return normalCDR;
        }

        #endregion
    }
}
