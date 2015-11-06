using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;

namespace Vanrise.Common.Business
{
    public class TypeManager
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

        private TypeManager()
        {

        }
        
        #endregion

        static ConcurrentDictionary<string, int> s_typesIds = new ConcurrentDictionary<string, int>();

        public int GetTypeId(Type t)
        {
            string type = t.AssemblyQualifiedName;
            int ruleTypeId;
            if (!s_typesIds.TryGetValue(type, out ruleTypeId))
            {
                ITypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<ITypeDataManager>();
                ruleTypeId = dataManager.GetTypeId(type);
                s_typesIds.TryAdd(type, ruleTypeId);
            }

            return ruleTypeId;
        }
    }
}
