using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class Switch
    {
        public int SwitchId { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public string AreaCode { get; set; }

        public TimeSpan TimeOffset { get; set; }

        public int? DataSourceId { get; set; }
    }
}
