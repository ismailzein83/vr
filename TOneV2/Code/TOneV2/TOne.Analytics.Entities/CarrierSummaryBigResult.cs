using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CarrierSummaryBigResult<T> : Vanrise.Entities.BigResult<T>
    {
        public T Summary { get; set; }
    }
}
