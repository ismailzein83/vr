using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{

    public class VRTimeZoneDetail
    {
        public VRTimeZone Entity { get; set; }
    }

    public class VRTimeZoneQuery
    {
        public string Name { get; set; }
    }

    public class VRTimeZoneInfo
    {
        public int TimeZoneId { get; set; }

        public string Name { get; set; }
    }

    public class VRTimeZone
    {
        public int TimeZoneId { get; set; }


        public string Name { get; set; }

        public VRTimeZoneSettings Settings { get; set; }

        public string SourceId { get; set; }

        public DateTime CreatedTime { get; set; }

        public int? CreatedBy { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedTime { get; set; }
        
    }

    public class VRTimeZoneSettings
    {
        public TimeSpan Offset { get; set; }
        
    }
}
