using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Entities
{
    public class ZoneSettings
    {
        public bool IsOffline { get; set; }

        public Dictionary<string, Object> ExtendedSettings { get; set; }
    }

    public abstract class ExtendedZoneSettingBehavior
    {
        public abstract void ApplyExtendedSettings(IApplyExtendedZoneSettingsContext context);
    }

    public interface IApplyExtendedZoneSettingsContext
    {
        Zone Zone { get; }

        List<string> ZoneCodes { get; }
    }
}
