using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRModuleVisibility
    {
        public abstract Guid ConfigId { get; }

        public abstract void GenerateScript(IVRModuleVisibilityGenerateScriptContext context);

        public virtual VRModuleVisibilityEditorRuntime GetEditorRuntime()
        {
            return null;
        }
    }

    public interface IVRModuleVisibilityGenerateScriptContext
    {
        void AddEntityScript(string entityName, string entityScript);
    }

    public abstract class VRModuleVisibilityEditorRuntime
    {

    }
}
