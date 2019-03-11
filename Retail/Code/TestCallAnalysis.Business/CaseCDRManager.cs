using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCallAnalysis.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
using Vanrise.Entities;

namespace TestCallAnalysis.Business
{
    public class CaseCDRManager
    {
        static Guid dataRecordStorage = new Guid("529032BA-D2C2-4612-88C2-FF64AEE9E6CC");
        static Guid statusBusinessEntityDefinitionId = new Guid("1264c992-479e-45fb-8e8a-7edd54a9bc18");

        #region Public Methods
        public List<DataRecord> GetAllCases()
        {
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(dataRecordStorage);
            List<string> columns = new List<string>();
            columns.Add("ID");
            columns.Add("CallingNumber");
            columns.Add("CalledNumber");
            columns.Add("FirstAttempt");
            columns.Add("LastAttempt");
            columns.Add("NumberOfCDRs");
            columns.Add("StatusId");
            columns.Add("OperatorID");
            columns.Add("CreatedTime");
            columns.Add("LastModifiedTime");
            columns.Add("CreatedBy");
            columns.Add("LastModifiedBy");
            var allCases = recordStorageDataManager.GetAllDataRecords(columns);
            if (allCases != null && allCases.Count > 0)
                return allCases;
            else
                return null;
        }

        public Dictionary<long, string> GetCasesCDRCallingNumbers()
        {
            Dictionary<long, string> ExcitingCaseCDRs = new Dictionary<long, string>();
            List<DataRecord> allCasesCDRs = new List<DataRecord>();
            allCasesCDRs = GetAllCases();
            if (allCasesCDRs != null && allCasesCDRs.Count > 0)
            {
                foreach (var record in allCasesCDRs)
                {
                    var caseId = (long)record.FieldValues.GetRecord("ID");
                    var callingNb = record.FieldValues.GetRecord("CallingNumber");

                    string callingNumber;
                    if (String.IsNullOrEmpty((String)callingNb))
                        callingNumber = null;
                    else
                        callingNumber = callingNb.ToString();

                    ExcitingCaseCDRs.Add(caseId, callingNumber);
                }
                return ExcitingCaseCDRs;
            }
            else
                return null;
        }
        public List<string> GetCasesCDRCallingNumbersList()
        {
            var allCasesCDRs = GetAllCases();
            if (allCasesCDRs != null && allCasesCDRs.Count > 0)
            {
                List<string> callingNumbersList = new List<string>();
                foreach (var caseCDRRecord in allCasesCDRs)
                {
                    string callingNumber;
                    var callingNb = caseCDRRecord.FieldValues.GetRecord("CallingNumber");
                    if (String.IsNullOrEmpty((String)callingNb))
                        callingNumber = null;
                    else
                        callingNumber = callingNb.ToString();
                    callingNumbersList.Add(callingNumber);
                }
                return callingNumbersList;
            }
            else
                return null;
        }

        public dynamic CaseCDRToRuntime(TCAnalCaseCDR caseCDR)
        {
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("TCAnal_CaseCDR");
            dynamic runtimeCDR = Activator.CreateInstance(cdrRuntimeType) as dynamic;
            runtimeCDR.ID = caseCDR.CaseId;
            runtimeCDR.CallingNumber = caseCDR.CallingNumber;
            runtimeCDR.CalledNumber = caseCDR.CalledNumber;
            runtimeCDR.FirstAttempt = caseCDR.FirstAttempt;
            runtimeCDR.LastAttempt = caseCDR.LastAttempt;
            runtimeCDR.NumberOfCDRs = caseCDR.NumberOfCDRs;
            runtimeCDR.StatusId = caseCDR.StatusId;
            runtimeCDR.OperatorID = caseCDR.OperatorID;
            runtimeCDR.CreatedTime = caseCDR.CreatedTime;
            runtimeCDR.LastModifiedTime = caseCDR.LastModifiedTime;
            runtimeCDR.CreatedBy = caseCDR.CreatedBy;
            runtimeCDR.LastModifiedBy = caseCDR.LastModifiedBy;
            return runtimeCDR;
        }

        public List<dynamic> CaseCDRsToRuntime(List<TCAnalCaseCDR> tCAnalCaseCDRs)
        {
            List<dynamic> result = new List<dynamic>();
            foreach (var tCAnalCaseCDR in tCAnalCaseCDRs)
            {
                var runtimeCaseCDR = CaseCDRToRuntime(tCAnalCaseCDR);
                result.Add(runtimeCaseCDR);
            }
            return result;
        }

        public void InsertCases(List<dynamic> casesToInsert)
        {
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(dataRecordStorage);
            recordStorageDataManager.InsertRecords(casesToInsert);
        }

