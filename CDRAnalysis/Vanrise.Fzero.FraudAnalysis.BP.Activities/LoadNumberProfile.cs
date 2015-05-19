using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class LoadNumberProfilesInput
    {
        public BaseQueue<NumberProfileBatch> OutputQueue { get; set; }

        public DateTime FromDate { get; set; }


        [RequiredArgument]
        public DateTime ToDate { get; set; }


        [RequiredArgument]
        public int PeriodId { get; set; }

    }

    #endregion

    public class LoadNumberProfiles : BaseAsyncActivity<LoadNumberProfilesInput>
    {

        #region Arguments

        [RequiredArgument]
        public  InOutArgument<BaseQueue<NumberProfileBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }


        [RequiredArgument]
        public InArgument<int> PeriodId { get; set; }


        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<NumberProfileBatch>());



            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(LoadNumberProfilesInput inputArgument, AsyncActivityHandle handle)
        {

            int? BatchSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["NumberProfileBatchSize"].ToString());
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Started ");


            List<NumberProfile> numberProfileBatch = new List<NumberProfile>();

            List<AggregateDefinition> AggregateDefinitions = new AggregateManager().GetAggregateDefinitions();
           


           
            NumberProfile numberProfile = new NumberProfile();
            string MSISDN = string.Empty;


            INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();
            dataManager.LoadCDR(inputArgument.FromDate,inputArgument.ToDate, BatchSize, (normalCDR) =>
            {

                // Agregates

                if (MSISDN == string.Empty)
                {
                    numberProfile = new NumberProfile();
                    foreach (var i in AggregateDefinitions)
                    {
                        i.Aggregation.Reset();
                    }

                    MSISDN = normalCDR.MSISDN;
                }

                else if (MSISDN != normalCDR.MSISDN)
                {

                    foreach (var i in AggregateDefinitions)
                    {
                        numberProfile.AggregateValues.Add(i.Name, i.Aggregation.GetResult());
                    }

                    numberProfileBatch.Add(numberProfile);
                    if (BatchSize.HasValue && numberProfileBatch.Count == BatchSize)
                    {
                        inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
                        {
                            numberProfiles = numberProfileBatch
                        });
                        handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Enqueued Count Items: {0} ", numberProfileBatch.Count);
                        numberProfileBatch = new List<NumberProfile>();
                    }

                    numberProfile = new NumberProfile();
                    foreach (var i in AggregateDefinitions)
                    {
                        i.Aggregation.Reset();
                    }
                    MSISDN = normalCDR.MSISDN;
                }


                numberProfile.SubscriberNumber = MSISDN;


                if (inputArgument.PeriodId == (int)Enums.Period.Day)
                {
                    numberProfile.ToDate = normalCDR.ConnectDateTime.Value.AddDays(1);
                }
                else if (inputArgument.PeriodId == (int)Enums.Period.Hour)
                {
                    numberProfile.ToDate = normalCDR.ConnectDateTime.Value.AddHours(1);
                }

                numberProfile.Period = (Enums.Period)Enum.ToObject(typeof(Enums.Period), inputArgument.PeriodId); ;
                numberProfile.FromDate = inputArgument.FromDate;
                numberProfile.IsOnNet = 1;


                foreach (var i in AggregateDefinitions)
                {
                    i.Aggregation.EvaluateCDR(normalCDR);
                }


                handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "123");
                

            });

            foreach (var i in AggregateDefinitions)
            {
                numberProfile.AggregateValues.Add(i.Name, i.Aggregation.GetResult());
            }

            numberProfileBatch.Add(numberProfile);


            if (numberProfileBatch.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
                {
                    numberProfiles = numberProfileBatch


                });
                handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Enqueued Count Items: {0} ", numberProfileBatch.Count);
            }

            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Ended");

        }

        protected override LoadNumberProfilesInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new LoadNumberProfilesInput
            {
                OutputQueue = this.OutputQueue.Get(context),
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
                PeriodId = this.PeriodId.Get(context)
            };
        }

    }
}
