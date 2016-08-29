using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceRule : ISourceItem
    {
        public string SourceId
        {
            get;
            set;
        }
        public RouteRule RouteRule { get; set; }
    }
}
