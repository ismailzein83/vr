using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Integration.Entities;

namespace Retail.Runtime.Mappers
{
    public static class OgeroMappers
    {
        public static Vanrise.Integration.Entities.MappingOutput MapCDR_SQL_Alcatel_Ogero(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            LogVerbose("Started");
            var cdrs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("ICX_CDR");

            int batchSize = 50000;

            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));
            IDataReader reader = importedData.Reader;

            var dsFieldNames = new Dictionary<Retail.Ogero.Business.DSFieldName, string>();
            dsFieldNames.Add(Retail.Ogero.Business.DSFieldName.ANumber, "ANumber");
            dsFieldNames.Add(Retail.Ogero.Business.DSFieldName.BNumber, "BNumber");

            var trunkValues = new Dictionary<Retail.Ogero.Business.TrunkType, string>();
            trunkValues.Add(Retail.Ogero.Business.TrunkType.MTC, "LC");
            trunkValues.Add(Retail.Ogero.Business.TrunkType.Alfa, "FT");
            trunkValues.Add(Retail.Ogero.Business.TrunkType.Ogero, "Ogero");

            while (reader.Read())
            {
                string cgpn;
                string cdpn;
                string inTrunk;
                string outTrunk;

                bool includeCDR = Retail.Ogero.Business.InterconnectDataSourceManager.IncludeCDR(reader, dsFieldNames, false, trunkValues, out cgpn, out cdpn, out inTrunk, out outTrunk);
                if (includeCDR)
                {
                    dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                    switch (reader["DataSourceId"].ToString().ToLower())
                    {
                        case "34689433-20d9-4757-ae2f-eeceef3c0518": cdr.Switch = 12; break;
                        case "3772f062-ae92-493a-b602-9d3fe455a7dc": cdr.Switch = 5; break;
                        case "9d8f5ed6-5ad5-44b6-bac7-1806ebe8b2a5": cdr.Switch = 6; break;
                    }

                    cdr.IDOnSwitch = Utils.GetReaderValue<long>(reader, "ID").ToString();
                    cdr.DataSource = dataSourceId;

                    DateTime attemptDatetime = Utils.GetReaderValue<DateTime>(reader, "AttemptDateTime");
                    cdr.AlertDateTime = attemptDatetime;
                    cdr.AttemptDateTime = attemptDatetime;
                    cdr.ConnectDateTime = attemptDatetime;

                    decimal durationInSeconds = Utils.GetReaderValue<int>(reader, "DurationInSeconds");
                    cdr.DurationInSeconds = durationInSeconds;
                    cdr.DisconnectDateTime = attemptDatetime.AddSeconds((double)durationInSeconds);
                    cdr.FileName = reader["FileName"] as string;
                    cdr.CGPN = cgpn;
                    cdr.CDPN = cdpn;
                    cdr.InTrunk = inTrunk;
                    cdr.OutTrunk = outTrunk;

                    cdrs.Add(cdr);

                    if (cdrs.Count == batchSize)
                        break;
                }

                importedData.LastImportedId = reader["id"];
            }

            if (cdrs.Count > 0)
            {
                long startingId;
                var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("ICX_CDR");
                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, cdrs.Count, out startingId);

