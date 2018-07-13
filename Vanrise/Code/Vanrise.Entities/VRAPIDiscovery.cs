using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRAPIDiscovery
    {
        public abstract List<string> GetModuleNames(IVRAPIDiscoveryGetModuleNamesContext context);

        public abstract List<Type> GetControllerTypes(IVRAPIDiscoveryGetControllerTypesContext context);

        public abstract bool IsCacheExpired(ref DateTime LastCheckTime);
    }

    public interface IVRAPIDiscoveryGetModuleNamesContext
    {

    }

    public interface IVRAPIDiscoveryGetControllerTypesContext
    {

    }
}
