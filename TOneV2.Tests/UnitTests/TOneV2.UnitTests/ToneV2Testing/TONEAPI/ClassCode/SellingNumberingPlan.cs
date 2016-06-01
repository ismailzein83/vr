using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{



    public class EntityNP
    {
        public int SellingNumberPlanId { get; set; }
        public string Name { get; set; }
    }

    public class DatumNP
    {
        public EntityNP Entity { get; set; }
    }

    public class SellingNumberPlan
    {
        public object ResultKey { get; set; }
        public List<DatumNP> Data { get; set; }
        public int TotalCount { get; set; }
    }
}