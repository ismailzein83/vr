using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
        public class EntityCR
    {
        public int CurrencyId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public object SourceId { get; set; }
    }

    public class DatumCR
    {
        public EntityCR Entity { get; set; }
        public object IsMain { get; set; }
    }

    public class Currency
    {
        public object ResultKey { get; set; }
        public List<DatumCR> Data { get; set; }
        public int TotalCount { get; set; }
    }
}