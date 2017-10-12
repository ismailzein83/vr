using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SwitchReleaseCauseManager
    {
     
        SwitchManager switchManager = new SwitchManager();
      
        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<SwitchReleaseCauseDetail> GetFilteredSwitchReleaseCauses(Vanrise.Entities.DataRetrievalInput<SwitchReleaseCauseQuery> input)
        {
            var allSwitchReleaseCauses = this.GetCachedSwitchReleaseCauses();

            Func<SwitchReleaseCause, bool> filterExpression = (prod) =>
             (input.Query.ReleaseCode == null || prod.ReleaseCode.ToLower().Contains(input.Query.ReleaseCode.ToLower())) &&

                (input.Query.SwitchIds == null || input.Query.SwitchIds.Contains(prod.SwitchId)) &&
                (!input.Query.IsDelivered.HasValue || input.Query.IsDelivered.Value == (prod.Settings.IsDelivered)) &&
                (input.Query.Description == null || prod.Settings.Description.ToLower().Contains(input.Query.Description.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSwitchReleaseCauses.ToBigResult(input, filterExpression, SwitchReleaseCauseDetailMapper));
        }
        public Vanrise.Entities.InsertOperationOutput<SwitchReleaseCauseDetail> AddSwitchReleaseCause(SwitchReleaseCause switchReleaseCause)
        {
            int switchReleaseCauseId = -1;
            InsertOperationOutput<SwitchReleaseCauseDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SwitchReleaseCauseDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            ISwitchReleaseCauseDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchReleaseCauseDataManager>();
            bool insertActionSucc = dataManager.AddSwitchReleaseCause(switchReleaseCause, out switchReleaseCauseId);
            if (insertActionSucc)
            {
                switchReleaseCause.SwitchReleaseCauseId = switchReleaseCauseId;
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = SwitchReleaseCauseDetailMapper(switchReleaseCause);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            return insertOperationOutput;

        }
        public SwitchReleaseCause GetSwitchReleaseCause(int switchReleaseCauseId)
        {
            var allSwitchReleaseCauses = this.GetCachedSwitchReleaseCauses();
            return allSwitchReleaseCauses.GetRecord(switchReleaseCauseId);
        }
        public Vanrise.Entities.UpdateOperationOutput<SwitchReleaseCauseDetail> UpdateSwitchReleaseCause(SwitchReleaseCause switchReleaseCause)
        {
            UpdateOperationOutput<SwitchReleaseCauseDetail> updateOperationOutput = new UpdateOperationOutput<SwitchReleaseCauseDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            ISwitchReleaseCauseDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchReleaseCauseDataManager>();
            bool updateActionSucc = dataManager.UpdateSwitchReleaseCause(switchReleaseCause);
            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SwitchReleaseCauseDetailMapper(switchReleaseCause);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        #endregion
   
        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISwitchReleaseCauseDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchReleaseCauseDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreSwitchReleaseCausesUpdated(ref _updateHandle);
            }
        }
        #endregion
     
        #region Mappers
        private SwitchReleaseCauseDetail SwitchReleaseCauseDetailMapper(SwitchReleaseCause switchReleaseCause)
        {
            var switchReleaseCauseDetail = new SwitchReleaseCauseDetail()
            {
                SwitchReleaseCauseId = switchReleaseCause.SwitchReleaseCauseId,
                ReleaseCode = switchReleaseCause.ReleaseCode,
                SwitchId = switchReleaseCause.SwitchId,
                SwitchName = switchManager.GetSwitchName(switchReleaseCause.SwitchId),

            };

            switchReleaseCauseDetail.Description = switchReleaseCause.Settings.Description;
            switchReleaseCauseDetail.IsDelivered = switchReleaseCause.Settings.IsDelivered;



            return switchReleaseCauseDetail;
        }
        #endregion
     
        #region Private Methods
        Dictionary<int, SwitchReleaseCause> GetCachedSwitchReleaseCauses()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitchReleaseCauses",
               () =>
               {
                   ISwitchReleaseCauseDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchReleaseCauseDataManager>();
                   IEnumerable<SwitchReleaseCause> switchReleaseCauses = dataManager.GetSwitchReleaseCauses();
                   return switchReleaseCauses.ToDictionary(cn => cn.SwitchReleaseCauseId, cn => cn);
               });
        }
        #endregion

 
    }
}
