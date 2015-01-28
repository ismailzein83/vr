using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Activities;

namespace Vanrise.BusinessProcess.WFActivities.Test
{
    [TestClass]
    public class SplitListIntoGroupsTest
    {
        [TestMethod]
        public void SplitListIntoGroups()
        {
            List<int> list = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            int groupSize = 3;
            Dictionary<string, object> inputArgs = new Dictionary<string, object>();
            inputArgs.Add("List", list);
            inputArgs.Add("GroupSize", groupSize);
            var outputArgs = WorkflowInvoker.Invoke(new Vanrise.BusinessProcess.WFActivities.SplitListIntoGroups<int>(), inputArgs);
            List<List<int>> groups = outputArgs["Groups"] as List<List<int>>;
            Assert.IsNotNull(groups);
            Assert.AreEqual(4, groups.Count);
            Assert.AreEqual(3, groups[0].Count);
            Assert.AreEqual(3, groups[1].Count);
            Assert.AreEqual(3, groups[2].Count);
            Assert.AreEqual(1, groups[3].Count);

            Assert.AreEqual(0, groups[0][0]);
            Assert.AreEqual(1, groups[0][1]);
            Assert.AreEqual(2, groups[0][2]);

            Assert.AreEqual(3, groups[1][0]);
            Assert.AreEqual(4, groups[1][1]);
            Assert.AreEqual(5, groups[1][2]);

            Assert.AreEqual(6, groups[2][0]);
            Assert.AreEqual(7, groups[2][1]);
            Assert.AreEqual(8, groups[2][2]);

            Assert.AreEqual(9, groups[3][0]);
        }
    }
}
