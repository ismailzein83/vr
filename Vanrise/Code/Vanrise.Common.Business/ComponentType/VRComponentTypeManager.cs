using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;

namespace Vanrise.Common.Business
{
    public class VRComponentTypeManager
    {
        static CacheManager s_cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>();

        private struct GetComponentTypeCacheName
        {
            public Guid ComponentTypeId { get; set; }

            public Type CLRType { get; set; }
        }

        public Q GetComponentType<T, Q>(Guid componentTypeId) where T : VRComponentTypeSettings
            where Q : VRComponentType<T>
        {
            var cacheName = new GetComponentTypeCacheName
            {
                 ComponentTypeId = componentTypeId,
                 CLRType = typeof(T)
            };
            return s_cacheManager.GetOrCreateObject(cacheName, ()=>
            {
                var componentTypeEntity = GetCachedComponentTypes().GetRecord(componentTypeId);
                if (componentTypeEntity == null)
                    throw new NullReferenceException(String.Format("componentTypeEntity '{0}'", componentTypeId));
                if (componentTypeEntity.Settings == null)
                    throw new NullReferenceException(String.Format("componentTypeEntity.Settings '{0}'", componentTypeId));
                T settingsAsT = componentTypeEntity.Settings as T;
                if (settingsAsT == null)
                    throw new NullReferenceException(String.Format("componentTypeEntity.Settings is not of type '{0}'. it is of type '{1}'", typeof(T), componentTypeEntity.Settings.GetType()));
                var componentType = Activator.CreateInstance<Q>();
                componentType.VRComponentTypeId = componentTypeEntity.VRComponentTypeId;
                componentType.Name = componentTypeEntity.Name;
                componentType.Settings = settingsAsT;
                return componentType;
            });
        }

        public T GetComponentTypeSettings<T>(Guid componentTypeId)where T : VRComponentTypeSettings
        {
            var componentType = GetComponentType<T, VRComponentType<T>>(componentTypeId);
            if (componentType == null)
                throw new NullReferenceException(String.Format("componentType '{0}'", componentTypeId));
            return componentType.Settings;
        }

        private ConcurrentDictionary<Guid, VRComponentType> GetCachedComponentTypes()
        {
            return s_cacheManager.GetOrCreateObject("GetCachedComponentTypes", ()=>
            {                
                throw new NotImplementedException();
                ConcurrentDictionary<Guid, VRComponentType> componentTypes = null;
                return componentTypes;
            });
        }

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool ShouldSetCacheExpired()
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
