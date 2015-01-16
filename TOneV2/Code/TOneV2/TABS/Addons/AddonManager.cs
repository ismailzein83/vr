using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TABS.Addons
{
	public sealed class AddonManager
	{
        static object synch = new object();
        static log4net.ILog log = log4net.LogManager.GetLogger("TABS.AddonManager");
        public static string AddonsLocation = string.Empty;
		static List<System.Reflection.Assembly> _AddonAssemblies;
        static Dictionary<string, System.Type> _AvailableSwitchManagers;
        static Dictionary<string, System.Type> _AvailableRunnables;
        static Dictionary<string, System.Type> _AvailableAlertCriterias;
        static Dictionary<string, System.Type> _AvailableCDRStores;

		/// <summary>
		/// Gets the addon assemblies (located in the Bin folder)
		/// The Addon manager will try to load all DLL files in that folder.
		/// </summary>
		public static List<System.Reflection.Assembly> AddonAssemblies 
		{
			get
			{
                lock (synch)
                {
                    if (_AddonAssemblies == null)
                    {
                        _AddonAssemblies = new List<System.Reflection.Assembly>();

                        try
                        {
                            AddonsLocation = (System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString());
                            //AddonsLocation = (new System.IO.DirectoryInfo(System.Web.HttpRuntime.BinDirectory).FullName);
                            // Look for assemblies in the provided path
                            foreach (string filename in System.IO.Directory.GetFiles(AddonsLocation, "TABS.Addons.*.dll"))
                            {
                                try
                                {
                                    System.Reflection.Assembly possible = System.Reflection.Assembly.LoadFrom(filename);
                                    _AddonAssemblies.Add(possible);
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine("Failed to load file: " + filename + ", reason: " + ex, "Addon Assembly Loading");
                                    log.Error("Failed to load file as Addon Assembly", ex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("Failed to load Addon Assemblies", ex);
                        }
                        finally
                        {
                            log.InfoFormat("Loaded {0} Addon Assemblies from {1}", _AddonAssemblies.Count, AddonsLocation);
                        }
                    }
                }
				return _AddonAssemblies;
			}
		}


        public static List<System.Reflection.Assembly> PluginAssemblies
        {
            get
            {
                return TABS.Plugins.Framework.PluginManager.PluginAssemblies;
            }
        }

        /// <summary>
        /// Gets the available CDR Stores (built-in and addons) 
        /// </summary>
        public static Dictionary<string, System.Type> AvailableCDRStores
        {
            get
            {
                lock (synch)
                {
                    if (_AvailableCDRStores == null)
                    {
                        _AvailableCDRStores = new Dictionary<string, Type>();

                        // The valid base type
                        System.Type validBaseType = typeof(TABS.Extensibility.ICDRStore);

                        // Full name filter
                        System.Reflection.TypeFilter filter = new System.Reflection.TypeFilter(ObjectAssembler.IsTypeFullName);

                        // Valid types, init from this Assembly first
                        List<Type> validTypes = new List<Type>();
                        foreach (Type possibleType in System.Reflection.Assembly.GetExecutingAssembly().GetExportedTypes())
                            if (possibleType.FindInterfaces(filter, validBaseType.FullName).Length > 0)
                                validTypes.Add(possibleType);

                        foreach (System.Reflection.Assembly addonAssembly in AddonAssemblies.Union(PluginAssemblies))
                        {
                            foreach (System.Type possibleType in addonAssembly.GetExportedTypes())
                                if (possibleType.FindInterfaces(filter, validBaseType.FullName).Length > 0)
                                {
                                    validTypes.Add(possibleType);
                                }
                        }

                        foreach (Type validType in validTypes)
                        {
                            object[] attributes = validType.GetCustomAttributes(typeof(NamedAddon), true);
                            if (attributes.Length > 0)
                            {
                                _AvailableCDRStores[attributes[0].ToString()] = validType;
                            }
                            else
                                _AvailableCDRStores[validType.FullName] = validType;
                        }
                    }
                }
                return _AvailableCDRStores;
            }
        }

        /// <summary>
        /// Gets the available switch managers (from addons) 
        /// </summary>
        public static Dictionary<string, System.Type> AvailableSwitchManagers 
        {
            get
            {
                lock (synch)
                {
                    if (_AvailableSwitchManagers == null)
                    {
                        _AvailableSwitchManagers = new Dictionary<string, Type>();

                        // The valid base type
                        System.Type validBaseType = typeof(TABS.Extensibility.ISwitchManager);

                        List<Type> validTypes = new List<Type>();

                        System.Reflection.TypeFilter filter = new System.Reflection.TypeFilter(ObjectAssembler.IsTypeFullName);

                        foreach (System.Reflection.Assembly addonAssembly in AddonAssemblies.Union(PluginAssemblies))
                        {
                            foreach (System.Type possibleType in addonAssembly.GetExportedTypes())
                                if (possibleType.FindInterfaces(filter, validBaseType.FullName).Length > 0)
                                {
                                    validTypes.Add(possibleType);
                                }
                        }

                        foreach (Type validType in validTypes)
                        {
                            object[] attributes = validType.GetCustomAttributes(typeof(NamedAddon), true);
                            if (attributes.Length > 0)
                            {
                                _AvailableSwitchManagers[attributes[0].ToString()] = validType;
                            }
                            else
                                _AvailableSwitchManagers[validType.FullName] = validType;
                        }
                    }
                }
                return _AvailableSwitchManagers;
            }
        }

        /// <summary>
        /// Gets the available Runnables (from addons)
        /// </summary>
        public static Dictionary<string, System.Type> AvailableRunnables
        {
            get
            {
                lock (synch)
                {
                    if (_AvailableRunnables == null)
                    {
                        _AvailableRunnables = new Dictionary<string, Type>();

                        // The valid base type
                        System.Type validBaseType = typeof(TABS.Extensibility.IRunnable);

                        // Valid types, init from this Assembly first
                        List<Type> validTypes = new List<Type>();
                        foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
                            if ((!type.IsAbstract) && type.IsSubclassOf(typeof(Addons.Runnables.RunnableBase)))
                                validTypes.Add(type);

                        System.Reflection.TypeFilter filter = new System.Reflection.TypeFilter(ObjectAssembler.IsTypeFullName);

                        foreach (System.Reflection.Assembly addonAssembly in AddonAssemblies.Union(PluginAssemblies))
                        {
                            foreach (System.Type possibleType in addonAssembly.GetExportedTypes())
                                if (possibleType.FindInterfaces(filter, validBaseType.FullName).Length > 0)
                                {
                                    validTypes.Add(possibleType);
                                }
                        }

                        foreach (Type validType in validTypes)
                        {
                            object[] attributes = validType.GetCustomAttributes(typeof(NamedAddon), true);
                            if (attributes.Length > 0)
                            {
                                _AvailableRunnables[attributes[0].ToString()] = validType;
                            }
                            else
                                _AvailableRunnables[validType.FullName] = validType;
                        }
                    }
                }
                return _AvailableRunnables;
            }
        }

        /// <summary>
        /// Gets the available Runnables (from addons)
        /// </summary>
        public static Dictionary<string, System.Type> AvailableAlertCriterias
        {
            get
            {
                lock (synch)
                {
                    if (_AvailableAlertCriterias == null)
                    {
                        _AvailableAlertCriterias = new Dictionary<string, Type>();

                        // The valid base type
                        System.Type validBaseType = typeof(TABS.Extensibility.IAlertCriteria);

                        // Valid types, init from this Assembly first
                        List<Type> validTypes = new List<Type>();
                        foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
                            if ((!type.IsAbstract) && typeof(Extensibility.IAlertCriteria).IsAssignableFrom(type))
                                validTypes.Add(type);

                        System.Reflection.TypeFilter filter = new System.Reflection.TypeFilter(ObjectAssembler.IsTypeFullName);

                        foreach (System.Reflection.Assembly addonAssembly in AddonAssemblies)
                        {
                            foreach (System.Type possibleType in addonAssembly.GetExportedTypes())
                                if (possibleType.FindInterfaces(filter, validBaseType.FullName).Length > 0)
                                {
                                    validTypes.Add(possibleType);
                                }
                        }

                        foreach (Type validType in validTypes)
                        {
                            object[] attributes = validType.GetCustomAttributes(typeof(NamedAddon), true);
                            if (attributes.Length > 0)
                            {
                                _AvailableAlertCriterias[attributes[0].ToString()] = validType;
                            }
                            else
                                _AvailableAlertCriterias[validType.FullName] = validType;
                        }
                    }
                    return _AvailableAlertCriterias;
                }
            }
        }

        /// <summary>
        /// Gets an instanciated Switch Manager. The name of the Switch Manager should be 
        /// in the <see cref="AvailableSwitchManagers" /> collection.
        /// </summary>
        /// <param name="name">The name of the Switch Manager</param>
        /// <returns></returns>
        public static TABS.Extensibility.ISwitchManager GetSwitchManager(string name) 
        {
            TABS.Extensibility.ISwitchManager switchManager = null;
            try
            {
                lock (synch)
                {
                    foreach (KeyValuePair<string, Type> manager in AvailableSwitchManagers)
                    {
                        if (manager.Key == name || manager.Value.FullName == name)
                        {
                            switchManager = (TABS.Extensibility.ISwitchManager)manager.Value.GetConstructor(Type.EmptyTypes).Invoke(null);
                            break;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error("Failed to Get Switch Manager: " + name, ex);
            }
            return switchManager;
        }

        /// <summary>
        /// Gets an instanciated CDR Store. The name of the CDR Store should be 
        /// in the <see cref="AvailableCDRStores" /> collection.
        /// </summary>
        /// <param name="name">The name of the CDR Store</param>
        /// <returns></returns>
        public static TABS.Extensibility.ICDRStore GetCDRStore(string name)
        {
            TABS.Extensibility.ICDRStore cdrStore = null;
            try
            {
                lock (synch)
                {
                    foreach (KeyValuePair<string, Type> manager in AvailableCDRStores)
                    {
                        if (manager.Key == name || manager.Value.FullName == name)
                        {
                            cdrStore = (TABS.Extensibility.ICDRStore)manager.Value.GetConstructor(Type.EmptyTypes).Invoke(null);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to Get CDR Store: " + name, ex);
            }
            return cdrStore;
        }

        /// <summary>
        /// Gets an instanciated IRunnable from Available Runnables. The name of the IRunnable should be 
        /// in the <see cref="AvailableRunnables" /> collection.
        /// </summary>
        /// <param name="name">The name of the Runnable</param>
        /// <returns></returns>
        public static TABS.Extensibility.IRunnable GetRunnable(string name)
        {
            TABS.Extensibility.IRunnable runnable = null;
            try
            {
                lock (synch)
                {
                    foreach (KeyValuePair<string, Type> runnablePair in AvailableRunnables)
                    {
                        if (runnablePair.Key == name || runnablePair.Value.FullName == name)
                        {
                            runnable = (TABS.Extensibility.IRunnable)runnablePair.Value.GetConstructor(Type.EmptyTypes).Invoke(null);
                            break;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error("Failed to Get Runnable: " + name, ex);
            }
            return runnable;
        }

        /// <summary>
        /// Gets an instanciated IAlertCriteria from Available Alert Criteria. The name of the Alert Criteria should be 
        /// in the <see cref="AvailableAlertCriterias" /> collection.
        /// </summary>
        /// <param name="name">The name of the Alert Criteria</param>
        /// <returns></returns>
        public static TABS.Extensibility.IAlertCriteria GetAlertCriteria(string name)
        {
            TABS.Extensibility.IAlertCriteria alertCriteria = null;
            try
            {
                lock (synch)
                {
                    foreach (KeyValuePair<string, Type> pair in AvailableAlertCriterias)
                    {
                        if (pair.Key == name || pair.Value.FullName == name)
                        {
                            alertCriteria = (TABS.Extensibility.IAlertCriteria)pair.Value.GetConstructor(Type.EmptyTypes).Invoke(null);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to Get an Alert Criteria: " + name, ex);
            }
            return alertCriteria;
        }


        /// <summary>
        /// Get the description of a named Addon type.
        /// </summary>
        /// <param name="type">The type to the description for. The type should have the Attribute TABS.Addons.NamedAddon</param>
        /// <returns>The description</returns>
        public static string GetAddonDescription(Type namedAddonType)
        {
            object[] attributes = namedAddonType.GetCustomAttributes(typeof(NamedAddon), true);
            if (attributes.Length > 0)
            {
                return ((NamedAddon)attributes[0]).Description;
            }
            else
                return null;
        }
	}
}