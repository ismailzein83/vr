using System;

namespace PSTN.BusinessEntity.Entities
{
    public class Switch
    {
        public int SwitchId { get; set; }

        public string Name { get; set; }

        public int BrandId { get; set; }

        public string AreaCode { get; set; }

        public TimeSpan TimeOffset { get; set; }

        public Guid? DataSourceId { get; set; }
    }
}
