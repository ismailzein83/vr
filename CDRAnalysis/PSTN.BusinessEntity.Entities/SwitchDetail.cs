using System;

namespace PSTN.BusinessEntity.Entities
{
    public class SwitchDetail
    {
        public int SwitchId { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public string TypeName { get; set; }

        public string AreaCode { get; set; }

        public TimeSpan TimeOffset { get; set; }

        public int? DataSourceId { get; set; }
    }
}
