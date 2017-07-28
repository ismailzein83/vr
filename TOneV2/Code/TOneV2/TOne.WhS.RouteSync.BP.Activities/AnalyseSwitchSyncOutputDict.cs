using System;
using System.Activities;
using System.Linq;
using System.Collections.Concurrent;
using Vanrise.Common;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.BP.Activities
{
    public class AnalyseSwitchSyncOutputDict : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ConcurrentDictionary<string, SwitchSyncOutput>> SwitchSyncOutputDict { get; set; }

        [RequiredArgument]
        public InArgument<int> NumberOfSwitches { get; set; }

        [RequiredArgument]
        public OutArgument<bool> AllSwitchesFailed { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ConcurrentDictionary<string, SwitchSyncOutput> switchSyncOutputDict = this.SwitchSyncOutputDict.Get(context);
            int numberOfSwitches = this.NumberOfSwitches.Get(context);
            bool allSwitchesFailed = false;
            if (switchSyncOutputDict != null)
            {
                var failedItems = switchSyncOutputDict.Values.FindAllRecords(itm => itm.SwitchSyncResult == SwitchSyncResult.Failed);
                allSwitchesFailed = failedItems != null && failedItems.Count() == numberOfSwitches;
            }
            this.AllSwitchesFailed.Set(context, allSwitchesFailed);
        }
    }
}