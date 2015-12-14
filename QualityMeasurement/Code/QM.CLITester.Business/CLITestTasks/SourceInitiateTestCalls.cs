using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business.EntitySynchronization;
using Vanrise.Entities.EntitySynchronization;

namespace QM.CLITester.Business
{
    public class SourceInitiateTestCalls : SourceItemSynchronizer<SourceInitiateTest, TestCall, ISourceItemReader<SourceInitiateTest>>
    {
        public SourceInitiateTestCalls(ISourceItemReader<SourceInitiateTest> sourceItemReader)
            : base(sourceItemReader)
        {

        }

        protected override void AddItems(List<TestCall> itemsToAdd)
        {
            TestCallManager manager = new TestCallManager();
            foreach (var s in itemsToAdd)
            {
            }
        }

        protected override TestCall BuildItemFromSource(SourceInitiateTest sourceItem)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceItemIds)
        {
            throw new NotImplementedException();
        }

        protected override void ReserveIdRange(int nbOfIds, out long startingId)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateItems(List<TestCall> itemsToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
