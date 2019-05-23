using System;
using System.Collections.Generic;
using System.Linq;
using TestCallAnalysis.Entities;
using Vanrise.Common.Business;
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
            runtimeCDR.GeneratedCalledNumber = correlatedCDR.GeneratedCalledNumber;
            runtimeCDR.ReceivedCalledNumber = correlatedCDR.ReceivedCalledNumber;
            runtimeCDR.GeneratedCallingNumber = correlatedCDR.GeneratedCallingNumber;
            runtimeCDR.ReceivedCallingNumber = correlatedCDR.ReceivedCallingNumber;
            runtimeCDR.CalledOperatorID = correlatedCDR.CalledOperatorID;
            runtimeCDR.CallingOperatorID = correlatedCDR.CallingOperatorID;
            runtimeCDR.OrigGeneratedCallingNumber = correlatedCDR.OrigGeneratedCallingNumber;
            runtimeCDR.OrigGeneratedCalledNumber = correlatedCDR.OrigGeneratedCalledNumber;
            runtimeCDR.OrigReceivedCallingNumber = correlatedCDR.OrigReceivedCallingNumber;
            runtimeCDR.OrigReceivedCalledNumber = correlatedCDR.OrigReceivedCalledNumber;
            runtimeCDR.ReceivedCallingNumberType = (int?)correlatedCDR.ReceivedCallingNumberType;
            runtimeCDR.CaseId = correlatedCDR.CaseId;
            runtimeCDR.CreatedTime = correlatedCDR.CreatedTime;
            runtimeCDR.GeneratedId = correlatedCDR.GeneratedId;
            runtimeCDR.ReceivedId = correlatedCDR.ReceivedId;
            runtimeCDR.ClientId = correlatedCDR.ClientId;
            runtimeCDR.OriginatedGeneratedZoneId = correlatedCDR.OriginatedGeneratedZoneId;
            runtimeCDR.OriginatedReceivedZoneId = correlatedCDR.OriginatedReceivedZoneId;
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
                var caseId = listOfCaseCDRsCallingNumbers.FirstOrDefault(x => x.Value == correlatedCDR.ReceivedCallingNumber).Key;
                if (caseId != 0)
                {
                    correlatedCDR.CaseId = caseId;
                    correlationBatch.OutputRecordsToInsert.Add(correlatedCDR);
                }
            }
            List<string> fieldsToJoin = new List<string>();
            List<string> fieldsToUpdate = new List<string>();
            fieldsToJoin.Add("ID");
            fieldsToUpdate.Add("CaseId");

            if (correlationBatch.OutputRecordsToInsert.Count() > 0)
                recordStorageDataManager.UpdateRecords(correlationBatch.OutputRecordsToInsert, fieldsToJoin, fieldsToUpdate);

            return correlationBatch.OutputRecordsToInsert.Count;
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetCorrelatedCDRsType(), numberOfIDs, out startingId);
            return startingId;
        }

        public TCAnalCorrelatedCDR CorrelatedCDRMapper(TCAnalMappedCDR mappedCDR)
        {
            return new TCAnalCorrelatedCDR
            {
                AttemptDateTime = mappedCDR.AttemptDateTime,
                DurationInSeconds = mappedCDR.DurationInSeconds,
                ReceivedCalledNumber = mappedCDR.CalledNumber,
                ReceivedCallingNumber = mappedCDR.CallingNumber,
                CalledOperatorID = mappedCDR.CalledOperatorID,
                CallingOperatorID = mappedCDR.CallingOperatorID,
                OrigReceivedCallingNumber = mappedCDR.OrigCallingNumber,
                OrigReceivedCalledNumber = mappedCDR.OrigCalledNumber,
                ReceivedCallingNumberType = (ReceivedCallingNumberType?)mappedCDR.CallingNumberType,
                CaseId = null,
                CreatedTime = mappedCDR.CreatedTime,
                ReceivedId = mappedCDR.MappedCDRId,
                ClientId = mappedCDR.ClientId,
                OriginatedReceivedZoneId = mappedCDR.OriginatedZoneId
            };
        }

        #endregion

        #region Private Methods
        private Type GetCorrelatedCDRsType()
        {
            return this.GetType();
        }
        #endregion
    }
}
