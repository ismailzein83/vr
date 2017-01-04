using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
   

    public class EntityCG
    {
        public int Id { get; set; }
        public  string  Code { get; set; }
        public int CountryId { get; set; }
      //  public object SourceId { get; set; }
    }

    public class DatumCG
    {
        public EntityCG Entity { get; set; }
        public string CountryName { get; set; }
    }

    public class jasonrespCG
    {
        public object ResultKey { get; set; }
        public List<DatumCG> Data { get; set; }
        public int TotalCount { get; set; }
    }

}