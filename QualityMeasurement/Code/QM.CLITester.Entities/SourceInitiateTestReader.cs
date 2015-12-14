using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace QM.CLITester.Entities
{
    public abstract class SourceInitiateTestReader : ISourceItemReader<SourceInitiateTest> 
    {
        public int ConfigId { get; set; }

        public abstract bool UseSourceItemId
        {
            get;
        }

        public abstract IEnumerable<SourceInitiateTest> GetChangedItems(ref object updatedHandle);
    }
}
