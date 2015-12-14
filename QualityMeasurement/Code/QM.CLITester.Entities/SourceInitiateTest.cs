using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public class SourceInitiateTest : Vanrise.Entities.EntitySynchronization.ISourceItem
    {
        public string SourceId
        {
            get;
            set;
        }

        public string Name { get; set; }
    }
}
