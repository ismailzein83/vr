﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

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
            return GetTypeId(t.AssemblyQualifiedName);
        }

        public int GetTypeId(IVanriseType vanriseType)
        {
            return GetTypeId(vanriseType.UniqueTypeName);
        }

        private int GetTypeId(string typeUniqueName)
        {            
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
