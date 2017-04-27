using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class OverriddenConfigurationSettings
    {
        public OverriddenConfigurationExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class OverriddenConfigurationExtendedSettings
    {
        public abstract Guid ConfigId { get; }
    }
}
