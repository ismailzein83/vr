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
            _columnMapper.Add("CallClass", "CallClassID");
            _columnMapper.Add("CallTypeDescription", "CallTypeID");
            _columnMapper.Add("SubType", "SubscriberTypeID");
            _columnMapper.Add("CellId", "Cell");
            _columnMapper.Add("UpVolume", "UpVolume");
            _columnMapper.Add("DownVolume", "DownVolume");
            _columnMapper.Add("ServiceType", "ServiceTypeID");
            _columnMapper.Add("ServiceVASName", "ServiceVASName");

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
                    normalCDR = NormalCDRMapper(reader);

                    currentIndex++;
                    if (currentIndex == 100000)
                    {
                        count += currentIndex;
                        currentIndex = 0;
                        Console.WriteLine("Loaded {0} CDRs", count);
                    }

                    onBatchReady(normalCDR);
                }


            }, from, to
               );
        }

        public BigResult<CDR> GetNormalCDRs(Vanrise.Entities.DataRetrievalInput<NormalCDRQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_NormalCDR_CreateTempByMSISDN", tempTableName, input.Query.MSISDN, input.Query.FromDate, input.Query.ToDate);
            };

            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");

            return RetrieveData(input, createTempTableAction, NormalCDRMapper, _columnMapper);
        }

        #region Private Methods

        private CDR NormalCDRMapper(IDataReader reader)
        {
            var normalCDR = new CDR();
            normalCDR.CallType = GetReaderValue<CallType>(reader, "CallTypeID");
            normalCDR.BTS = reader["BTS"] as string;
            normalCDR.ConnectDateTime = GetReaderValue<DateTime>(reader, "ConnectDateTime");
            normalCDR.IMSI = reader["IMSI"] as string;
            normalCDR.DurationInSeconds = GetReaderValue<decimal>(reader, "DurationInSeconds");
            normalCDR.DisconnectDateTime = GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
            normalCDR.CallClassId = GetReaderValue<int?>(reader, "CallClassID");
            normalCDR.IsOnNet = GetReaderValue<bool>(reader, "IsOnNet");
            normalCDR.SubscriberType = (SubscriberType)GetReaderValue<int?>(reader, "SubscriberTypeID");
            normalCDR.IMEI = reader["IMEI"] as string;
            normalCDR.Cell = reader["Cell"] as string;
            normalCDR.UpVolume = GetReaderValue<Decimal?>(reader, "UpVolume");
            normalCDR.DownVolume = GetReaderValue<Decimal?>(reader, "DownVolume");
            normalCDR.CellLatitude = GetReaderValue<Decimal?>(reader, "CellLatitude");
            normalCDR.CellLongitude = GetReaderValue<Decimal?>(reader, "CellLongitude");
            normalCDR.InTrunkId = GetReaderValue<int?>(reader, "InTrunkID");
            normalCDR.OutTrunkId = GetReaderValue<int?>(reader, "OutTrunkID");
            normalCDR.ServiceTypeId = GetReaderValue<int?>(reader, "ServiceTypeID");
            normalCDR.ServiceVASName = reader["ServiceVASName"] as string;
            normalCDR.ReleaseCode = reader["ReleaseCode"] as string;
            normalCDR.MSISDNAreaCode = reader["MSISDNAreaCode"] as string;
            normalCDR.DestinationAreaCode = reader["DestinationAreaCode"] as string;
            normalCDR.Destination = reader["Destination"] as string;
            normalCDR.MSISDN = reader["MSISDN"] as string;
            return normalCDR;
        }

       

        #endregion
    }
}
