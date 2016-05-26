using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
    
    public class SwitchEntity
    {
        public int SwitchId { get; set; }
        public string Name { get; set; }
        public object SourceId { get; set; }
    }

    public class SwitchDatum
    {
        public SwitchEntity switchEntity { get; set; }
    }

    public class SwitchClass
    {
        public object ResultKey { get; set; }
        public List<SwitchDatum> switchData { get; set; }
        public int TotalCount { get; set; }
    }
}