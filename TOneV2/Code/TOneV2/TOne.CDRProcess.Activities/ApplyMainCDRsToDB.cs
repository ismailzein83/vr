﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Data;
using TOne.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.CDRProcess.Activities
{
    #region Argument Classes

    public class ApplyMainCDRsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    #endregion

    public sealed class ApplyMainCDRsToDB : DependentAsyncActivity<ApplyMainCDRsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override ApplyMainCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyMainCDRsToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }


        protected override void DoWork(ApplyMainCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            //TimeSpan totalTime = default(TimeSpan);
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (preparedMainCDRs) =>
                        {
                            //Console.WriteLine("{0}: start writting {1} records to database", DateTime.Now, dtCodeMatches.Rows.Count);
                            //DateTime start = DateTime.Now;
                            dataManager.ApplyMainCDRsToDB(preparedMainCDRs);
                            //totalTime += (DateTime.Now - start);
                            //Console.WriteLine("{0}: writting batch to database is done in {1}", DateTime.Now, (DateTime.Now - start));
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }


    }
}
