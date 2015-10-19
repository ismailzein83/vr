using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public TrunkDetail GetTrunkDetialById(int trunkId)
        {
            var trunks = GetCachedTrunks();
            return trunks.MapRecord(TrunkDetailMapper, x => x.TrunkId == trunkId);
        }

        public Trunk GetTrunkById(int trunkId)
        {
            var trunks = GetCachedTrunks();
            return trunks.GetRecord(trunkId);
        }


        public TrunkInfo GetTrunkBySymbol(string symbol)
        {
            var trunks = GetCachedTrunks();
            return trunks.MapRecord(TrunkInfoMapper, x => x.Symbol == symbol);
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
                insertOperationOutput.Result = InsertOperationResult.Succeeded;

                if (trunkObj.LinkedToTrunkId != null)
                {
                    int linkedToTrunkId = (int)trunkObj.LinkedToTrunkId;

                    dataManager.UnlinkTrunk(linkedToTrunkId);
                    dataManager.LinkTrunks(trunkId, linkedToTrunkId);
                }

                insertOperationOutput.InsertedObject = GetTrunkDetialById(trunkId);
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

                updateOperationOutput.UpdatedObject = GetTrunkDetialById(trunkObj.TrunkId);
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
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;

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
              
        TrunkDetail TrunkDetailMapper(Trunk trunk)
        {
            SwitchManager manager= new SwitchManager();
            SwitchDetail currentSwitch = manager.GetSwitchById(trunk.SwitchId);
            Trunk currentTrunk = null;
            if (trunk.LinkedToTrunkId.HasValue)
                currentTrunk=GetTrunkById(trunk.LinkedToTrunkId.Value);

            TrunkDetail trunkDetail = new TrunkDetail();
            trunkDetail.TrunkId = trunk.TrunkId;
            trunkDetail.Name = trunk.Name;
            trunkDetail.Symbol = trunk.Symbol;
            trunkDetail.SwitchId = trunk.SwitchId;
            trunkDetail.SwitchName = (currentSwitch !=null ?currentSwitch.Name : string.Empty );
            trunkDetail.Type = trunk.Type;
            trunkDetail.Direction = trunk.Direction;
            trunkDetail.LinkedToTrunkId = trunk.LinkedToTrunkId;
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
