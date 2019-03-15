﻿using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRConnectionManager
    {
        ExtensionConfigurationManager _extensionManager;

        public VRConnectionManager()
        {
            _extensionManager = new ExtensionConfigurationManager();
        }

        #region Public Methods

        public IDataRetrievalResult<VRConnectionDetail> GetFilteredVRConnections(DataRetrievalInput<VRConnectionQuery> input)
        {
            var allVRConnections = GetCachedVRConnections();
            Func<VRConnection, bool> filterExpression = (x) =>
            {
                if (input.Query.Name != null && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.ExtensionConfigIds != null && !input.Query.ExtensionConfigIds.Contains(x.Settings.ConfigId))
                    return false;
                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(VRConnectionLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRConnections.ToBigResult(input, filterExpression, VRConnectionDetailMapper));
        }

        public VRConnection GetVRConnection(Guid vrConnectionId, bool isViewedFromUI)
        {
            var vrConnections = GetCachedVRConnections();
            if (vrConnections == null)
                return null;

            var vrConnection = vrConnections.GetRecord(vrConnectionId);
            if (vrConnection != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(VRConnectionLoggableEntity.Instance, vrConnection);

            return vrConnection;
        }

        public VRConnection GetVRConnection(Guid vrConnectionId)
        {

            return GetVRConnection(vrConnectionId, false);
        }

        public string GetVRConnectionName(VRConnection VRConnection)
        {
            if (VRConnection != null)
                return VRConnection.Name;
            else
                return null;
        }

        public VRConnection GetVRConnection<T>(Guid vrConnectionId) where T : VRConnectionSettings
        {
            VRConnection vrConnection = GetVRConnection(vrConnectionId);

            if (vrConnection == null)
                throw new NullReferenceException("vrConnection");

            if (vrConnection.Settings == null)
                throw new NullReferenceException("vrConnection.Settings");

            T connectionSettings = vrConnection.Settings as T;
            if (connectionSettings == null)
                throw new Exception(String.Format("vrConnection.Settings is not of type {0}. it is of type '{1}'.", typeof(T), vrConnection.Settings.GetType()));

            return vrConnection;
        }

        public IEnumerable<VRConnectionInfo> GetVRConnectionInfos(VRConnectionFilter filter)
        {
            var vrConnections = GetCachedVRConnections();
            if (vrConnections == null)
                return null;

            Func<VRConnection, bool> predicate = (itm) =>
            {
                if (filter == null)
                    return true;

                if (filter.Filters != null)
                {
                    foreach (var connectionFilter in filter.Filters)
                        if (!connectionFilter.IsMatched(itm))
                            return false;
                }

                if (filter.ConnectionTypeIds != null && filter.ConnectionTypeIds.Count > 0)
                {
                    itm.ThrowIfNull("itm");
                    itm.Settings.ThrowIfNull("itm.Settings", itm.VRConnectionId);

                    if (!filter.ConnectionTypeIds.Contains(itm.Settings.ConfigId))
                        return false;
                }

                return true;
            };

            return vrConnections.MapRecords(VRConnectionInfoMapper, predicate);
        }

        public IEnumerable<VRConnection> GetVRConnections<T>() where T : VRConnectionSettings
        {
            var vrConnections = GetCachedVRConnections();
            if (vrConnections == null)
                return null;

            Func<VRConnection, bool> predicate = (itm) =>
            {
                if (itm.Settings == null || itm.Settings as T == null)
                    return false;

                return true;
            };
            return vrConnections.FindAllRecords(predicate);
        }

        public IEnumerable<VRConnectionConfig> GetVRConnectionConfigTypes()
        {
            return _extensionManager.GetExtensionConfigurations<VRConnectionConfig>(VRConnectionConfig.EXTENSION_TYPE);
        }

        public IEnumerable<VRConnection> GetAllVRConnections()
        {
            return this.GetCachedVRConnections().MapRecords(x => x).OrderBy(x => x.Name);
        }

        public InsertOperationOutput<VRConnectionDetail> AddVRConnection(VRConnection componentType)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRConnectionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRConnectionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRConnectionDataManager>();

            componentType.VRConnectionId = Guid.NewGuid();

            if (dataManager.Insert(componentType))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(VRConnectionLoggableEntity.Instance, componentType);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRConnectionDetailMapper(componentType);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<VRConnectionDetail> UpdateVRConnection(VRConnection componentType)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRConnectionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRConnectionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRConnectionDataManager>();

            if (dataManager.Update(componentType))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(VRConnectionLoggableEntity.Instance, componentType);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRConnectionDetailMapper(this.GetVRConnection(componentType.VRConnectionId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public Dictionary<Guid, VRConnection> GetCachedVRConnections()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedVRConnections",
               () =>
               {
                   IVRConnectionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRConnectionDataManager>();
                   IEnumerable<VRConnection> vrConnections = dataManager.GetVRConnections();
                   return vrConnections.ToDictionary(itm => itm.VRConnectionId, itm => itm);
               });
        }

        #endregion

        #region Private Methods

        private VRConnectionInfo VRConnectionInfoMapper(VRConnection vrConnection)
        {
            return new VRConnectionInfo()
            {
                Name = vrConnection.Name,
                VRConnectionId = vrConnection.VRConnectionId
            };
        }

        private VRConnectionDetail VRConnectionDetailMapper(VRConnection connection)
        {
            VRConnectionDetail connectionDetail = new VRConnectionDetail()
            {
                Entity = connection
            };
            connectionDetail.TypeDescription = _extensionManager.GetExtensionConfiguration<VRConnectionConfig>(connection.Settings.ConfigId, VRConnectionConfig.EXTENSION_TYPE).Name;
            return connectionDetail;
        }

        #endregion

        #region Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRConnectionDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRConnectionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRConnectionsUpdated(ref _updateHandle);
            }
        }

        private class VRConnectionLoggableEntity : VRLoggableEntityBase
        {
            static VRConnectionManager s_connectionManager = new VRConnectionManager();

            public static VRConnectionLoggableEntity Instance = new VRConnectionLoggableEntity();

            private VRConnectionLoggableEntity()
            {

            }

            public override string EntityUniqueName { get { return "VR_Common_Connection"; } }

            public override string ModuleName { get { return "Common"; } }

            public override string EntityDisplayName { get { return "Connection"; } }

            public override string ViewHistoryItemClientActionName { get { return "VR_Common_Connection_ViewHistoryItem"; } }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VRConnection connection = context.Object.CastWithValidate<VRConnection>("context.Object");
                return connection.VRConnectionId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VRConnection connection = context.Object.CastWithValidate<VRConnection>("context.Object");
                return s_connectionManager.GetVRConnectionName(connection);
            }
        }

        #endregion



    }
}