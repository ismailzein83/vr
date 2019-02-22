﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Runtime.Business
{
    public class RuntimeNodeManager
    {
        static IRuntimeNodeDataManager s_dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeDataManager>();

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRuntimeNodeDataManager runtimeNodeDataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeDataManager>();
            object _lastReceivedDataInfo;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return runtimeNodeDataManager.AreRuntimeNodeUpdated(ref _lastReceivedDataInfo);
            }
        }

        public Dictionary<Guid, RuntimeNode> GetCachedRuntimeNodes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
          .GetOrCreateObject("GetCachedRuntimeNodes", () =>
          {
              var allNodes = s_dataManager.GetAllNodes();
              return allNodes != null ? allNodes.ToDictionary(n => n.RuntimeNodeId, n => n) : null;
          });
        }

        public List<RuntimeNode> GetAllNodes()
        {
            var allNodes = GetCachedRuntimeNodes();
            Func<RuntimeNode, bool> filterExpression = (itm) => (true);

            IEnumerable<RuntimeNode> nodes = allNodes.FindAllRecords(filterExpression);
            if (nodes == null)
                return null;
            return nodes.ToList();
        }

        public RuntimeNode GetNode(Guid nodeId)
        {
            return GetCachedRuntimeNodes().GetRecord(nodeId);
        }

        public InsertOperationOutput<RuntimeNodeDetails> AddRuntimeNode(RuntimeNode runtimeNode)
        {
            IRuntimeNodeDataManager runtimeNodeDataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeDataManager>();
            InsertOperationOutput<RuntimeNodeDetails> insertOperationOutput = new InsertOperationOutput<RuntimeNodeDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            runtimeNode.RuntimeNodeId = Guid.NewGuid();
            bool insertActionSuccess = runtimeNodeDataManager.Insert(runtimeNode);
            if (insertActionSuccess)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = RuntimeNodeDetailMapper(runtimeNode);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }

        public UpdateOperationOutput<RuntimeNodeDetails> UpdateRuntimeNode(RuntimeNode runtimeNode)
        {
            IRuntimeNodeDataManager runtimeNodeDataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeDataManager>();
            UpdateOperationOutput<RuntimeNodeDetails> updateOperationOutput = new UpdateOperationOutput<RuntimeNodeDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = runtimeNodeDataManager.Update(runtimeNode); //insert to update
            if (updateActionSuccess)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = RuntimeNodeDetailMapper(runtimeNode);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        #region Mapper
        private RuntimeNodeDetails RuntimeNodeDetailMapper(RuntimeNode node)
        {
            RuntimeNodeDetails runtimeNodeDetails = new RuntimeNodeDetails();
            runtimeNodeDetails.RuntimeNodeId = node.RuntimeNodeId;
            runtimeNodeDetails.Name = node.Name;
            return runtimeNodeDetails;
        }
        #endregion
    }
}
