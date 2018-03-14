using System;

namespace Vanrise.Rules.Entities
{
    public class Rule
    {
        public int RuleId { get; set; }

        public int TypeId { get; set; }

        public string RuleDetails { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public string SourceId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedTime { get; set; }

        public int CreatedBy { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public int LastModifiedBy { get; set; }
    }
}
