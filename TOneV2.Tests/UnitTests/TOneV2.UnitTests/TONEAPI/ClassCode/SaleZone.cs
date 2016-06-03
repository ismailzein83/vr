using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
    public class EntitySZ
    {
        public Int64 SaleZoneId { get; set; }
        public int SellingNumberPlanId { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }
        public DateTime BED { get; set; }
        public object EED { get; set; }
        public object SourceId { get; set; }


    }





    public class DatumSZ
    {
        public EntitySZ Entity { get; set; }
        public string CountryName { get; set; }
        public string SellingNumberPlanName { get; set; }
    }

    public class JasonSZ
    {
        public object ResultKey { get; set; }
        public List<DatumSZ> Data { get; set; }
        public int TotalCount { get; set; }
    }



}