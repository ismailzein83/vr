using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCallAnalysis.Entities;
using Vanrise.GenericData.Business;

namespace TestCallAnalysis.Business
{
    public class MappedCDRManager
    {
        static Guid dataRecordStorage = new Guid("58FCA073-8F5C-4A56-A4AF-025EB3B8BB60");

        public dynamic MappedCDRToRuntime(TCAnalMappedCDR mappedCDR)
        {
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("TCAnal_MappedCDR");
            dynamic runtimeCDR = Activator.CreateInstance(cdrRuntimeType) as dynamic;
            runtimeCDR.ID = mappedCDR.MappedCDRId;
            runtimeCDR.AttemptDateTime = mappedCDR.AttemptDateTime;
            runtimeCDR.DurationInSeconds = mappedCDR.DurationInSeconds;
            runtimeCDR.CalledNumber = mappedCDR.CalledNumber;
            runtimeCDR.OperatorID = mappedCDR.OperatorID;
            runtimeCDR.OrigCallingNumber = mappedCDR.OrigCallingNumber;
            runtimeCDR.OrigCalledNumber = mappedCDR.OrigCalledNumber;
            runtimeCDR.DataSourceId = mappedCDR.DataSourceId;
            runtimeCDR.CallingNumber = mappedCDR.CallingNumber;
            runtimeCDR.CDRType = (int)mappedCDR.CDRType;
            runtimeCDR.IsCorrelated = mappedCDR.IsCorrelated;
            runtimeCDR.CallingNumberType = mappedCDR.CallingNumberType;
            runtimeCDR.CalledNumberType = mappedCDR.CalledNumberType;
            runtimeCDR.CreatedTime = mappedCDR.CreatedTime;
            runtimeCDR.LastModifiedTime = mappedCDR.LastModifiedTime;
            runtimeCDR.CreatedBy = mappedCDR.CreatedBy;
            runtimeCDR.LastModifiedBy = mappedCDR.LastModifiedBy;
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
                if (updatedMappedCDRs.UpdatedIds.IndexOf(mappedCDR.MappedCDRId) != -1)
                {
                    correlationBatch.OutputRecordsToInsert.Add(MappedCDRToRuntime(mappedCDR));
                }
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
                OperatorID = cdr.OperatorID,
                OrigCallingNumber = cdr.OrigCallingNumber,
                OrigCalledNumber = cdr.OrigCalledNumber,
                CreatedTime = cdr.CreatedTime,
                CallingNumberType = cdr.CallingNumberType,
                CalledNumberType = cdr.CalledNumberType,
                IsCorrelated = cdr.IsCorrelated,
                LastModifiedTime = cdr.LastModifiedTime,
                CreatedBy = cdr.CreatedBy,
                LastModifiedBy = cdr.LastModifiedBy
            };
        }
    }
}
