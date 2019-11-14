using System;

namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public class ZoneData
    {
        public ZoneDataDetail data { get; set; }
    }

    public class ZoneDataDetail
    {
        public int ver { get; set; }
        public string adminState { get; set; }
    }
    
    public class PostCallResponse
    {
        public bool Success { get; set; }
    }
}