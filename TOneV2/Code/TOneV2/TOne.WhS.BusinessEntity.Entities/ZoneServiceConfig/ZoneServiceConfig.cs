using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class ZoneServiceConfig
    {
        public int ZoneServiceConfigId { get; set; }

        public string Symbol { get; set; }

        public ServiceConfigSetting Settings { get; set; }

        public string SourceId { get; set; }
    }

    public class ServiceConfigSetting
    {
        public string Name { get; set; }

        public string Color { get; set; }

        public int? ParentId { get; set; }

        public string Description { get; set; }


    }
}
