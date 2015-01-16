using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Caching;

namespace Vanrise.BusinessProcess.WFActivities
{

    public sealed class CreateCacheManager<T> : CodeActivity where T : class, ICacheManager
    {
        [RequiredArgument]
        public OutArgument<Guid> CacheManagerId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Guid cacheManagerId = Guid.NewGuid();
            var cacheManager = CacheManagerFactory.GetCacheManager<T>(cacheManagerId);
            this.CacheManagerId.Set(context, cacheManagerId);
        }
    }
}
