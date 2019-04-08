﻿using System;
using System.Collections.Generic;
using System.Linq;
using TestCallAnalysis.Entities;
using Vanrise.GenericData.Business;

namespace TestCallAnalysis.Business
{
    public class CorrelatedCDRManager
    {
        static Guid dataRecordStorage = new Guid("F5E8B48B-70E0-46B8-BA69-9A4C37E6A520");
        #region Public Methods

        public dynamic CorrelatedCDRToRuntime(TCAnalCorrelatedCDR correlatedCDR)
        {
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("TCAnal_CorrelatedCDR");
            dynamic runtimeCDR = Activator.CreateInstance(cdrRuntimeType) as dynamic;
            runtimeCDR.ID = correlatedCDR.CorrelatedCDRId;
            runtimeCDR.AttemptDateTime = correlatedCDR.AttemptDateTime;
            runtimeCDR.DurationInSeconds = correlatedCDR.DurationInSeconds;
            runtimeCDR.CalledNumber = correlatedCDR.CalledNumber;
            runtimeCDR.OperatorID = correlatedCDR.OperatorID;
            runtimeCDR.OrigCallingNumber = correlatedCDR.OrigCallingNumber;
            runtimeCDR.OrigCalledNumber = correlatedCDR.OrigCalledNumber;
            runtimeCDR.GeneratedCallingNumber = correlatedCDR.GeneratedCallingNumber;
            runtimeCDR.ReceivedCallingNumber = correlatedCDR.ReceivedCallingNumber;
            runtimeCDR.ReceivedCallingNumberType = (int?)correlatedCDR.ReceivedCallingNumberType;
            runtimeCDR.ReceivedCallingNumberOperatorID = correlatedCDR.ReceivedCallingNumberOperatorID;
            runtimeCDR.CaseId = correlatedCDR.CaseId;
            runtimeCDR.CreatedTime = correlatedCDR.CreatedTime;
            return runtimeCDR;
        }

        public List<dynamic> CorrelatedCDRsToRuntime(List<TCAnalCorrelatedCDR> tCAnalCorrelatedCDRs)
        {
            List<dynamic> result = new List<dynamic>();
            foreach (var correlatedCDR in tCAnalCorrelatedCDRs)
            {
                result.Add(CorrelatedCDRToRuntime(correlatedCDR));
            }
            return result;
        }

        public void InsertCorrelatedCDRs(List<dynamic> correlatedCDRsToInsert)
        {
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(dataRecordStorage);
            recordStorageDataManager.InsertRecords(correlatedCDRsToInsert);
        }

        public int UpdateCorrelatedCDRs(List<dynamic> correlatedCDRsToUpdate, Dictionary<long, string> listOfCaseCDRsCallingNumbers)
        {
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(dataRecordStorage);
            Entities.CDRCorrelationBatch correlationBatch = new Entities.CDRCorrelationBatch();
            foreach (var correlatedCDR in correlatedCDRsToUpdate)
            {
                var index = listOfCaseCDRsCallingNumbers.FirstOrDefault(x => x.Value == correlatedCDR.ReceivedCallingNumber).Key;
                if (index != 0)
                    correlatedCDR.CaseId = index;
                correlationBatch.OutputRecordsToInsert.Add(correlatedCDR);
            }
            List<string> fieldsToJoin = new List<string>();
            List<string> fieldsToUpdate = new List<string>();
            fieldsToJoin.Add("ID");
            fieldsToUpdate.Add("CaseId");
            recordStorageDataManager.UpdateRecords(correlationBatch.OutputRecordsToInsert, fieldsToJoin, fieldsToUpdate);
            return correlationBatch.OutputRecordsToInsert.Count;
        }

        public TCAnalCorrelatedCDR CorrelatedCDRMapper(TCAnalMappedCDR mappedCDR)
        {
            return new TCAnalCorrelatedCDR
            {
                CorrelatedCDRId = mappedCDR.MappedCDRId,
                AttemptDateTime = mappedCDR.AttemptDateTime,
                DurationInSeconds = mappedCDR.DurationInSeconds,
                CalledNumber = mappedCDR.CalledNumber,
                ReceivedCallingNumber = mappedCDR.CallingNumber,
                OperatorID = mappedCDR.OperatorID,
                OrigCallingNumber = mappedCDR.OrigCallingNumber,
                OrigCalledNumber = mappedCDR.OrigCalledNumber,
                ReceivedCallingNumberOperatorID = mappedCDR.OperatorID,
                ReceivedCallingNumberType = (ReceivedCallingNumberType?)mappedCDR.CallingNumberType,
                CaseId = null,
                CreatedTime = mappedCDR.CreatedTime
            };
        }

        #endregion
    }
}
