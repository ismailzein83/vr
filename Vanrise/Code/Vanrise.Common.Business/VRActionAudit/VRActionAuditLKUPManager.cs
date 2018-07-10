using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRActionAuditLKUPManager
    {
        #region public Methods
        public IEnumerable<VRActionAuditLKUPInfo> GetVRActionAuditLKUPInfo(VRActionAuditLKUPInfoFilter filter)
        {
            IEnumerable<VRActionAuditLKUP> vrActionAuditLKUPs = GetCachedvrActionAuditLKUPs().Values;
            Func<VRActionAuditLKUP, bool> filterFunc = (vrActionAuditLKUP) =>
            {
                if (filter != null)
                {
                    if (filter.Type.HasValue && filter.Type != vrActionAuditLKUP.Type)
                        return false;
                }
                return true;
            };
            return vrActionAuditLKUPs.MapRecords(VRActionAuditLKUPInfoMapper, filterFunc).OrderBy(x => x.Name);
        }

        public IEnumerable<VRActionAuditLKUP> GetAllLKUPItems()
        {
            return this.GetCachedvrActionAuditLKUPs().Values;
        }

        public string GetVRActionAuditLKUPName(int vrActionAuditLKUPId)
        {
            var vrActionAuditLKUP = GetVRActionAuditLKUP(vrActionAuditLKUPId);
            return (vrActionAuditLKUP != null) ? vrActionAuditLKUP.Name : null;
        }
        public VRActionAuditLKUP GetVRActionAuditLKUP(int vrActionAuditLKUPId)
        {
            var vrActionAuditLKUPs = GetCachedvrActionAuditLKUPs();
            return vrActionAuditLKUPs.GetRecord(vrActionAuditLKUPId);
        }
        public int GetLKUPId(VRActionAuditLKUPType lkupType, string name)
        {
            int id;
            LKUPDictKey lkupKey = new LKUPDictKey
            {
                Type = lkupType,
                Name = name
            };
            if (!s_lkupIds.TryGetValue(lkupKey, out id))
            {
                lock (s_lkupIds)
                {
                    if (s_lkupIds.Count == 0)
                        LoadAllLKUPs();
                    if (!s_lkupIds.TryGetValue(lkupKey, out id))
                    {                        
                        IVRActionAuditLKUPDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRActionAuditLKUPDataManager>();
                        id = dataManager.AddLKUPIfNotExists(lkupType, name);
                        s_lkupIds.Add(lkupKey, id);
                    }
                }
            }
            return id;
        }
        #endregion

       
       
       
        private struct LKUPDictKey
        {
            public VRActionAuditLKUPType Type { get; set; }

            public string Name { get; set; }
        }

        static Dictionary<LKUPDictKey, int> s_lkupIds = new Dictionary<LKUPDictKey, int>();
        #region private Class
        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRActionAuditLKUPDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRActionAuditLKUPDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRActionAuditLKUPUpdated(ref _updateHandle);
            }
        }

        #endregion

         #region private Methods
        private Dictionary<int, VRActionAuditLKUP> GetCachedvrActionAuditLKUPs()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedvrActionAuditLKUPs",
              () =>
              {
                  IVRActionAuditLKUPDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRActionAuditLKUPDataManager>();
                  IEnumerable<VRActionAuditLKUP> vrActionAuditLKUPs = dataManager.GetAll();
                  return vrActionAuditLKUPs.ToDictionary(a => a.VRActionAuditLKUPId, a => a);
              });
        }


        private VRActionAuditLKUPInfo VRActionAuditLKUPInfoMapper(VRActionAuditLKUP vrActionAuditLKUP)
        {
            return new VRActionAuditLKUPInfo
            {
                Name = vrActionAuditLKUP.Name,
                VRActionAuditLKUPId = vrActionAuditLKUP.VRActionAuditLKUPId
            };
        }
        private static void LoadAllLKUPs()
        {
            IVRActionAuditLKUPDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRActionAuditLKUPDataManager>();
            List<VRActionAuditLKUP> allLKUPs = dataManager.GetAll();
            if (allLKUPs != null)
            {
                foreach (var lkup in allLKUPs)
                {
                    LKUPDictKey lkupKey = new LKUPDictKey
                    {
                        Type = lkup.Type,
                        Name = lkup.Name
                    };
                    if (!s_lkupIds.ContainsKey(lkupKey))
                        s_lkupIds.Add(lkupKey, lkup.VRActionAuditLKUPId);
                }
            }
        }
         #endregion
    }
}