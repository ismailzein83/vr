using System.Collections.Generic;

namespace RecordAnalysis.Entities
{
    public class C4ProbeSettings
    {
        public List<C4ProbeTrunkMapping> ProbeTrunkMappings { get; set; }
    }

    public class C4ProbeTrunkMapping
    {
        public int SwitchId { get; set; }
        public string PointCode { get; set; }
        public string Trunk { get; set; }
    }
}