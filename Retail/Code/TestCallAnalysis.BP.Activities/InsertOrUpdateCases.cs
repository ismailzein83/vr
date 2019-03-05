using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{
    #region Arguments
    public class InsertCaseCDRsInput
    {
        public MemoryQueue<PrepareCDRCasesToInsert> InputQueue { get; set; }
        public MemoryQueue<UpdateCorrelatedCDRsBatch> UpdateCorrelatedCDRs { get; set; }
    }

    public class InsertCaseCDRsOutput
    {
    }
    #endregion

    public class InsertOrUpdateCases : DependentAsyncActivity<InsertCaseCDRsInput, InsertCaseCDRsOutput>
    {
        [RequiredArgument]
        public InOutArgument<MemoryQueue<PrepareCDRCasesToInsert>> InputQueueToInsert { get; set; }

        [RequiredArgument]
        public InOutArgument<MemoryQueue<UpdateCorrelatedCDRsBatch>> UpdateCorrelatedCDRs { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.InputQueueToInsert.Get(context) == null)
                this.InputQueueToInsert.Set(context, new MemoryQueue<PrepareCDRCasesToInsert>());
            if (this.UpdateCorrelatedCDRs.Get(context) == null)
                this.UpdateCorrelatedCDRs.Set(context, new MemoryQueue<UpdateCorrelatedCDRsBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override InsertCaseCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new InsertCaseCDRsInput()
            {
                InputQueue = this.InputQueueToInsert.Get(context),
                UpdateCorrelatedCDRs = this.UpdateCorrelatedCDRs.Get(context),
            };
        }

        protected override InsertCaseCDRsOutput DoWorkWithResult(InsertCaseCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            Guid dataRecordStorage = new Guid("529032BA-D2C2-4612-88C2-FF64AEE9E6CC");
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(dataRecordStorage);
            int maxDBNumberQuery = recordStorageDataManager.GetDBQueryMaxParameterNumber();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    if (inputArgument.InputQueue != null && inputArgument.InputQueue.Count > 0)
                    {
                        hasItem = inputArgument.InputQueue.TryDequeue((caseCDRsList) =>
                        {
                            DateTime batchStartTime = DateTime.Now;

                            // Get all Records
                            List<string> columns = new List<string>();
                            columns.Add("CallingNumber");
                            var allCasesCDRs = recordStorageDataManager.GetAllDataRecords(columns);

                            List<dynamic> caseCDRsToInsert = new List<dynamic>();
                            List<string> existingCallingNumbers = new List<string>();

                            if (allCasesCDRs != null && allCasesCDRs.Count > 0)
                            {
                                foreach (var record in allCasesCDRs)
                                {
                                    var callingNb = record.FieldValues.GetRecord("CallingNumber").ToString();
                                    existingCallingNumbers.Add(callingNb);
                                }

                                if (existingCallingNumbers.Count > 0)
                                {
                                    PrepareCDRCasesToInsert cdrCaseBatch = new PrepareCDRCasesToInsert();
                                    foreach (var caseCDR in caseCDRsList.TCAnalListToInsert)
                                    {
                                        var indexCaseCDR = existingCallingNumbers.IndexOf(caseCDR.CallingNumber);

                                        if (indexCaseCDR != -1)
                                        {
                                            caseCDR.NumberOfCDRs++;

                                            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
                                            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("TCAnal_CaseCDR");

                                            dynamic runtimeCDR = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                                            runtimeCDR.ID = caseCDR.ID;
                                            runtimeCDR.CallingNumber = caseCDR.CallingNumber;
                                            runtimeCDR.CalledNumber = caseCDR.CalledNumber;
                                            runtimeCDR.FirstAttempt = caseCDR.FirstAttempt;
                                            runtimeCDR.LastAttempt = caseCDR.LastAttempt;
                                            runtimeCDR.NumberOfCDRs = caseCDR.NumberOfCDRs;
                                            runtimeCDR.StatusId = caseCDR.StatusId;
                                            runtimeCDR.OperatorID = caseCDR.OperatorID;
                                            cdrCaseBatch.TCAnalListToInsert.Add(runtimeCDR);
                                        }

                                        else
                                            caseCDRsToInsert.Add(caseCDR);

                                    }

                                    // Update case CDRs
                                    if (cdrCaseBatch != null && cdrCaseBatch.TCAnalListToInsert.Count > 0)
                                    {
                                        List<string> fieldsToJoin = new List<string>();
                                        List<string> fieldsToUpdate = new List<string>();
                                        fieldsToJoin.Add("ID");
                                        fieldsToUpdate.Add("NumberOfCDRs");
                                        fieldsToUpdate.Add("LastAttempt");

                                        recordStorageDataManager.UpdateRecords(cdrCaseBatch.TCAnalListToInsert, fieldsToJoin, fieldsToUpdate);
                                        double elapsedTimeToUpdate = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                        handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Update case CDRs Batch is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                                            cdrCaseBatch.TCAnalListToInsert.Count, elapsedTimeToUpdate.ToString());
                                    }

                                    // Insert Case CDRs
                                    if (caseCDRsToInsert != null && caseCDRsToInsert.Count > 0)
                                    {
                                        recordStorageDataManager.InsertRecords(caseCDRsToInsert);
                                        double elapsedTimeToInsert = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                        handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert case CDRs Batch is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                                            caseCDRsToInsert.Count, elapsedTimeToInsert.ToString());
                                    }
                                }
                            }
                            else
                            {
                                // Insert Case CDRs if no existing cases
                                recordStorageDataManager.InsertRecords(caseCDRsList.TCAnalListToInsert);
                                double elapsedTimeToInsert = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert case CDRs Batch is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                                    caseCDRsList.TCAnalListToInsert.Count, elapsedTimeToInsert.ToString());
                            }

                            // Get all records to send them to UpdateCorrelatedCDRs Activity 
                            List<string> columns2 = new List<string>();
                            columns2.Add("CallingNumber");
                            columns2.Add("ID");
                            var allCasesCDRs2 = recordStorageDataManager.GetAllDataRecords(columns2);

                            if (allCasesCDRs2 != null && allCasesCDRs2.Count > 0)
                            {
                                UpdateCorrelatedCDRsBatch updateCorrelatedCDRsBatch = new UpdateCorrelatedCDRsBatch();
                                foreach (var record in allCasesCDRs2)
                                {
                                    var caseId = (long)record.FieldValues.GetRecord("ID");
                                    var callingNb = record.FieldValues.GetRecord("CallingNumber");

                                    string callingNumber;
                                    if (String.IsNullOrEmpty((String)callingNb))
                                       callingNumber = null;
                                    
                                    else
                                        callingNumber = callingNb.ToString();
                                 
                                    updateCorrelatedCDRsBatch.ExcitingCaseCDRs.Add(caseId, callingNumber);
                                }
                                inputArgument.UpdateCorrelatedCDRs.Enqueue(updateCorrelatedCDRsBatch);
                            }
                        });
                    }
                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert case CDRs is done.");
            return new InsertCaseCDRsOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, InsertCaseCDRsOutput result)
        {
        }
    }
}
