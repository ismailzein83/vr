using System;
using System.Collections.Generic;

namespace TABS.Components
{
    /// <summary>
    /// Provides a centralized way to access configured CDR stores in the system.
    /// </summary>
    public class CDRStoreManager
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(CDRStoreManager));

        /// <summary>
        /// The singleton
        /// </summary>
        static CDRStoreManager instance = new CDRStoreManager();

        /// <summary>
        /// List of Configured Stores
        /// </summary>
        protected List<Extensibility.ICDRStore> ConfiguredStores { get; set; }

        /// <summary>
        /// Construct the Store manager
        /// </summary>
        private CDRStoreManager() 
        {
            // Refresh stores
            ConfiguredStores = new List<TABS.Extensibility.ICDRStore>();

            try
            {
                // Get details from System Paramater
                List<SpecialSystemParameters.CDRStoreDetails> configDetails = SpecialSystemParameters.CDRStoreDetails.Get(SystemParameter.ConfiguredCDRStores);

                bool defaultCDRStoreAdded = false;

                // Create a CDR Store for each configured entry
                foreach (SpecialSystemParameters.CDRStoreDetails configDetail in configDetails)
                {
                    Extensibility.ICDRStore cdrStore = TABS.Addons.AddonManager.GetCDRStore(configDetail.ClassName);
                    if (cdrStore != null)
                    {
                        // Allow only once instance of Default CDR Store to exist 
                        if (cdrStore is Addons.CDRStores.DefaultCDRStore)
                        {
                            if (defaultCDRStoreAdded) continue;
                            else defaultCDRStoreAdded = true;
                        }

                        // copy details
                        cdrStore.Description = configDetail.Description;
                        cdrStore.ConfigString = configDetail.ConfigString;
                        cdrStore.ConfigOptions = configDetail.ConfigOptions;
                        cdrStore.IsEnabled = configDetail.IsEnabled;

                        // add to configured stores
                        ConfiguredStores.Add(cdrStore);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error loading the CDR Storage Manager", ex);
            }
        }

        /// <summary>
        /// Save the Given CDRs using all configured CDR Stores
        /// </summary>
        /// <param name="cdrs"></param>
        public static bool Save(NHibernate.ISession session, IEnumerable<CDR> cdrs, out bool allFailed)
        {
            bool success = true;
            allFailed = true;
            foreach (Extensibility.ICDRStore store in instance.ConfiguredStores)
            {
                if (store.IsEnabled)
                {
                    try
                    {
                        store.Put(session, cdrs);
                        allFailed = false;
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Error trying to put CDRs in Store {0} (1)", store.Name, store.Description), ex);
                        success = false;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Invoke a reload of the configured CDR Stores in the System
        /// </summary>
        public static void Reload()
        {
            lock (log)
            {
                log.Info("Reloading CDR Storage Managers");
                instance = new CDRStoreManager();
                log.Info("Reloaded CDR Storage Managers");
            }
        }

        /// <summary>
        /// Get Store description for a CDR Store using its name (or Full Type Name if not a named Addon)
        /// </summary>
        /// <param name="storeName">The name of the ICDRStore</param>
        /// <returns>The description for the store type</returns>
        public static string GetDescription(string storeName)
        {
            string description = null;
            try
            {
                Extensibility.ICDRStore cdrStore = Addons.AddonManager.GetCDRStore(storeName);
                description = ((Addons.NamedAddon)(cdrStore.GetType().GetCustomAttributes(typeof(Addons.NamedAddon), true)[0])).Description;
            }
            catch
            {

            }
            return description;
        }

        /// <summary>
        /// Get HTML help for a CDR Store using its name (or Full Type Name if not a named Addon)
        /// </summary>
        /// <param name="storeName">The name of the ICDRStore</param>
        /// <returns>The help for the  store type in HTML</returns>
        public static string GetHelpHtml(string storeName)
        {
            string helpHtml = null;
            try
            {
                Extensibility.ICDRStore cdrStore = Addons.AddonManager.GetCDRStore(storeName);
                helpHtml = cdrStore.HelpHtml;
            }
            catch
            {

            }
            return helpHtml;
        }


    }
}
