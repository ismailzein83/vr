using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class BigResult<T>
    {
        public string ResultKey { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
