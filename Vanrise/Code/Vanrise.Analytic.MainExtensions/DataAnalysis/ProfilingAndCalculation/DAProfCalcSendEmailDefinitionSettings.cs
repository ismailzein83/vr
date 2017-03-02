using System;
using Vanrise.Notification.Entities;

namespace Vanrise.Analytic.MainExtensions.DataAnalysis.ProfilingAndCalculation
{
    public class DAProfCalcSendEmailDefinitionSettings : VRActionDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("3B904E8C-2AC0-43DB-A4EF-425869D40544"); }

        }
        public override string RuntimeEditor
        {
            get { return "vr-analytic-daprofcalc-vraction-sendemail"; }
        }
    }
}
