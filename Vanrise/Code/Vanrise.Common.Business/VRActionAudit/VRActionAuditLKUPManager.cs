using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    internal class VRActionAuditLKUPManager
    {
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

        private struct LKUPDictKey
        {
            public VRActionAuditLKUPType Type { get; set; }

            public string Name { get; set; }
        }

        static Dictionary<LKUPDictKey, int> s_lkupIds = new Dictionary<LKUPDictKey, int>();
    }
}