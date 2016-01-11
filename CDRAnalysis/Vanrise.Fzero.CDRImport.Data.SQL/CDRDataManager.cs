﻿using System.Collections.Generic;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Data.SQL
{
    public class CDRDataManager : BaseSQLDataManager, ICDRDataManager
    {
        static string[] s_cdrColumns = new string[] {
            "MSISDN"
          ,"IMSI"
          ,"ConnectDateTime"
          ,"Destination"
          ,"DurationInSeconds"
          ,"DisconnectDateTime"
          ,"Call_Class"
          ,"IsOnNet"
          ,"Call_Type"
          ,"Sub_Type"
          ,"IMEI"
          ,"BTS_ID"
          ,"Cell_ID"
          ,"SwitchID"
          ,"Up_Volume"
          ,"Down_Volume"
          ,"Cell_Latitude"
          ,"Cell_Longitude"
          ,"In_Trunk"
          ,"Out_Trunk"
          ,"Service_Type"
          ,"Service_VAS_Name"
          ,"InTrunkID"
          ,"OutTrunkID"
          ,"ReleaseCode"
          ,"MSISDNAreaCode"
          ,"DestinationAreaCode"
        };
        public CDRDataManager()
            : base("CDRDBConnectionString")
        {
        }

        public void SaveCDRsToDB(List<CDR> cdrs)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (CDR cdr in cdrs)
            {
                WriteRecordToStream(cdr, dbApplyStream);
            }

            ApplyCDRsToDB(FinishDBApplyStream(dbApplyStream));
        }

        public void ApplyCDRsToDB(object preparedCDRs)
        {
            InsertBulkToTable(preparedCDRs as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[FraudAnalysis].[NormalCDR]",
                ColumnNames = s_cdrColumns,
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

        public void WriteRecordToStream(CDR record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}^{20}^{21}^{22}^{23}^{24}^{25}^{26}",
                                     record.MSISDN
                                   , record.IMSI
                                   , record.ConnectDateTime
                                   , record.Destination
                                   , record.DurationInSeconds
                                   , record.DisconnectDateTime
                                   , record.CallClass
                                   , null
                                   , (int) record.CallType
                                   , record.SubType
                                   , record.IMEI
                                   , record.BTSId
                                   , record.CellId
                                   , record.SwitchId
                                   , record.UpVolume
                                   , record.DownVolume
                                   , record.CellLatitude
                                   , record.CellLongitude
                                   , record.InTrunkSymbol
                                   , record.OutTrunkSymbol
                                   , record.ServiceType
                                   , record.ServiceVASName
                                   , record.InTrunkId
                                   , record.OutTrunkId
                                   , record.ReleaseCode
                                   , record.MSISDNAreaCode
                                   , record.DestinationAreaCode
                                    );

        }
    }
}
