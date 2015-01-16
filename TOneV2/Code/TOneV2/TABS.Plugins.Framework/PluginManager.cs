using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using TABS.Addons.Utilities;

namespace TABS.Plugins.Framework
{
    public class PluginManager
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(PluginManager));

        private static List<Pair<Assembly, IPluginModule>> _LoadedPlugins;
        public delegate void LicensedPluginsNeededDelegate(out bool shouldCheckPlugins, out string[] licensePlugins);
        public static event LicensedPluginsNeededDelegate LicensedPluginsNeeded;

        protected static void OnLicensedPluginsNeeded(out bool shouldCheckPlugins, out string[] licensePlugins)
        {
            shouldCheckPlugins = false;
            licensePlugins = null;
            if (LicensedPluginsNeeded != null)
                LicensedPluginsNeeded(out shouldCheckPlugins, out licensePlugins);
        }

        public static void LoadPlugins()
        {
            log.Info("Loading Plugins");
            try
            {
                _LoadedPlugins = new List<Pair<Assembly, IPluginModule>>();

                //Get the path of the bin directory of the website
                //System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(System.Web.HttpRuntime.BinDirectory);
                //System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Assembly.GetExecutingAssembly().Location);
                string PlugInsLocation = (System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString());
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(PlugInsLocation);

                //Get the licensed Plugins
                bool shouldCheckPlugins;
                string[] licensedPlugins;
                OnLicensedPluginsNeeded(out shouldCheckPlugins, out licensedPlugins);
                //Loop through the files in bin and check for plugins, add plugins to the List when found
                foreach (System.IO.FileInfo fi in di.GetFiles())
                {
                    if (fi.Extension == ".dll" &&
                        fi.Name != "TABS.Plugins.Framework.dll" &&
                        fi.Name.StartsWith("TABS.Plugins."))
                    {
                        // Load an assembly from file
                        Assembly loadedAssembly = Assembly.LoadFrom(fi.FullName);
                        log.Info("Checking Plugin Assembly: " + fi.Name + " ...");
                        if (!shouldCheckPlugins || (licensedPlugins != null && licensedPlugins.Contains(loadedAssembly.GetName().Name)))
                        {
                            Type implementation;
                            if (CheckIfPlugin(loadedAssembly, out implementation))
                            {
                                log.Info("Initializing Plugin Assembly: " + fi.Name + " ...");
                                try
                                {
                                    IPluginModule pluginModule = implementation.GetConstructor(new Type[] { }).Invoke(null) as IPluginModule;
                                    _LoadedPlugins.Add(new Pair<Assembly, IPluginModule>(loadedAssembly, pluginModule));
                                    pluginModule.Initialize();
                                    log.Info("Plugin " + pluginModule.Name + " Initialized.");
                                }
                                catch (Exception ex)
                                {
                                    log.Error(string.Format("Error Initializing Plugin Module from Assembly: {0}", loadedAssembly.GetName().Name), ex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error Loading Plugins", ex);
            }

        }
        /// <summary>
        /// Check if the assembly contains an implementation of IPluginModule interface
        /// </summary>
        /// <param name="loadedAssembly"></param>
        /// <param name="implementation">The Type that implements the IPluginModule interface if any</param>
        /// <returns></returns>
        private static bool CheckIfPlugin(Assembly loadedAssembly, out Type implementation)
        {
            bool implementationFound;
            implementation = null;
            Type[] publicTypes = loadedAssembly.GetExportedTypes();
            int counter = 0;
            implementationFound = false;
            Type type = null;
            while (!implementationFound && counter < publicTypes.Length)
            {
                type = publicTypes[counter];
                implementationFound = typeof(IPluginModule).IsAssignableFrom(type);
                counter++;
            }
            implementationFound = implementationFound && type != typeof(IPluginModule);
            if (implementationFound)
                implementation = type;
            return implementationFound;
        }

        public static List<IPluginModule> LoadedPluginModules
        {
            get
            {
                return LoadedPlugins.Select(p => p.Second).ToList();
            }
        }

        public static List<Pair<Assembly, IPluginModule>> LoadedPlugins
        {
            get
            {
                lock (typeof(PluginManager))
                {
                    if (_LoadedPlugins == null)
                        LoadPlugins();
                }
                return _LoadedPlugins;
            }
        }

        public static List<Assembly> PluginAssemblies
        {
            get
            {
                return LoadedPlugins.Select(p => p.First).ToList();
            }
        }

    }
}
