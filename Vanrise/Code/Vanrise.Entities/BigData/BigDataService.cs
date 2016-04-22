using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class BigDataService
    {
        public long BigDataServiceId { get; set; }

        public string URL { get; set; }

        public long TotalCachedRecordsCount { get; set; }

        public HashSet<Guid> CachedObjectIds { get; set; }
    }
}
