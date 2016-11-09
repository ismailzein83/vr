using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class TypeManager : ITypeManager
    {
        #region Singleton

        static TypeManager s_instance = new TypeManager();
        public static TypeManager Instance
        {
            get
            {
                return s_instance;
            }
        }

        #endregion

        static VRDictionary<string, int> s_typesIds = new VRDictionary<string, int>(true);

        public int GetTypeId(Type t)
        {
            return GetTypeId(t.AssemblyQualifiedName);
        }

        public int GetTypeId(IVanriseType vanriseType)
        {
            return GetTypeId(vanriseType.UniqueTypeName);
        }

        public int GetTypeId(string typeUniqueName)
        {
            if (typeUniqueName == null)
                throw new ArgumentNullException("typeUniqueName");
            int ruleTypeId;
            if (!s_typesIds.TryGetValue(typeUniqueName, out ruleTypeId))
            {
                ITypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<ITypeDataManager>();
                ruleTypeId = dataManager.GetTypeId(typeUniqueName);
                s_typesIds.TryAdd(typeUniqueName, ruleTypeId);
            }

            return ruleTypeId;
        }
    }
}
