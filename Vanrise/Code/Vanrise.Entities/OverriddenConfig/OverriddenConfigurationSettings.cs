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

        public abstract Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context);
    }

    public interface IOverriddenConfigurationGetBehaviorContext
    {

    }

    public abstract class OverriddenConfigurationBehavior
    {
        public virtual string ModuleName { get { return null; } }
        public abstract void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context);
    }

    public interface IOverriddenConfigurationBehaviorGenerateScriptContext
    {
        List<OverriddenConfiguration> Configs { get; }
        void AddEntityScript(string entityName, string entityScript);
    }
}
