using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TestCallAnalysis.Business;
using TestCallAnalysis.Entities;
using Vanrise.BusinessProcess;
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

            PrepareCDRCasesOutput prepareCDRCasesOutput = new PrepareCDRCasesOutput();
            prepareCDRCasesOutput.CasesToInsert = new List<TCAnalCaseCDR>();
            prepareCDRCasesOutput.CasesToUpdate = new List<TCAnalCaseCDR>();

            List<string> existingCasesCallingNumbers = caseCDRManager.GetExistingCasesCallingNumber();
            List<TCAnalCaseCDR> existingCasesCDRs = caseCDRManager.GetCases();

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

                                    if (existingCasesCallingNumbers != null && existingCasesCDRs != null && existingCasesCallingNumbers.Count() > 0 && existingCasesCallingNumbers.IndexOf(tcanalCaseCDR.CallingNumber) != -1)
                                    {
                                        var caseCDR = existingCasesCDRs.Find(itm => itm.CallingNumber == tcanalCaseCDR.CallingNumber && itm.CreatedTime == tcanalCaseCDR.CreatedTime);
                                        tcanalCaseCDR.CaseId = caseCDR.CaseId;
                                        tcanalCaseCDR.NumberOfCDRs = ++caseCDR.NumberOfCDRs;
                                        prepareCDRCasesOutput.CasesToUpdate.Add(tcanalCaseCDR);
                                    }
                                    else
                                    {
                                        if (record.ReceivedCallingNumberType.HasValue && record.ReceivedCallingNumberType != ReceivedCallingNumberType.International)
                                        {
                                            tcanalCaseCDR.StatusId = new Guid("4ea323c2-56ba-46db-a84d-5792009924a3"); // Fraud
                                            prepareCDRCasesOutput.CasesToInsert.Add(tcanalCaseCDR);
                                            continue;
                                        }
                                        else if (!record.ReceivedCallingNumberType.HasValue)
                                        {
                                            tcanalCaseCDR.StatusId = new Guid("43f65fbf-ba78-4211-a0bb-88edc91b26ff"); // Suspect
                                            prepareCDRCasesOutput.CasesToInsert.Add(tcanalCaseCDR);
                                            continue;
                                        }
                                        else if (record.GeneratedCalledNumber == null && record.GeneratedCallingNumber == null)
                                        {
                                            tcanalCaseCDR.StatusId = new Guid("43f65fbf-ba78-4211-a0bb-88edc91b26ff"); // Suspect
                                            prepareCDRCasesOutput.CasesToInsert.Add(tcanalCaseCDR);
                                            continue;
                                        }
                                    }
                                }

                                long caseCDRStartingId = caseCDRManager.ReserveIDRange(prepareCDRCasesOutput.CasesToInsert.Count);
                                foreach(var caseCDR in prepareCDRCasesOutput.CasesToInsert)
                                {
                                    caseCDR.CaseId = caseCDRStartingId++;
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
    }
}
