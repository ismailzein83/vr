﻿using System;
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

        public NormalCDRDataManager()
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
                    normalCDR.CallType =  GetReaderValue<CallType>(reader, "Call_Type");
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
                    normalCDR.InTrunk = reader["In_Trunk"] as string;
                    normalCDR.OutTrunk = reader["Out_Trunk"] as string;
                    normalCDR.ServiceType = GetReaderValue<int?>(reader, "Service_Type");
                    normalCDR.ServiceVASName = reader["Service_VAS_Name"] as string;
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
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("CallClass", "Call_Class");
            mapper.Add("CallTypeDescription", "Call_Type");
            mapper.Add("SubType", "Sub_Type");
            mapper.Add("CellId", "Cell_Id");
            mapper.Add("UpVolume", "Up_Volume");
            mapper.Add("DownVolume", "Down_Volume");
            mapper.Add("ServiceType", "Service_Type");
            mapper.Add("ServiceVASName", "Service_VAS_Name");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_NormalCDR_CreateTempForFilteredNormalCDRs", tempTableName, input.Query.FromDate, input.Query.ToDate, input.Query.MSISDN);
            };
            return RetrieveData(input, createTempTableAction, NormalCDRMapper, mapper);
        }


        #region Private Methods

        private CDR NormalCDRMapper(IDataReader reader)
        {
            var normalCDR = new CDR();
            normalCDR.CallType = (CallType)Enum.ToObject(typeof(CallType), GetReaderValue<int>(reader, "Call_Type"));
            normalCDR.ConnectDateTime = GetReaderValue<DateTime?>(reader, "ConnectDateTime");
            normalCDR.IMSI = reader["IMSI"] as string;
            normalCDR.DurationInSeconds = GetReaderValue<Decimal?>(reader, "DurationInSeconds");
            normalCDR.CallClass = reader["Call_Class"] as string;
            normalCDR.SubType = reader["Sub_Type"] as string;
            normalCDR.IMEI = reader["IMEI"] as string;
            normalCDR.CellId = reader["Cell_Id"] as string;
            normalCDR.UpVolume = GetReaderValue<Decimal?>(reader, "Up_Volume");
            normalCDR.DownVolume = GetReaderValue<Decimal?>(reader, "Down_Volume");
            normalCDR.ServiceType = GetReaderValue<int>(reader, "Service_Type");
            normalCDR.ServiceVASName = reader["Service_VAS_Name"] as string;
            normalCDR.Destination = reader["Destination"] as string;
            return normalCDR;
        }


        #endregion



    }
}
