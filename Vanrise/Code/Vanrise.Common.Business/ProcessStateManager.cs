//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Caching;
//using Vanrise.Common.Data;
//using Vanrise.Entities;
//using static Vanrise.Common.Business.ExtensionConfigurationManager;

//namespace Vanrise.Common.Business
//{
//    public class ProcessStateManager : IProcessStateManager
//    {
//        #region ctor/Local Variables
//        #endregion

//        #region Public Methods

//        IProcessStateDataManager dataManager = CommonDataManagerFactory.GetDataManager<IProcessStateDataManager>();
//        public T GetProcessStateSetting<T>(string uniqueName) where T : ProcessStateSettings
//        {
//            if (uniqueName == null)
//                throw new ArgumentNullException("processStateUniqueName");

//            var setting = GetCachedProcessStates().GetRecord(uniqueName);
//            return setting != null ? setting as T : null;
//        }

//        public bool InsertOrUpdate(string uniqueName, ProcessStateSettings processState)
//        {
//            return dataManager.InsertOrUpdate(uniqueName, processState);
//        }

//        #endregion

//        #region Private Methods

//        private Dictionary<string, ProcessStateSettings> GetCachedProcessStates()
//        {
//            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetProcessStates",
//               () =>
//               {
//                   IProcessStateDataManager dataManager = CommonDataManagerFactory.GetDataManager<IProcessStateDataManager>();
//                   IEnumerable<ProcessState> processStates = dataManager.GetProcessStates();
//                   return processStates.ToDictionary(item => item.UniqueName, item=> item.Settings);
//               });
//        }



//        #endregion
//    }
//}
