using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class BigResult<T> : IDataRetrievalResult<T>
    {
        public string ResultKey { get; set; }

        public IEnumerable<T> Data { get; set; }

        public int TotalCount { get; set; }
    }
}
