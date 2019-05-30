using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TestCallAnalysis.Business;
using TestCallAnalysis.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{

    #region Arguments

    public class PrepareCDRCasesInput
    {
        public MemoryQueue<CDRCaseBatch> InputQueue { get; set; }
        public MemoryQueue<PrepareCDRCasesOutput> OutputQueue { get; set; }
    }
    public class PrepareCDRCasesToInsert
    {
        public List<dynamic> TCAnalListToInsert { get; set; }

        public PrepareCDRCasesToInsert()
        {
            TCAnalListToInsert = new List<dynamic>();
        }
    }

    public class PrepareCDRCasesOutput
    {
        public List<TCAnalCaseCDR> CasesToInsert { get; set; }
        public List<TCAnalCaseCDR> CasesToUpdate { get; set; }

    }

    #endregion

    public class PrepareCDRCases : DependentAsyncActivity<PrepareCDRCasesInput>
    {
        [RequiredArgument]
        public InOutArgument<MemoryQueue<CDRCaseBatch>> InputQueue { get; set; }
        [RequiredArgument]
        public OutArgument<MemoryQueue<PrepareCDRCasesOutput>> PrepareCDRCasesOutput { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.PrepareCDRCasesOutput.Get(context) == null)
                this.PrepareCDRCasesOutput.Set(context, new MemoryQueue<PrepareCDRCasesOutput>());

            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareCDRCasesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCDRCasesInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.PrepareCDRCasesOutput.Get(context),
            };
        }

        protected override void DoWork(PrepareCDRCasesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            WhiteListManager whiteListManager = new WhiteListManager();
            CaseCDRManager caseCDRManager = new CaseCDRManager();
            var suspectStatusId = new Guid("43f65fbf-ba78-4211-a0bb-88edc91b26ff");
            var fraudStatusId = new Guid("4ea323c2-56ba-46db-a84d-5792009924a3");

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    if (inputArgument.InputQueue != null && inputArgument.InputQueue.Count > 0)
                    {
                        hasItem = inputArgument.InputQueue.TryDequeue((recordBatch) =>
                        {
                            if (recordBatch.CaseCDRsToInsert != null && recordBatch.CaseCDRsToInsert.Count > 0)
                            {

                                PrepareCDRCasesOutput prepareCDRCasesOutput = new PrepareCDRCasesOutput();
                                prepareCDRCasesOutput.CasesToInsert = new List<TCAnalCaseCDR>();
                                prepareCDRCasesOutput.CasesToUpdate = new List<TCAnalCaseCDR>();

                                TCAnalCaseCDR existingNullCaseEntity = null;
                                Dictionary<string, List<TCAnalCaseCDR>> existingCasesCDRs = caseCDRManager.GetNotCleanCases(out existingNullCaseEntity);

                                Dictionary<string, TCAnalCaseCDR> casesToInsert = new Dictionary<string, TCAnalCaseCDR>();
                                Dictionary<string, TCAnalCaseCDR> casesToUpdate = new Dictionary<string, TCAnalCaseCDR>();
                                TCAnalCaseCDR nullCaseEntity = null;
                                foreach (var record in recordBatch.CaseCDRsToInsert)
                                {
                                    if (record.CallingOperatorID.HasValue)
                                    {
                                        List<string> whiteList = whiteListManager.GetWhiteListByOperatorId(record.CallingOperatorID.Value);
                                        if (whiteList != null && whiteList.Count > 0 && whiteList.Contains(record.ReceivedCallingNumber))
                                            continue;
                                    }

                                    TCAnalCaseCDR tcanalCaseCDR = new TCAnalCaseCDR();
                                    tcanalCaseCDR = caseCDRManager.CaseCDRMapper(record);

                                    if (record.ReceivedCallingNumber == null)
                                    {
                                        if (existingNullCaseEntity != null)
                                        {
                                            existingNullCaseEntity.NumberOfCDRs++;
                                            existingNullCaseEntity.LastAttempt = tcanalCaseCDR.LastAttempt;
                                            prepareCDRCasesOutput.CasesToUpdate.Add(existingNullCaseEntity);
                                        }
                                        else
                                        {
                                            tcanalCaseCDR.StatusId = suspectStatusId;
                                            if (nullCaseEntity == null)
                                                nullCaseEntity = tcanalCaseCDR;
                                            else
                                            {
                                                nullCaseEntity.NumberOfCDRs++;
                                                nullCaseEntity.LastAttempt = tcanalCaseCDR.LastAttempt;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (existingCasesCDRs != null && existingCasesCDRs.ContainsKey(record.ReceivedCallingNumber))
                                        {
                                            List<TCAnalCaseCDR> cases = existingCasesCDRs.GetRecord(record.ReceivedCallingNumber);
                                            if (cases != null && cases.Count > 0)
                                            {
                                                var caseCDR = cases.OrderByDescending(x => x.CreatedTime).First();
                                                GetUpdateOrUpdateItem(caseCDR, casesToUpdate);
                                            }
                                        }
                                        else
                                        {
                                            if (record.ReceivedCallingNumberType.HasValue && record.ReceivedCallingNumberType != ReceivedCallingNumberType.International)
                                            {
                                                tcanalCaseCDR.StatusId = fraudStatusId;
                                                GetInsertOrUpdateInsertedItem(tcanalCaseCDR, casesToInsert);

                                                continue;
                                            }
                                            else if (!record.ReceivedCallingNumberType.HasValue)
                                            {
                                                tcanalCaseCDR.StatusId = suspectStatusId;
                                                GetInsertOrUpdateInsertedItem(tcanalCaseCDR, casesToInsert);
                                                continue;
                                            }
                                            else if (record.GeneratedCalledNumber == null && record.GeneratedCallingNumber == null)
                                            {
                                                tcanalCaseCDR.StatusId = suspectStatusId;
                                                GetInsertOrUpdateInsertedItem(tcanalCaseCDR, casesToInsert);
                                                continue;
                                            }
                                        }
                                    }
                                }

                                var numberOfInsertedItems = casesToInsert.Count;
                                if (nullCaseEntity != null)
                                    numberOfInsertedItems++;

                                long caseCDRStartingId = caseCDRManager.ReserveIDRange(numberOfInsertedItems);
                                foreach (var caseCDR in casesToInsert.Values)
                                {
                                    caseCDR.CaseId = caseCDRStartingId++;
                                    prepareCDRCasesOutput.CasesToInsert.Add(caseCDR);
                                }
                                if (nullCaseEntity != null)
                                {
                                    nullCaseEntity.CaseId = caseCDRStartingId++;
                                    prepareCDRCasesOutput.CasesToInsert.Add(nullCaseEntity);

                                }

                                foreach (var caseCDR in casesToUpdate.Values)
                                {
                                    prepareCDRCasesOutput.CasesToUpdate.Add(caseCDR);
                                }

                                inputArgument.OutputQueue.Enqueue(prepareCDRCasesOutput);
                            }

                        });
                    }
                    else
                    {
                        hasItem = false;
                    }
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        private void GetInsertOrUpdateInsertedItem(TCAnalCaseCDR tcanalCaseCDR, Dictionary<string, TCAnalCaseCDR> casesToInsert)
        {
            TCAnalCaseCDR insertedItem = null;
            if (casesToInsert.TryGetValue(tcanalCaseCDR.CallingNumber, out insertedItem))
            {
                insertedItem.NumberOfCDRs++;
                insertedItem.LastAttempt = tcanalCaseCDR.LastAttempt;
            }
            else
            {
                casesToInsert.Add(tcanalCaseCDR.CallingNumber, tcanalCaseCDR);
            }
        }

        private void GetUpdateOrUpdateItem(TCAnalCaseCDR tcanalCaseCDR, Dictionary<string, TCAnalCaseCDR> casesToUpdate)
        {
            TCAnalCaseCDR updatedItem = null;
            if (casesToUpdate.TryGetValue(tcanalCaseCDR.CallingNumber, out updatedItem))
            {
                updatedItem.NumberOfCDRs++;
                updatedItem.LastAttempt = tcanalCaseCDR.LastAttempt;
            }
            else
            {
                casesToUpdate.Add(tcanalCaseCDR.CallingNumber, tcanalCaseCDR);
            }
        }
    }
}