                foreach (var cdr in cdrs)
                    cdr.ID = startingId++;

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "ICX_CDR");
                mappedBatches.Add("Distribute Raw CDRs Stage", batch);
            }
            else
            {
                importedData.IsEmpty = true;
            }

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_SQL_Ericsson_Ogero(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            LogVerbose("Started");
            var cdrs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("ICX_CDR");

            int batchSize = 50000;

            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));
            IDataReader reader = importedData.Reader;

            var dsFieldNames = new Dictionary<Retail.Ogero.Business.DSFieldName, string>();
            dsFieldNames.Add(Retail.Ogero.Business.DSFieldName.ANumber, "ANumber");
            dsFieldNames.Add(Retail.Ogero.Business.DSFieldName.BNumber, "BNumber");
            dsFieldNames.Add(Retail.Ogero.Business.DSFieldName.InTrunk, "IncomingRoute");
            dsFieldNames.Add(Retail.Ogero.Business.DSFieldName.OutTrunk, "OutgoingRoute");

            var trunkValues = new Dictionary<Retail.Ogero.Business.TrunkType, string>();
            trunkValues.Add(Retail.Ogero.Business.TrunkType.MTC, "LC");
            trunkValues.Add(Retail.Ogero.Business.TrunkType.Alfa, "FT");
            trunkValues.Add(Retail.Ogero.Business.TrunkType.Ogero, "Ogero");

            while (reader.Read())
            {
                string cgpn;
                string cdpn;
                string inTrunk;
                string outTrunk;

                bool includeCDR = Retail.Ogero.Business.InterconnectDataSourceManager.IncludeCDR(reader, dsFieldNames, true, trunkValues, out cgpn, out cdpn, out inTrunk, out outTrunk);
                if (includeCDR)
                {
                    dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                    switch (reader["DataSourceId"].ToString().ToLower())
                    {
                        case "e37c51ba-120d-4ee8-8060-bb5008e14952": cdr.Switch = 8; break;
                        case "0ddfb5ca-18c5-48ca-8ba9-3d012090221f": cdr.Switch = 7; break;
                        case "fbbf4c1c-9609-4106-b25d-d5b8d4c53ea8": cdr.Switch = 11; break;
                    }

                    cdr.IDOnSwitch = Utils.GetReaderValue<long>(reader, "ID").ToString();
                    cdr.DataSource = dataSourceId;
                    cdr.AlertDateTime = Utils.GetReaderValue<DateTime>(reader, "ConnectDateTime");
                    cdr.AttemptDateTime = Utils.GetReaderValue<DateTime>(reader, "ConnectDateTime");
                    cdr.ConnectDateTime = Utils.GetReaderValue<DateTime>(reader, "ConnectDateTime");
                    cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime>(reader, "DisconnectDateTime");
                    cdr.DurationInSeconds = Utils.GetReaderValue<int>(reader, "ChargeableDuration");
                    cdr.CGPN = cgpn;
                    cdr.CDPN = cdpn;
                    cdr.FileName = reader["FileName"] as string;
                    cdr.InTrunk = inTrunk;
                    cdr.OutTrunk = outTrunk;

                    cdrs.Add(cdr);

                    if (cdrs.Count == batchSize)
                        break;
                }

                importedData.LastImportedId = reader["ID"];
            }

            if (cdrs.Count > 0)
            {
                long startingId;
                var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("ICX_CDR");
                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, cdrs.Count, out startingId);

                foreach (var cdr in cdrs)
                    cdr.ID = startingId++;

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "ICX_CDR");
                mappedBatches.Add("Distribute Raw CDRs Stage", batch);
            }
            else
            {
                importedData.IsEmpty = true;
            }

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_SQL_Nokia_Ogero(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            LogVerbose("Started");
            var cdrs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("ICX_CDR");

            int batchSize = 50000;

            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));
            IDataReader reader = importedData.Reader;

            var dsFieldNames = new Dictionary<Retail.Ogero.Business.DSFieldName, string>();
            dsFieldNames.Add(Retail.Ogero.Business.DSFieldName.ANumber, "CallingPartyNumber");
            dsFieldNames.Add(Retail.Ogero.Business.DSFieldName.BNumber, "CalledPartyNumber");
            dsFieldNames.Add(Retail.Ogero.Business.DSFieldName.InTrunk, "IncomingTrunkGroupNumberCIC");
            dsFieldNames.Add(Retail.Ogero.Business.DSFieldName.OutTrunk, "OutgoingTrunkGroupNumberCIC");

            var trunkValues = new Dictionary<Retail.Ogero.Business.TrunkType, string>();
            trunkValues.Add(Retail.Ogero.Business.TrunkType.MTC, "LC");
            trunkValues.Add(Retail.Ogero.Business.TrunkType.Alfa, "FT");
            trunkValues.Add(Retail.Ogero.Business.TrunkType.Ogero, "Ogero");

            while (reader.Read())
            {
                string cgpn;
                string cdpn;
                string inTrunk;
                string outTrunk;

                bool includeCDR = Retail.Ogero.Business.InterconnectDataSourceManager.IncludeCDR(reader, dsFieldNames, true, trunkValues, out cgpn, out cdpn, out inTrunk, out outTrunk);
                if (includeCDR)
                {
                    dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                    switch (reader["DataSourceId"].ToString().ToLower())
                    {
                        case "e12cfdfa-0bb0-432c-a6f8-9931ae1a33ed": cdr.Switch = 13; break;
                        case "0e703b35-cc89-4dc8-8f19-120ae429e09b": cdr.Switch = 10; break;
                    }

                    cdr.IDOnSwitch = Utils.GetReaderValue<long>(reader, "ID").ToString();
                    cdr.DataSource = dataSourceId;
                    cdr.AlertDateTime = Utils.GetReaderValue<DateTime?>(reader, "BeginDate");
                    cdr.AttemptDateTime = Utils.GetReaderValue<DateTime>(reader, "BeginDate");
                    cdr.ConnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "BeginDate");
                    cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "EndDate");
                    int durationInSeconds = Utils.GetReaderValue<int>(reader, "DurationInSeconds");
                    cdr.DurationInSeconds = (decimal)durationInSeconds;
                    cdr.FileName = reader["FileName"] as string;
                    cdr.CGPN = cgpn;
                    cdr.CDPN = cdpn;
                    cdr.OutTrunk = outTrunk;
                    cdr.InTrunk = inTrunk;

                    cdrs.Add(cdr);

                    if (cdrs.Count == batchSize)
                        break;
                }

                importedData.LastImportedId = reader["ID"];
            }

            if (cdrs.Count > 0)
            {
                long startingId;
                var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("ICX_CDR");
                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, cdrs.Count, out startingId);
                foreach (var cdr in cdrs)
                    cdr.ID = startingId++;

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "ICX_CDR");
                mappedBatches.Add("Distribute Raw CDRs Stage", batch);
            }
            else
            {
                importedData.IsEmpty = true;
            }

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");

            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_SQL_Huawei_Ogero(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            LogVerbose("Started");
            var cdrs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("ICX_CDR");

            int batchSize = 50000;
            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));
            IDataReader reader = importedData.Reader;

            var dsFieldNames = new Dictionary<Retail.Ogero.Business.DSFieldName, string>();
            dsFieldNames.Add(Retail.Ogero.Business.DSFieldName.ANumber, "CallingNumber");
            dsFieldNames.Add(Retail.Ogero.Business.DSFieldName.BNumber, "CalledNumber");
            dsFieldNames.Add(Retail.Ogero.Business.DSFieldName.InTrunk, "MSCIncomingRoute");
            dsFieldNames.Add(Retail.Ogero.Business.DSFieldName.OutTrunk, "MSCOutgoingRoute");

            var trunkValues = new Dictionary<Retail.Ogero.Business.TrunkType, string>();
            trunkValues.Add(Retail.Ogero.Business.TrunkType.MTC, "TO");
            trunkValues.Add(Retail.Ogero.Business.TrunkType.Alfa, "FT");
            trunkValues.Add(Retail.Ogero.Business.TrunkType.Ogero, "Ogero");

            while (reader.Read())
            {
                string cgpn;
                string cdpn;
                string inTrunk;
                string outTrunk;

                bool includeCDR = Retail.Ogero.Business.InterconnectDataSourceManager.IncludeCDR(reader, dsFieldNames, true, trunkValues, out cgpn, out cdpn, out inTrunk, out outTrunk);
                if (includeCDR)
                {
                    dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                    switch (reader["DataSourceId"].ToString().ToLower())
                    {
                        case "036a4165-284c-41c0-b285-0deb755c5677": cdr.Switch = 14; break;
                        case "09dd0e06-0f3a-46e6-9a12-f6462f298eec": cdr.Switch = 9; break;
                    }

                    cdr.IDOnSwitch = Utils.GetReaderValue<long>(reader, "ID").ToString();
                    cdr.DataSource = dataSourceId;
                    cdr.AlertDateTime = Utils.GetReaderValue<DateTime?>(reader, "AlertingTime");
                    cdr.AttemptDateTime = Utils.GetReaderValue<DateTime>(reader, "SetupTime");
                    cdr.ConnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "AnswerTime");
                    cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "ReleaseTime");
                    cdr.DurationInSeconds = Utils.GetReaderValue<int>(reader, "CallDuration");
                    cdr.CGPN = cgpn;
                    cdr.CDPN = cdpn;
                    cdr.InTrunk = inTrunk;
                    cdr.OutTrunk = outTrunk;
                    cdr.InIP = reader["OrigIOI"] as string;
                    cdr.OutIP = reader["TermIOI"] as string;
                    cdr.FileName = reader["FileName"] as string;

                    cdrs.Add(cdr);

                    if (cdrs.Count == batchSize)
                        break;
                }
                importedData.LastImportedId = reader["ID"];
            }

            if (cdrs.Count > 0)
            {
                long startingId;
                var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("ICX_CDR");
                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, cdrs.Count, out startingId);
                foreach (var cdr in cdrs)
                    cdr.ID = startingId++;

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "ICX_CDR");
                mappedBatches.Add("Distribute Raw CDRs Stage", batch);
            }
            else
            {
                importedData.IsEmpty = true;
            }

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");

            return result;
        }


        #region Private Methods

        private static void LogVerbose(string Message)
        {
            Console.WriteLine(Message);
        }

        private class Utils
        {
            public static T GetReaderValue<T>(IDataReader reader, string fieldName)
            {
                return reader[fieldName] != DBNull.Value ? (T)reader[fieldName] : default(T);
            }
        }

        #endregion
    }
}