        public void UpdateCases(List<dynamic> casesToUpdate)
        {
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(dataRecordStorage);
            List<string> fieldsToJoin = new List<string>();
            List<string> fieldsToUpdate = new List<string>();
            fieldsToJoin.Add("ID");
            fieldsToUpdate.Add("NumberOfCDRs");
            fieldsToUpdate.Add("LastAttempt");
            recordStorageDataManager.UpdateRecords(casesToUpdate, fieldsToJoin, fieldsToUpdate);
        }

        public TCAnalCaseCDR GetCaseCDREntity(Guid businessEntityDefinitionId, Object genericBusinessEntityId)
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            if (genericBusinessEntityId != null && businessEntityDefinitionId != null)
            {
                var genericBusinessEntity = genericBusinessEntityManager.GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);
                TCAnalCaseCDR caseCDR = CaseCDRMapper(genericBusinessEntity);
                return caseCDR;
            }
            else
                return null;
        }

        public UpdateOperationOutput<GenericBusinessEntityDetail> UpdateCaseCDRStatus(CaseCDRToUpdate caseCDRToUpdate)
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            if (statusBusinessEntityDefinitionId != null)
            {
                GenericBusinessEntity genericBusinessEntity = genericBusinessEntityManager.GetGenericBusinessEntity(caseCDRToUpdate.CaseCDRId, statusBusinessEntityDefinitionId);
                GenericBusinessEntityToUpdate genericBusinessEntityToUpdate = new GenericBusinessEntityToUpdate();
                genericBusinessEntityToUpdate.GenericBusinessEntityId = caseCDRToUpdate.CaseCDRId;
                genericBusinessEntityToUpdate.BusinessEntityDefinitionId = statusBusinessEntityDefinitionId;
                genericBusinessEntityToUpdate.FilterGroup = null;
                genericBusinessEntityToUpdate.FieldValues = genericBusinessEntity.FieldValues;
                genericBusinessEntityToUpdate.FieldValues["StatusId"] = caseCDRToUpdate.StatusId;
                genericBusinessEntityToUpdate.FieldValues.Remove("ID");
                genericBusinessEntityToUpdate.FieldValues.Remove("LastModifiedTime");
                genericBusinessEntityToUpdate.FieldValues.Remove("LastModifiedBy");

                return genericBusinessEntityManager.UpdateGenericBusinessEntity(genericBusinessEntityToUpdate);
            }
            else
                return null;
             
        }

        public TCAnalCaseCDR CaseCDRMapper(TCAnalCorrelatedCDR correlatedCDR)
        {
            return new TCAnalCaseCDR()
            {
                CaseId = correlatedCDR.CorrelatedCDRId,
                CallingNumber = correlatedCDR.ReceivedCallingNumber,
                CalledNumber = correlatedCDR.CalledNumber,
                FirstAttempt = correlatedCDR.AttemptDateTime,
                LastAttempt = correlatedCDR.AttemptDateTime,
                NumberOfCDRs = 1,
                OperatorID = correlatedCDR.OperatorID,
                CreatedTime = correlatedCDR.CreatedTime,
                LastModifiedTime = correlatedCDR.LastModifiedTime,
                CreatedBy = correlatedCDR.CreatedBy,
                LastModifiedBy = correlatedCDR.LastModifiedBy,
            };
        }
        #endregion


        #region Private Methods
        private TCAnalCaseCDR CaseCDRMapper(GenericBusinessEntity genericBusinessEntity)
        {
            TCAnalCaseCDR caseCDR = new TCAnalCaseCDR();
            caseCDR.CaseId = (long)genericBusinessEntity.FieldValues.GetRecord("ID");
            caseCDR.CallingNumber = (string)genericBusinessEntity.FieldValues.GetRecord("ReceivedCallingNumber");
            caseCDR.CalledNumber = (string)genericBusinessEntity.FieldValues.GetRecord("CalledNumber");
            caseCDR.FirstAttempt = (DateTime)genericBusinessEntity.FieldValues.GetRecord("FirstAttempt");
            caseCDR.LastAttempt = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastAttempt");
            caseCDR.NumberOfCDRs = (int)genericBusinessEntity.FieldValues.GetRecord("NumberOfCDRs");
            caseCDR.StatusId = (Guid)genericBusinessEntity.FieldValues.GetRecord("StatusId");
            caseCDR.OperatorID = (long)genericBusinessEntity.FieldValues.GetRecord("OperatorID");
            caseCDR.CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime");
            caseCDR.LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime");
            caseCDR.CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy");
            caseCDR.LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy");
            return caseCDR;
        }
    
        #endregion
    }
}
