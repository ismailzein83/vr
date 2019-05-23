using System;
using System.Collections.Generic;
using TestCallAnalysis.Entities;
using Vanrise.GenericData.Business;

namespace TestCallAnalysis.Business
{
    public class MappedCDRManager
    {
        public static Guid dataRecordStorage = new Guid("58FCA073-8F5C-4A56-A4AF-025EB3B8BB60");

        public dynamic MappedCDRToRuntime(TCAnalMappedCDR mappedCDR)
        {
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("TCAnal_MappedCDR");
            dynamic runtimeCDR = Activator.CreateInstance(cdrRuntimeType) as dynamic;
            runtimeCDR.ID = mappedCDR.MappedCDRId;
            runtimeCDR.AttemptDateTime = mappedCDR.AttemptDateTime;
            runtimeCDR.DurationInSeconds = mappedCDR.DurationInSeconds;
            runtimeCDR.CalledNumber = mappedCDR.CalledNumber;
            runtimeCDR.CalledOperatorID = mappedCDR.CalledOperatorID;
            runtimeCDR.CallingOperatorID = mappedCDR.CallingOperatorID;
            runtimeCDR.OrigCallingNumber = mappedCDR.OrigCallingNumber;
            runtimeCDR.OrigCalledNumber = mappedCDR.OrigCalledNumber;
            runtimeCDR.DataSourceId = mappedCDR.DataSourceId;
            runtimeCDR.CallingNumber = mappedCDR.CallingNumber;
            runtimeCDR.CDRType = (int)mappedCDR.CDRType;
            runtimeCDR.IsCorrelated = mappedCDR.IsCorrelated;
            runtimeCDR.CallingNumberType = (int?)mappedCDR.CallingNumberType;
            runtimeCDR.CreatedTime = mappedCDR.CreatedTime;
            runtimeCDR.ClientId = mappedCDR.ClientId;
            runtimeCDR.OriginatedZoneId = mappedCDR.OriginatedZoneId;
            return runtimeCDR;
        }

        public void UpdateMappedCDRs(UpdatedMappedCDRs updatedMappedCDRs)
        {
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(dataRecordStorage);
            List<string> fieldsToJoin = new List<string>();
            List<string> fieldsToUpdate = new List<string>();
            fieldsToJoin.Add("ID");
            fieldsToUpdate.Add("IsCorrelated");
            Entities.CDRCorrelationBatch correlationBatch = new Entities.CDRCorrelationBatch();
            foreach (var mappedCDR in updatedMappedCDRs.MappedCDRsToUpdate)
            {
                correlationBatch.OutputRecordsToInsert.Add(MappedCDRToRuntime(mappedCDR));
            }
            recordStorageDataManager.UpdateRecords(correlationBatch.OutputRecordsToInsert, fieldsToJoin, fieldsToUpdate);
        }

        public TCAnalMappedCDR MappedCDRMapper(dynamic cdr)
        {
            return new TCAnalMappedCDR
            {
                MappedCDRId = cdr.ID,
                DataSourceId = cdr.DataSourceId,
                AttemptDateTime = cdr.AttemptDateTime,
                DurationInSeconds = cdr.DurationInSeconds,
                CalledNumber = cdr.CalledNumber,
                CallingNumber = cdr.CallingNumber,
                CDRType = (CDRType)cdr.CDRType,
                CalledOperatorID = cdr.CalledOperatorID,
                CallingOperatorID = cdr.CallingOperatorID,
                OrigCallingNumber = cdr.OrigCallingNumber,
                OrigCalledNumber = cdr.OrigCalledNumber,
                CreatedTime = cdr.CreatedTime,
                CallingNumberType = (CDRNumberType?)cdr.CallingNumberType,
                IsCorrelated = cdr.IsCorrelated,
                ClientId = cdr.ClientId,
                OriginatedZoneId = cdr.OriginatedZoneId
            };
        }
    }
}
