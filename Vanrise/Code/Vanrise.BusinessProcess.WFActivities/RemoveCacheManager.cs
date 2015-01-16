using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Caching;

namespace Vanrise.BusinessProcess.WFActivities
{

    public sealed class RemoveCacheManager : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> CacheManagerId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            CacheManagerFactory.RemoveCacheManager(this.CacheManagerId.Get(context));
        }
    }
}
