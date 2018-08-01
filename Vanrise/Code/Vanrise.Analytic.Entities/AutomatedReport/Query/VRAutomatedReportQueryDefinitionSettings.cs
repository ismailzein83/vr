using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportQueryDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("A5B07696-61B4-4371-B52C-770667E0EB05"); }
        }
        public VRAutomatedReportQueryDefinitionExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class VRAutomatedReportQueryDefinitionExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string RuntimeEditor { get; set; }

        public abstract bool  DoesUserHaveAccess(IVRAutomatedReportQueryDefinitionExtendedSettingsContext context);
    }

    public interface IVRAutomatedReportQueryDefinitionExtendedSettingsContext
    {
        int LoggedInUserId { get; }
    }
}
