using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.CDRImport.Business;
using Vanrise.Fzero.CDRImport.Data;
using System.Linq;
using Vanrise.Queueing;
using System.Configuration;

namespace Vanrise.Fzero.CDRImport.BP.Activities
{

    #region Arguments Classes

    public class UnifyRepeatedCDRsInput
    {
        public BaseQueue<StagingCDRBatch> InputQueue { get; set; }

        public BaseQueue<CDRBatch> OutputQueue { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }

    #endregion

    public class UnifyRepeatedCDRs : DependentAsyncActivity<UnifyRepeatedCDRsInput>
    {

        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<StagingCDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }


        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<CDRBatch>());



            base.OnBeforeExecute(context, handle);
        }

        static string configuredDirectory = ConfigurationManager.AppSettings["LoadCDRsDirectory"];

        class CallPartie
        {
            public string CDPN { get; set; }

            public string CGPN { get; set; }
        }



        public class RelatedCDR
        {
            public int SwitchID { get; set; }
            public string InTrunkSymbol { get; set; }
            public string OutTrunkSymbol { get; set; }
            public decimal? DurationInSeconds { get; set; }
            public DateTime? DisconnectDateTime { get; set; }
            public DateTime? ConnectDateTime { get; set; }
        }


        protected override void DoWork(UnifyRepeatedCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int minimumGapBetweenRepeatedCDRs = 5;
            DateTime? currentConnectDateTime = null;
            CDR currentCDR = new CDR();
            Dictionary<CallPartie, List<RelatedCDR>> unifiedCDRs = new Dictionary<CallPartie, List<RelatedCDR>>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {

                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (stagingcdrBatch) =>
                        {
                            var serializedCDRs = Vanrise.Common.Compressor.Decompress(System.IO.File.ReadAllBytes(stagingcdrBatch.StagingCDRBatchFilePath));
                            System.IO.File.Delete(stagingcdrBatch.StagingCDRBatchFilePath);
                            var stagingCDRs = Vanrise.Common.ProtoBufSerializer.Deserialize<List<StagingCDR>>(serializedCDRs);

                            List<RelatedCDR> relatedCDRs;
                            CallPartie callPartie;
                            RelatedCDR relatedCDR;

                            var firstConnectDateTime = stagingCDRs.First().ConnectDateTime.Value;
                            var CurrentStagingCDRs = stagingCDRs.Where(x => ToPositive(x.ConnectDateTime.Value.Subtract(currentConnectDateTime.Value).TotalSeconds) <= minimumGapBetweenRepeatedCDRs);
                            stagingCDRs = stagingCDRs.Except(CurrentStagingCDRs).ToList();

                            foreach (var stagingCDR in CurrentStagingCDRs)
                            {
                                if (currentConnectDateTime != null)
                                {
                                    PrepareUnifiedCDR(stagingCDR, out relatedCDRs, out callPartie, out relatedCDR);

                                    if (unifiedCDRs.TryGetValue(callPartie, out relatedCDRs))
                                    {
                                        relatedCDRs.Add(relatedCDR);
                                        unifiedCDRs[callPartie] = relatedCDRs;
                                    }

                                    else
                                    {
                                        relatedCDRs.Add(relatedCDR);
                                        unifiedCDRs.Add(callPartie, relatedCDRs);
                                    }
                                }
                                else
                                {
                                    currentConnectDateTime = stagingCDR.ConnectDateTime.Value;
                                    PrepareUnifiedCDR(stagingCDR, out relatedCDRs, out callPartie, out relatedCDR);
                                    relatedCDRs.Add(relatedCDR);
                                    unifiedCDRs.Add(callPartie, relatedCDRs);
                                }
                            }
                            inputArgument.OutputQueue.Enqueue(BuildCDRBatch(unifiedCDRs));

                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Loading CDRs from Database to Memory");


        }

        private CDRBatch BuildCDRBatch(Dictionary<CallPartie, List<RelatedCDR>> unifiedCDRs)
        {
            List<CDR> cdrBatch = new List<CDR>();

            foreach (KeyValuePair<CallPartie, List<RelatedCDR>> entry in unifiedCDRs)
            {
                cdrBatch.Add(new CDR()
                {
                    CallType = CallType.IncomingVoiceCall,
                    ConnectDateTime = entry.Value.First().ConnectDateTime,
                    Destination = entry.Key.CDPN,
                    MSISDN = entry.Key.CGPN,
                    DisconnectDateTime = entry.Value.First().DisconnectDateTime,
                    DurationInSeconds = entry.Value.First().DurationInSeconds,
                    InTrunk = entry.Value.First().InTrunkSymbol,
                    OutTrunk = entry.Value.First().OutTrunkSymbol
                }
                    );



                cdrBatch.Add(new CDR()
                {
                    CallType = CallType.OutgoingVoiceCall,
                    ConnectDateTime = entry.Value.First().ConnectDateTime,
                    Destination = entry.Key.CGPN,
                    MSISDN = entry.Key.CDPN,
                    DisconnectDateTime = entry.Value.First().DisconnectDateTime,
                    DurationInSeconds = entry.Value.First().DurationInSeconds,
                    InTrunk = entry.Value.First().InTrunkSymbol,
                    OutTrunk = entry.Value.First().OutTrunkSymbol
                }
                    );

            }


            var cdrsBytes = Vanrise.Common.Compressor.Compress(Vanrise.Common.ProtoBufSerializer.Serialize(cdrBatch));
            string filePath = !String.IsNullOrEmpty(configuredDirectory) ? System.IO.Path.Combine(configuredDirectory, Guid.NewGuid().ToString()) : System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllBytes(filePath, cdrsBytes);
            return new CDRBatch
            {
                CDRBatchFilePath = filePath
            };



        }

        private static double ToPositive(double seconds)
        {
            var difference = seconds < 0 ? seconds : -seconds;
            return difference;
        }

        private static void PrepareUnifiedCDR(Entities.StagingCDR stagingCDR, out List<RelatedCDR> relatedCDRs, out CallPartie callPartie, out RelatedCDR relatedCDR)
        {
            relatedCDRs = new List<RelatedCDR>();
            callPartie = new CallPartie() { CDPN = stagingCDR.CDPN, CGPN = stagingCDR.CGPN };
            relatedCDR = new RelatedCDR()
            {
                ConnectDateTime = stagingCDR.ConnectDateTime,
                DisconnectDateTime = stagingCDR.DisconnectDateTime,
                DurationInSeconds = stagingCDR.DurationInSeconds,
                InTrunkSymbol = stagingCDR.InTrunkSymbol,
                OutTrunkSymbol = stagingCDR.OutTrunkSymbol,
                SwitchID = stagingCDR.SwitchID
            };
        }

        protected override UnifyRepeatedCDRsInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new UnifyRepeatedCDRsInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context)
            };
        }

    }
}
