using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TABS.Addons.Utilities;

namespace TABS.Plugins.Framework
{
    public interface IPluginModule  
    {
        /// <summary>
        /// Initialize the Module. This should be called once by TOne upon initialization.
        /// </summary>
        void Initialize();

        /// <summary>
        /// The Module ID
        /// </summary>
        string ID { get; }

        /// <summary>
        /// The Module Name
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Return a "Resource Name" / PluginResource dictionary of all pages that this Module implements.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, PluginResource> GetResources();


        /// <summary>
        /// Return a dictioanry of runnables (the runnable name, and a runtime Type that can construct it)
        /// </summary>
        /// <returns></returns>
        Dictionary<string, Type> GetRunnables();
    }
}
