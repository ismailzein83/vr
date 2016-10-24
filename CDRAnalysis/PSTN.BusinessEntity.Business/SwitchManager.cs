using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class SwitchManager
    {
        private Dictionary<int, Switch> GetCachedSwitches()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitches",
               () =>
               {
                   ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
                   IEnumerable<Switch> switches = dataManager.GetSwitches();
                   return switches.ToDictionary(kvp => kvp.SwitchId, kvp => kvp);
               });
        }

        public IDataRetrievalResult<SwitchDetail> GetFilteredSwitches(DataRetrievalInput<SwitchQuery> input)
        {
            var allSwitches = GetCachedSwitches();

            Func<Switch, bool> filterExpression = (switchObject) =>
                 (input.Query.Name == null || switchObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                  ((input.Query.SelectedBrandIds != null ? input.Query.SelectedBrandIds.Contains(switchObject.BrandId) : input.Query.SelectedBrandIds == null))
                 &&
                 (input.Query.AreaCode == null || input.Query.AreaCode.Contains(switchObject.AreaCode));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSwitches.ToBigResult(input, filterExpression, SwitchDetailMapper));
        }

        public IEnumerable<SwitchInfo> GetSwitchesInfo(SwitchFilter filter)
        {
            var switches = GetCachedSwitches();
            Func<Switch, bool> filterPredicate = null;

            if (filter != null)
            {
                filterPredicate = (s) => (filter.ExcludedIds == null || !filter.ExcludedIds.Contains(s.SwitchId));
            }

            return switches.MapRecords(SwitchInfoMapper, filterPredicate);

        }

        
        public Switch GetSwitchById(int switchId)
        {
            var switches = GetCachedSwitches();
            return switches.GetRecord(switchId);
        }

        public Switch GetSwitchByDataSourceId(Guid dataSourceId)
        {
            var switches = GetCachedSwitches();
            return switches.FindRecord(x => x.DataSourceId == dataSourceId);
        }
       
        public IEnumerable<SwitchInfo> GetAllSwitches()
        {
            var switches = GetCachedSwitches();
            return switches.MapRecords(SwitchInfoMapper);
        }

        public IEnumerable<SwitchInfo> GetSwitchesByIds(List<int> switchIds)
        {
            var switches = GetCachedSwitches();
            return switches.MapRecords(SwitchInfoMapper, x => switchIds.Contains(x.SwitchId));
        }

        public IEnumerable<Guid> GetSwitchAssignedDataSources()
        {
            var switches = GetCachedSwitches();
            return switches.FindAllRecords( x => x.DataSourceId != null).Select(x=>x.DataSourceId.Value);
        }

        public InsertOperationOutput<SwitchDetail> AddSwitch(Switch switchObj)
        {
            InsertOperationOutput<SwitchDetail> insertOperationOutput = new InsertOperationOutput<SwitchDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int switchId = -1;

            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            bool inserted = dataManager.AddSwitch(switchObj, out switchId);

            if (inserted)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                switchObj.SwitchId = switchId;
                insertOperationOutput.InsertedObject = SwitchDetailMapper(switchObj);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<SwitchDetail> UpdateSwitch(Switch switchObj)
        {
            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();

            bool updated = dataManager.UpdateSwitch(switchObj);
            UpdateOperationOutput<SwitchDetail> updateOperationOutput = new UpdateOperationOutput<SwitchDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updated)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SwitchDetailMapper(switchObj);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public DeleteOperationOutput<object> DeleteSwitch(int switchId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.InUse;

            ISwitchDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();

            bool deleted = dataManager.DeleteSwitch(switchId);

            if (deleted)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISwitchDataManager _dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSwitchesUpdated(ref _updateHandle);
            }
        }

        private SwitchInfo SwitchInfoMapper(Switch switchObject)
        {
            SwitchInfo switchInfo = new SwitchInfo();

            switchInfo.SwitchId = switchObject.SwitchId;
            switchInfo.Name = switchObject.Name;

            return switchInfo;
        }

        private SwitchDetail SwitchDetailMapper(Switch switchObject)
        {
            SwitchDetail switchDetail = new SwitchDetail();
            SwitchBrand brand = new SwitchBrandManager().GetSwitchBrandById(switchObject.BrandId);
            switchDetail.Entity = switchObject;
            switchDetail.BrandName = (brand!=null)? brand.Name: string.Empty;

            return switchDetail;
        }

    }
}
