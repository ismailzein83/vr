﻿using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class TrunkManager
    {
        private Dictionary<int, Trunk> GetCachedTrunks()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetTrunks",
               () =>
               {
                   ITrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITrunkDataManager>();
                   IEnumerable<Trunk> trunks = dataManager.GetTrunks();
                   return trunks.ToDictionary(kvp => kvp.TrunkId, kvp => kvp);
               });
        }

        public IDataRetrievalResult<TrunkDetail> GetFilteredTrunks(DataRetrievalInput<TrunkQuery> input)
        {
            var allTrunks = GetCachedTrunks();

            Func<Trunk, bool> filterExpression = (trunkObject) =>
                 (input.Query.Name == null || trunkObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&

                 (input.Query.Symbol == null || trunkObject.Symbol.ToLower().Contains(input.Query.Symbol.ToLower()))
                 &&

                  ((input.Query.SelectedSwitchIds != null ? input.Query.SelectedSwitchIds.Contains(trunkObject.SwitchId) : input.Query.SelectedSwitchIds == null))

                   &&
                  ((input.Query.SelectedTypes != null ? input.Query.SelectedTypes.Contains(trunkObject.Type) : input.Query.SelectedTypes == null))

                   &&
                  ((input.Query.SelectedDirections != null ? input.Query.SelectedDirections.Contains(trunkObject.Direction) : input.Query.SelectedDirections == null))


                 &&
                 ((input.Query.IsLinkedToTrunk == null) || (input.Query.IsLinkedToTrunk == true && trunkObject.LinkedToTrunkId != null) || (input.Query.IsLinkedToTrunk == false && trunkObject.LinkedToTrunkId == null));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allTrunks.ToBigResult(input, filterExpression, TrunkDetailMapper));
        }

        public Trunk GetTrunkById(int trunkId)
        {
            var trunks = GetCachedTrunks();
            return trunks.GetRecord(trunkId);
        }


        public Trunk GetTrunkBySymbol(string symbol)
        {
            var trunks = GetCachedTrunks();
            return trunks.FindRecord(x => x.Symbol == symbol);
        }


        public IEnumerable<TrunkInfo> GetTrunksBySwitchIds(TrunkFilter trunkFilterObj)
        {
            var trunks = GetCachedTrunks();
            return trunks.MapRecords(TrunkInfoMapper, x => trunkFilterObj.SwitchIds.Contains(x.SwitchId));
        }

        public IEnumerable<TrunkInfo> GetTrunks()
        {
            var trunks = GetCachedTrunks();
            return trunks.MapRecords(TrunkInfoMapper);
        }

        public IEnumerable<TrunkInfo> GetTrunksByIds(List<int> trunkIds)
        {
            var trunks = GetCachedTrunks();
            return trunks.MapRecords(TrunkInfoMapper, x => trunkIds.Contains(x.TrunkId));
        }

        public InsertOperationOutput<TrunkDetail> AddTrunk(Trunk trunkObj)
        {
            InsertOperationOutput<TrunkDetail> insertOperationOutput = new InsertOperationOutput<TrunkDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int trunkId = -1;

            ITrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITrunkDataManager>();
            bool inserted = dataManager.AddTrunk(trunkObj, out trunkId);

            if (inserted)
            {

                trunkObj.TrunkId = trunkId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;

                if (trunkObj.LinkedToTrunkId != null)
                {
                    int linkedToTrunkId = (int)trunkObj.LinkedToTrunkId;

                    dataManager.UnlinkTrunk(linkedToTrunkId);
                    dataManager.LinkTrunks(trunkId, linkedToTrunkId);
                }

                insertOperationOutput.InsertedObject = TrunkDetailMapper(trunkObj);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetTrunks");
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<TrunkDetail> UpdateTrunk(Trunk trunkObj)
        {
            UpdateOperationOutput<TrunkDetail> updateOperationOutput = new UpdateOperationOutput<TrunkDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ITrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITrunkDataManager>();

            bool updated = dataManager.UpdateTrunk(trunkObj);

            if (updated)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;

                dataManager.UnlinkTrunk(trunkObj.TrunkId);

                if (trunkObj.LinkedToTrunkId != null)
                {
                    int linkedToTrunkId = (int)trunkObj.LinkedToTrunkId;
                    dataManager.UnlinkTrunk(linkedToTrunkId);

                    dataManager.LinkTrunks(trunkObj.TrunkId, linkedToTrunkId);
                }

                updateOperationOutput.UpdatedObject = TrunkDetailMapper(trunkObj);

                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetTrunks");
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public DeleteOperationOutput<object> DeleteTrunk(int trunkId, int? linkedToTrunkId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;

            ITrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ITrunkDataManager>();

            bool deleted = dataManager.DeleteTrunk(trunkId);

            if (linkedToTrunkId != null)
            {
                int id = (int)linkedToTrunkId;
                dataManager.UnlinkTrunk(id);
            }

            if (deleted)
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetTrunks");
            }


            return deleteOperationOutput;
        }


        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ITrunkDataManager _dataManager = PSTNBEDataManagerFactory.GetDataManager<ITrunkDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreTrunksUpdated(ref _updateHandle);
            }
        }

        TrunkDetail TrunkDetailMapper(Trunk trunkObj)
        {
            SwitchManager manager = new SwitchManager();
            Switch currentSwitch = manager.GetSwitchById(trunkObj.SwitchId);
            Trunk currentTrunk = null;
            if (trunkObj.LinkedToTrunkId.HasValue)
                currentTrunk = GetTrunkById(trunkObj.LinkedToTrunkId.Value);

            TrunkDetail trunkDetail = new TrunkDetail();
            trunkDetail.Entity = trunkObj;
            trunkDetail.SwitchName = (currentSwitch != null ? currentSwitch.Name : string.Empty);
            trunkDetail.LinkedToTrunkName = (currentTrunk != null ? currentTrunk.Name : string.Empty);
            return trunkDetail;
        }

        TrunkInfo TrunkInfoMapper(Trunk trunk)
        {
            TrunkInfo trunkInfo = new TrunkInfo();

            trunkInfo.TrunkId = trunk.TrunkId;
            trunkInfo.Name = trunk.Name;
            trunkInfo.SwitchId = trunk.SwitchId;

            return trunkInfo;
        }

    }
}
