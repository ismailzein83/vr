using System;

namespace Demo.BestPractices.Entities
{
    public class ChildDetail
    {
        public long ChildId { get; set; }
        public string Name { get; set; }
        public string ParentName { get; set; }
        public string AreaDescription { get; set; }
    }
}