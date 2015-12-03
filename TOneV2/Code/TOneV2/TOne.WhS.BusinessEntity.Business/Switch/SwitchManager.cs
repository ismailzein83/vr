using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SwitchManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SwitchDetail> GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            var allSwitches = GetCachedSwitches();

            Func<Switch, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSwitches.ToBigResult(input, filterExpression, SwitchDetailMapper));
        }

        public Switch GetSwitch(int switchId)
        {
            var switchs = GetCachedSwitches();
            return switchs.GetRecord(switchId);
        }

        public IEnumerable<SwitchInfo> GetSwitchesInfo()
        {
            var switchs = GetCachedSwitches();
            return switchs.MapRecords(SwitchInfoMapper);
        }

        public Vanrise.Entities.InsertOperationOutput<SwitchDetail> AddSwitch(Switch whsSwitch)
        {
            Vanrise.Entities.InsertOperationOutput<SwitchDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SwitchDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int switchId = -1;

            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            bool insertActionSucc = dataManager.Insert(whsSwitch, out switchId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                whsSwitch.SwitchId = switchId;
                insertOperationOutput.InsertedObject = SwitchDetailMapper(whsSwitch);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SwitchDetail> UpdateSwitch(Switch whsSwitch)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();

            bool updateActionSucc = dataManager.Update(whsSwitch);
            Vanrise.Entities.UpdateOperationOutput<SwitchDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SwitchDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SwitchDetailMapper(whsSwitch);
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<SwitchDetail> DeleteSwitch(int switchId)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            Vanrise.Entities.DeleteOperationOutput<SwitchDetail> deleteOperationOutput = new Vanrise.Entities.DeleteOperationOutput<SwitchDetail>();
            bool updateActionSucc = dataManager.Delete(switchId);
            if (updateActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }
            else
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;
            return deleteOperationOutput;
        }


        #region Private Members

        public Dictionary<int, Switch> GetCachedSwitches()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitches",
               () =>
               {
                   ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
                   IEnumerable<Switch> switchs = dataManager.GetSwitches();
                   return switchs.ToDictionary(cn => cn.SwitchId, cn => cn);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISwitchDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSwitchesUpdated(ref _updateHandle);
            }
        }

        private SwitchInfo SwitchInfoMapper(Switch whsSwitch)
        {
            return new SwitchInfo()
            {
                SwitchId = whsSwitch.SwitchId,
                Name = whsSwitch.Name,
            };
        }

        private SwitchDetail SwitchDetailMapper(Switch whsSwitch)
        {
            SwitchDetail switchDetail = new SwitchDetail();
            switchDetail.Entity = whsSwitch;
            return switchDetail;
        }

        #endregion

    }
}
