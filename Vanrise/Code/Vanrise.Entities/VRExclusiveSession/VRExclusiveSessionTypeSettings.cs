using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRExclusiveSessionTypeSettings : VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("50ACE946-07FC-4D3E-B246-8622250ED0FC"); }
        }

        public VRExclusiveSessionTypeExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class VRExclusiveSessionTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetTargetName(IVRExclusiveSessionGetTargetNameContext context);

        public abstract bool DoesUserHaveTakeAccess(IVRExclusiveSessionDoesUserHaveTakeAccessContext context);

        public abstract bool DoesUserHaveAdminAccess(IVRExclusiveSessionDoesUserHaveAdminAccessContext context);
    }

    public interface IVRExclusiveSessionGetTargetNameContext
    {
        string TargetId { get; }
    }

    public class VRExclusiveSessionGetTargetNameContext : IVRExclusiveSessionGetTargetNameContext
    {
        public string TargetId { get; set; }
    }


    public interface IVRExclusiveSessionDoesUserHaveTakeAccessContext
    {
        int UserId { get; }
    }

    public interface IVRExclusiveSessionDoesUserHaveAdminAccessContext
    {
        int UserId { get; }
    }

    public class VRExclusiveSessionTypeExtendedSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "ExclusiveSessionTypeSettings";

        public string Editor { get; set; }
    }
}
