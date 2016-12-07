using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using NP.IVSwitch.Data;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace NP.IVSwitch.Business
{
    public class EndPointManager
    {
        #region Public Methods
        public EndPoint GetEndPoint(int endPointId)
        {
            Dictionary<int, EndPoint> cachedEndPoint = this.GetCachedEndPoint();
            return cachedEndPoint.GetRecord(endPointId);
        }

        public IDataRetrievalResult<EndPointDetail> GetFilteredEndPoints(DataRetrievalInput<EndPointQuery> input)
        {
            //Get Carrier by id
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            EndPointCarrierAccountExtension extendedSettingsObject = carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(input.Query.CarrierAccountId.Value);

            Dictionary<int, EndPointInfo> aclEndPointDic = new Dictionary<int, EndPointInfo>();
            Dictionary<int, EndPointInfo> userEendPointDic = new Dictionary<int, EndPointInfo>();

            if (extendedSettingsObject != null)
            {
                if (extendedSettingsObject.AclEndPointInfo != null) aclEndPointDic = extendedSettingsObject.AclEndPointInfo.ToDictionary(k => k.EndPointId, v => v);
                if (extendedSettingsObject.UserEndPointInfo != null) userEendPointDic = extendedSettingsObject.UserEndPointInfo.ToDictionary(k => k.EndPointId, v => v);
            }
            var allEndPoints = GetCachedEndPoint();
            Func<EndPoint, bool> filterExpression =
                x => (aclEndPointDic.ContainsKey(x.EndPointId) || (userEendPointDic.ContainsKey(x.EndPointId)));

            return DataRetrievalManager.Instance.ProcessResult(input, allEndPoints.ToBigResult(input, filterExpression, EndPointDetailMapper));
        }

        public InsertOperationOutput<EndPointDetail> AddEndPoint(EndPointToAdd endPointItem)
        {
            InsertOperationOutput<EndPointDetail> insertOperationOutput = new InsertOperationOutput<EndPointDetail>
            {
                Result = InsertOperationResult.Failed,
                InsertedObject = null
            };
            IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
            Helper.SetSwitchConfig(dataManager);
            int endPointId;
            if (Insert(endPointItem, out endPointId))
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = EndPointDetailMapper(GetEndPoint(endPointId));
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }

        private bool Insert(EndPointToAdd endPointItem, out int endPointId)
        {
            endPointId = 0;
            List<EndPointInfo> userEndPointInfoList = new List<EndPointInfo>();
            List<EndPointInfo> aclEndPointInfoList = new List<EndPointInfo>();

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var profileId = carrierAccountManager.GetCarrierProfileId(endPointItem.CarrierAccountId);
            if (profileId.HasValue)
            {
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                AccountCarrierProfileExtension accountExtended = carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(profileId.Value);
                if (accountExtended != null && accountExtended.CustomerAccountId.HasValue)
                    endPointItem.Entity.AccountId = accountExtended.CustomerAccountId.Value;
                else
                    endPointItem.Entity.AccountId = CreateNewAccount(profileId.Value);

                EndPointCarrierAccountExtension endPointsExtendedSettings =
                    carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(
                        endPointItem.CarrierAccountId) ??
                    new EndPointCarrierAccountExtension();

                if (endPointsExtendedSettings.AclEndPointInfo != null)
                    aclEndPointInfoList = endPointsExtendedSettings.AclEndPointInfo;
                if (endPointsExtendedSettings.UserEndPointInfo != null)
                    userEndPointInfoList = endPointsExtendedSettings.UserEndPointInfo;

                IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
                Helper.SetSwitchConfig(dataManager);

                return dataManager.Insert(endPointItem.Entity, userEndPointInfoList, aclEndPointInfoList, out endPointId);
            }
            return false;
        }
        private int CreateNewAccount(int profileId)
        {
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            CarrierProfile carrierProfile = carrierProfileManager.GetCarrierProfile(profileId);
            AccountManager accountManager = new AccountManager();
            Account account = accountManager.GetAccountInfoFromProfile(carrierProfile, true);
            int accountId = accountManager.AddAccount(account);

            AccountCarrierProfileExtension extendedSettings = carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(profileId);
            extendedSettings.CustomerAccountId = accountId;

            carrierProfileManager.UpdateCarrierProfileExtendedSetting<AccountCarrierProfileExtension>(profileId, extendedSettings);
            return accountId;
        }
        public Vanrise.Entities.UpdateOperationOutput<EndPointDetail> UpdateEndPoint(EndPointToAdd endPointItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<EndPointDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
            Helper.SetSwitchConfig(dataManager);

            if (dataManager.Update(endPointItem.Entity))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = EndPointDetailMapper(this.GetEndPoint(endPointItem.Entity.EndPointId));
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
            IEndPointDataManager _dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
            protected override bool IsTimeExpirable { get { return true; } }

        }
        #endregion

        #region Private Methods

        Dictionary<int, EndPoint> GetCachedEndPoint()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetEndPoint",
                () =>
                {
                    IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
                    Helper.SetSwitchConfig(dataManager);
                    return dataManager.GetEndPoints().ToDictionary(x => x.EndPointId, x => x);
                });
        }

        #endregion

        #region Mappers

        public EndPointDetail EndPointDetailMapper(EndPoint endPoint)
        {
            EndPointDetail endPointDetail = new EndPointDetail()
            {
                Entity = endPoint,
                CurrentStateDescription = Vanrise.Common.Utilities.GetEnumDescription<State>(endPoint.CurrentState),
                CurrentEndPointType = Vanrise.Common.Utilities.GetEnumDescription<EndPointType>(endPoint.EndPointType),

            };

            return endPointDetail;
        }

        #endregion
    }
}