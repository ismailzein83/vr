using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace QM.CLITester.iTestIntegration
{
    public class ITestExtendedZoneSettingBehavior : ExtendedZoneSettingBehavior
    {
        public const string EXTENDEDZONESETTING_KEYNAME = "CLISYSZoneExtendedSettings";
        public override void ApplyExtendedSettings(IApplyExtendedZoneSettingsContext context)
        {
            if (context.Zone == null)
                throw new NullReferenceException("context.Zone");
            if (context.Zone.Settings == null)
                throw new NullReferenceException(String.Format("context.Zone.Settings {0}", context.Zone.ZoneId));
            if (context.Zone.Settings.ExtendedSettings == null)
                throw new NullReferenceException(String.Format("context.Zone.Settings.ExtendedSettings {0}", context.Zone.ZoneId));

            ITestZoneManager itestZoneManager = new ITestZoneManager();
            ITestZone matchITestZone = context.Zone.IsFromTestingConnectorZone ? itestZoneManager.GetZone(context.Zone.SourceId): itestZoneManager.GetMatchZone(context.ZoneCodes);
            if (matchITestZone != null)
            {
                context.Zone.Settings.IsOffline = matchITestZone.IsOffline;
                var itestExtendedSettings = context.Zone.Settings.ExtendedSettings.GetOrCreateItem(EXTENDEDZONESETTING_KEYNAME, () => new ITestExtendedZoneSetting()) as ITestExtendedZoneSetting;
                itestExtendedSettings.ITestZoneId = matchITestZone.ZoneId;
                itestExtendedSettings.ITestCountryId = matchITestZone.CountryId;
            }
            else
                context.Zone.Settings.IsOffline = true;
        }
    }
}
