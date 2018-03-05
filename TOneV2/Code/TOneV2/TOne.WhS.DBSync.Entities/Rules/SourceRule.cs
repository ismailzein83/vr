using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;
using Vanrise.Rules;
using Vanrise.Rules.Entities;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceRule : ISourceItem
    {
        public string SourceId { get; set; }

        public Rule Rule { get; set; }
    }
}
