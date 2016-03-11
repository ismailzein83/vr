using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Business
{
    public class ApplyExtendedZoneSettingsContext : IApplyExtendedZoneSettingsContext
    {
        public Zone Zone { get; set; }

        public List<string> ZoneCodes { get; set; }
    }
}
