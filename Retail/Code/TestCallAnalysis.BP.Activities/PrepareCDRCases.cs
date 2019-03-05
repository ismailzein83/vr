using System;
using System.Activities;
using System.Collections.Generic;
using TestCallAnalysis.Business;
using TestCallAnalysis.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{

    #region Arguments

    public class PrepareCDRCasesInput
    {
        public MemoryQueue<CDRCaseBatch> InputQueue { get; set; }
        public MemoryQueue<PrepareCDRCasesToInsert> OutputQueue { get; set; }
    }
    public class PrepareCDRCasesToInsert
    {
        public List<dynamic> TCAnalListToInsert { get; set; }

        public PrepareCDRCasesToInsert()
        {
            TCAnalListToInsert = new List<dynamic>();
        }
    }

    #endregion

    public class PrepareCDRCases : DependentAsyncActivity<PrepareCDRCasesInput>
    {
        [RequiredArgument]
        public InOutArgument<MemoryQueue<CDRCaseBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<MemoryQueue<PrepareCDRCasesToInsert>> PrepareCDRCasesToInsert { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.PrepareCDRCasesToInsert.Get(context) == null)
                this.PrepareCDRCasesToInsert.Set(context, new MemoryQueue<PrepareCDRCasesToInsert>());
         
            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareCDRCasesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCDRCasesInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.PrepareCDRCasesToInsert.Get(context),
            };
        }

        protected override void DoWork(PrepareCDRCasesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    if (inputArgument.InputQueue != null && inputArgument.InputQueue.Count > 0)
                    {
                        hasItem = inputArgument.InputQueue.TryDequeue((recordBatch) =>
                        {
                            if (recordBatch.OutputRecordsToInsert != null && recordBatch.OutputRecordsToInsert.Count > 0)
                            {
                                List<TCAnalCaseCDR> tCAnalCaseCDRs = new List<TCAnalCaseCDR>();
                                WhiteListManager whiteListManager = new WhiteListManager();
                             
                                foreach (var record in recordBatch.OutputRecordsToInsert)
                                {
                                    List<string> whiteList = whiteListManager.GetWhiteListByOperatorId(record.OperatorID);

                                    if (whiteList != null && whiteList.Count>0 && whiteList.Contains(record.ReceivedCallingNumber))
                                        continue;

                                    TCAnalCaseCDR tcanalCaseCDR = new TCAnalCaseCDR();
                                    tcanalCaseCDR.ID = record.ID;
                                    tcanalCaseCDR.CallingNumber = record.ReceivedCallingNumber;
                                    tcanalCaseCDR.CalledNumber = record.CalledNumber;
                                    tcanalCaseCDR.FirstAttempt = record.AttemptDateTime;
                                    tcanalCaseCDR.LastAttempt = record.AttemptDateTime; 
                                    tcanalCaseCDR.NumberOfCDRs = 1;
                                    tcanalCaseCDR.OperatorID = record.OperatorID;

                                    if (record.ReceivedCallingNumber != null && record.GeneratedCallingNumber !=null)
                                        tcanalCaseCDR.StatusId = new Guid("4ea323c2-56ba-46db-a84d-5792009924a3"); // Fraud

                                    else
                                        tcanalCaseCDR.StatusId = new Guid("43f65fbf-ba78-4211-a0bb-88edc91b26ff"); // Suspect

                                    tCAnalCaseCDRs.Add(tcanalCaseCDR);
                                }

                                PrepareCDRCasesToInsert caseCDRsList = new PrepareCDRCasesToInsert();
                                if (tCAnalCaseCDRs.Count > 0)
                                {
                                    var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
                                    Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("TCAnal_CaseCDR");

                                    foreach (var caseCDR in tCAnalCaseCDRs)
                                    {
                                        dynamic runtimeCDR = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                                        runtimeCDR.ID = caseCDR.ID;
                                        runtimeCDR.CallingNumber = caseCDR.CallingNumber;
                                        runtimeCDR.CalledNumber = caseCDR.CalledNumber;
                                        runtimeCDR.FirstAttempt = caseCDR.FirstAttempt;
                                        runtimeCDR.LastAttempt = caseCDR.LastAttempt;
                                        runtimeCDR.NumberOfCDRs = caseCDR.NumberOfCDRs;
                                        runtimeCDR.StatusId = caseCDR.StatusId;
                                        runtimeCDR.OperatorID = caseCDR.OperatorID;
                                        caseCDRsList.TCAnalListToInsert.Add(runtimeCDR);
                                    }

                                    inputArgument.OutputQueue.Enqueue(caseCDRsList);
                                }
                            }

                        });
                    }
                } while (!ShouldStop(handle) && hasItem);
            });
        }
    }
}
