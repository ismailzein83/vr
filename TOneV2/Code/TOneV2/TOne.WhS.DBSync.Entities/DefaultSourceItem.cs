using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.DBSync.Entities
{
    public class DefaultSourceItem : ISourceItem
    {
        public string SourceId
        {
            get { return string.Empty; }
        }
    }
}
