using System;
using System.Collections.Generic;
using TestCallAnalysis.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

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

        public List<TCAnalCaseCDR> GetCases()
        {
            List<TCAnalCaseCDR> tcanalCaseCDRs = new List<TCAnalCaseCDR>();
            List<DataRecord> allCasesCDRs = GetAllCases();
            if(allCasesCDRs != null && allCasesCDRs.Count > 0)
            {
                foreach (var caseCDR in allCasesCDRs)
                {
                    tcanalCaseCDRs.Add(CaseCDRMapperFromDataRecord(caseCDR));
                }
            }
            return tcanalCaseCDRs;
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetCaseCDRsType(), numberOfIDs, out startingId);
            return startingId;
        }

        public Dictionary<long, string> GetExistingCases()
        {
            Dictionary<long, string> ExcitingCaseCDRs = new Dictionary<long, string>();
            List<DataRecord> allCasesCDRs = GetAllCases();

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

        public List<string> GetExistingCasesCallingNumber()
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
            runtimeCDR.FirstAttempt = caseCDR.FirstAttempt;
            runtimeCDR.LastAttempt = caseCDR.LastAttempt;
            runtimeCDR.NumberOfCDRs = caseCDR.NumberOfCDRs;
            runtimeCDR.StatusId = caseCDR.StatusId;
            runtimeCDR.OperatorID = caseCDR.OperatorID;
            runtimeCDR.ClientId = caseCDR.ClientId;
            runtimeCDR.CreatedTime = caseCDR.CreatedTime;
            runtimeCDR.LastModifiedTime = caseCDR.LastModifiedTime;
            runtimeCDR.CreatedBy = caseCDR.CreatedBy;
            runtimeCDR.LastModifiedBy = caseCDR.LastModifiedBy;
            return runtimeCDR;
        }

        public void InsertCases(List<TCAnalCaseCDR> casesToInsert)
        {
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(dataRecordStorage);
            List<dynamic> runtimeCases = new List<dynamic>();
        
            foreach (var tCAnalCaseCDR in casesToInsert)
            {
                var runtimeCaseCDR = CaseCDRToRuntime(tCAnalCaseCDR);
                runtimeCases.Add(runtimeCaseCDR);
            }
            recordStorageDataManager.InsertRecords(runtimeCases);
        }

        public void UpdateCases(List<TCAnalCaseCDR> casesToUpdate)
        {
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(dataRecordStorage);

            List<dynamic> runtimeCases = new List<dynamic>();
            foreach (var tCAnalCaseCDR in casesToUpdate)
            {
                var runtimeCaseCDR = CaseCDRToRuntime(tCAnalCaseCDR);
                runtimeCases.Add(runtimeCaseCDR);
            }

            List<string> fieldsToJoin = new List<string>();
            List<string> fieldsToUpdate = new List<string>();
            fieldsToJoin.Add("ID");
            fieldsToUpdate.Add("NumberOfCDRs");
            fieldsToUpdate.Add("LastAttempt");
            recordStorageDataManager.UpdateRecords(runtimeCases, fieldsToJoin, fieldsToUpdate);
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

        public bool DoesUserHaveEditAccess()
        {
            GenericBusinessEntityManager manager = new GenericBusinessEntityManager();
            return manager.DoesUserHaveEditAccess(statusBusinessEntityDefinitionId);
        }

        public TCAnalCaseCDR CaseCDRMapper(TCAnalCorrelatedCDR correlatedCDR)
        {
            return new TCAnalCaseCDR()
            {
                CallingNumber = correlatedCDR.ReceivedCallingNumber,
                FirstAttempt = correlatedCDR.AttemptDateTime,
                LastAttempt = correlatedCDR.AttemptDateTime,
                NumberOfCDRs = 1,
                OperatorID = correlatedCDR.CalledOperatorID,
                ClientId = correlatedCDR.ClientId,
                CreatedTime = correlatedCDR.CreatedTime,
                LastModifiedTime = DateTime.Now,
                CreatedBy = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId(),
                LastModifiedBy = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId()
            };
        }
        #endregion


        #region Private Methods
        private TCAnalCaseCDR CaseCDRMapper(GenericBusinessEntity genericBusinessEntity)
        {
            TCAnalCaseCDR caseCDR = new TCAnalCaseCDR();
            caseCDR.CaseId = (long)genericBusinessEntity.FieldValues.GetRecord("ID");
            caseCDR.CallingNumber = (string)genericBusinessEntity.FieldValues.GetRecord("CallingNumber");
            caseCDR.FirstAttempt = (DateTime)genericBusinessEntity.FieldValues.GetRecord("FirstAttempt");
            caseCDR.LastAttempt = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastAttempt");
            caseCDR.NumberOfCDRs = (int)genericBusinessEntity.FieldValues.GetRecord("NumberOfCDRs");
            caseCDR.StatusId = (Guid)genericBusinessEntity.FieldValues.GetRecord("StatusId");
            caseCDR.ClientId = (string)genericBusinessEntity.FieldValues.GetRecord("ClientId");
            caseCDR.OperatorID = (long?)genericBusinessEntity.FieldValues.GetRecord("OperatorID");
            caseCDR.CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime");
            caseCDR.LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime");
            caseCDR.CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy");
            caseCDR.LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy");
            return caseCDR;
        }

        private TCAnalCaseCDR CaseCDRMapperFromDataRecord (DataRecord dataRecord)
        {
            TCAnalCaseCDR caseCDR = new TCAnalCaseCDR();
            caseCDR.CaseId = (long)dataRecord.FieldValues.GetRecord("ID");
            caseCDR.CallingNumber = (string)dataRecord.FieldValues.GetRecord("CallingNumber");
            caseCDR.FirstAttempt = (DateTime)dataRecord.FieldValues.GetRecord("FirstAttempt");
            caseCDR.LastAttempt = (DateTime)dataRecord.FieldValues.GetRecord("LastAttempt");
            caseCDR.NumberOfCDRs = (int)dataRecord.FieldValues.GetRecord("NumberOfCDRs");
            caseCDR.StatusId = (Guid)dataRecord.FieldValues.GetRecord("StatusId");
            caseCDR.ClientId = (string)dataRecord.FieldValues.GetRecord("ClientId");
            caseCDR.OperatorID = (long?)dataRecord.FieldValues.GetRecord("OperatorID");
            caseCDR.CreatedTime = (DateTime)dataRecord.FieldValues.GetRecord("CreatedTime");
            caseCDR.LastModifiedTime = (DateTime)dataRecord.FieldValues.GetRecord("LastModifiedTime");
            caseCDR.CreatedBy = (int)dataRecord.FieldValues.GetRecord("CreatedBy");
            caseCDR.LastModifiedBy = (int)dataRecord.FieldValues.GetRecord("LastModifiedBy");
            return caseCDR;
        }

        private Type GetCaseCDRsType()
        {
            return this.GetType();
        }

        #endregion
    }
}
