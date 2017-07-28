using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Business
{
    public static class Helper
    {
        public static SwitchSyncOutput MergeSwitchSyncOutputItems(List<SwitchSyncOutput> switchSyncOutputList)
        {
            if (switchSyncOutputList == null || switchSyncOutputList.Count == 0)
                return null;

            HashSet<string> distinctSwitchIds = switchSyncOutputList.Select(itm => itm.SwitchId).ToHashSet();
            if (distinctSwitchIds.Count > 1)
                throw new Exception("switchSyncOutputList items should be related to one Switch.");

            SwitchSyncOutput result = new SwitchSyncOutput() { SwitchId = distinctSwitchIds.First(), SwitchSyncResult = SwitchSyncResult.Succeed, SwitchRouteSynchroniserOutputList = new List<SwitchRouteSynchroniserOutput>() };
            foreach (SwitchSyncOutput switchSyncOutput in switchSyncOutputList)
            {
                result.SwitchSyncResult = (int)result.SwitchSyncResult > (int)switchSyncOutput.SwitchSyncResult ? result.SwitchSyncResult : switchSyncOutput.SwitchSyncResult;
                if (switchSyncOutput.SwitchRouteSynchroniserOutputList != null)
                    result.SwitchRouteSynchroniserOutputList.AddRange(switchSyncOutput.SwitchRouteSynchroniserOutputList);
            }
            return result;
        }

        public static SwitchSyncOutput MergeSwitchSyncOutputItems(SwitchSyncOutput firstItem, SwitchSyncOutput secondItem)
        {
            List<SwitchSyncOutput> items = new List<SwitchSyncOutput>();
            if (firstItem != null)
                items.Add(firstItem);

            if (secondItem != null)
                items.Add(secondItem);

            return MergeSwitchSyncOutputItems(items);
        }
    }
}
