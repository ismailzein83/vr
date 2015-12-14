using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace QM.CLITester.Entities
{
    public class SourceProfile : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string Name { get; set; }
    }
}
